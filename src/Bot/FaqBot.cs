using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Bot.Models;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.FaqBot
{
    public class FaqBot
    {
        private List<QA> _faq;
        private BotSettings _botSettings;
        public bool InitStatus = true;
        public FaqBot(string settingsFilePath, string faqFilePath)
        {
            if(!BotSetUp(settingsFilePath, faqFilePath))
            {
                InitStatus = false;
            }
        }
        public async Task BotRun(CancellationTokenSource cts)
        {
            var botClient = new TelegramBotClient(_botSettings.ApiKey);
            
            var command = new BotCommand(){Command = "showquestions", Description = "Show a list of questions."};
            var commands = new List<BotCommand>(){command};
            await botClient.SetMyCommandsAsync(commands);
            //await botClient.GetMyCommandsAsync();

            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            botClient.StartReceiving(
                new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync),
                cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

            Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                var textFormMessage = string.Empty;
                var chatId = new long();

                switch (update.Type)
                {
                    case UpdateType.Message:
                        textFormMessage = update.Message.Text;
                        chatId = update.Message.Chat.Id;
                        Console.WriteLine($"Received a '{textFormMessage}' message in chat {chatId}.");
                        break;
                    case UpdateType.CallbackQuery:
                        textFormMessage = update.CallbackQuery.Data;
                        chatId = update.CallbackQuery.From.Id;
                        Console.WriteLine($"Received a '{textFormMessage}' message from user {chatId}.");
                        break;
                }
                if (textFormMessage == "")
                {
                   return;
                }

                if (textFormMessage.StartsWith('/'))
                {
                    if (textFormMessage.Remove(0, 1) == command.Command)
                    {
                        var keyboardWithCallback = new List<List<InlineKeyboardButton>>();

                        foreach (var qaData in _faq)
                        {
                            var botKeyButtonData = InlineKeyboardButton.WithCallbackData(
                                text: $"{qaData.Question}",
                                callbackData: qaData.Question);
                            var buffer = new List<InlineKeyboardButton>();
                            buffer.Add(botKeyButtonData);
                            keyboardWithCallback.Add(buffer);
                        }

                        var markupForAnswer = new InlineKeyboardMarkup(keyboardWithCallback);
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Available questions:",
                            replyMarkup: markupForAnswer,
                            cancellationToken: cts.Token);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Unknown command.",
                            cancellationToken: cts.Token);
                    }
                }
                else
                {
                    var questionIndex = GetQuestionIndex(textFormMessage);
                    if (questionIndex >= 0)
                    {
                        await SendResponseMessageAsync(botClient, _faq[questionIndex].Answer, chatId, cts.Token);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Can't understand your question :\n" + textFormMessage,
                            replyMarkup: new InlineKeyboardMarkup(
                                InlineKeyboardButton.WithCallbackData(
                                    text: $"Click to check '{_faq[0].Question}' question",
                                    callbackData: _faq[0].Question)),
                            cancellationToken: cts.Token
                        ); 
                    }  
                }
            }
        }

        private bool BotSetUp(string settingsFilePath, string faqFilePath)
        {
            if (!System.IO.File.Exists(settingsFilePath))
            {
                Console.WriteLine($"File \"{settingsFilePath}\" is not exist!");
                return false;
            }

            var fileText = System.IO.File.ReadAllText(faqFilePath);
            try
            {
                _faq = JsonSerializer.Deserialize<List<QA>>(fileText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            if (_faq.Count < 1)
            {
                Console.WriteLine("Error! faq.json must contain at least one question with an answer.");
                return false;
            }

            if (!System.IO.File.Exists(faqFilePath))
            {
                Console.WriteLine($"File \"{faqFilePath}\" is not exist!");
                return false;
            }

            fileText = System.IO.File.ReadAllText(settingsFilePath);

            try
            {
               _botSettings = JsonSerializer.Deserialize<BotSettings>(fileText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            
            if (_botSettings.ApiKey == "")
            {
                Console.WriteLine("Error! Your API-key is empty.");
                return false;
            }

            return true;
        }
        
        private int GetQuestionIndex(string message)
        {
            var searchStr = message.Trim();
            if (!searchStr.Contains('?'))
            {
                searchStr += '?';
            }
            var questionIndex = _faq.FindIndex(d => d.Question.ToLower() == searchStr.ToLower()); 
            return questionIndex;
        }

        private async Task SendResponseMessageAsync(ITelegramBotClient bot, Answer responseMessage, long chatId, CancellationToken ct)
        {
            switch (responseMessage.Type)
            {
                case AnswerType.Text:
                    await bot.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"{responseMessage.Text ?? ""}",
                        cancellationToken: ct
                    );
                    break;
                case AnswerType.Document:
                    await bot.SendDocumentAsync(
                        chatId: chatId,
                        document: responseMessage.DocumentUrl,
                        cancellationToken: ct
                    );
                    break;
                case AnswerType.Venue:
                    await bot.SendVenueAsync(
                        chatId: chatId,
                        latitude: responseMessage.VenueData.Latitude,
                        longitude: responseMessage.VenueData.Longitude,
                        title: responseMessage.VenueData.Title,
                        address: responseMessage.VenueData.Address,
                        cancellationToken: ct);
                    break;
                case AnswerType.Location:
                    await bot.SendLocationAsync(
                        chatId: chatId,
                        latitude: responseMessage.LocationData.Latitude,
                        longitude: responseMessage.LocationData.Longitude,
                        cancellationToken: ct);
                    break;
                case AnswerType.Contact:
                    await bot.SendContactAsync(
                        chatId: chatId,
                        firstName: responseMessage.ContactData.FirstName,
                        lastName: responseMessage.ContactData.LastName,
                        phoneNumber: responseMessage.ContactData.PhoneNumber,
                        cancellationToken: ct
                    );
                    break;
            }
        } 
    }
}
