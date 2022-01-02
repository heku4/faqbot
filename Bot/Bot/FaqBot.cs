using System;
using System.Collections.Generic;
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
                if (update.Type != UpdateType.Message)
                    return;
                if (update.Message.Type != MessageType.Text)
                    return;

                var chatId = update.Message.Chat.Id;
                Console.WriteLine($"Received a '{update.Message.Text}' message in chat {chatId}.");
                
                var questionIndex = _faq.FindIndex(d => d.Question == update.Message.Text.ToString());
                if (questionIndex >= 0)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"{_faq[questionIndex].Answer}"
                    );
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "You said:\n" + update.Message.Text
                    ); 
                }   
            }
        }

        public bool BotSetUp(string settingsFilePath, string faqFilePath)
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
    }
}
