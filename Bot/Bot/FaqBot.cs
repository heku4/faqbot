using System;
using System.Collections.Generic;
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
        public QA[] Faq;
        public BotSettings BotSettings;
        public string ApiKey;
        public FaqBot(string settingsFile, string faqFile)
        {
            if(!BotSetUp(settingsFile, faqFile))
            {
                Faq = null;
                ApiKey = null;
            }
        }
        public async Task BotRun(CancellationTokenSource cts)
        {
            var botClient = new TelegramBotClient(BotSettings.ApiKey);
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

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "You said:\n" + update.Message.Text
                );
            }
        }

        public bool BotSetUp(string settingsFile, string faqFile)
        {
            if (!System.IO.File.Exists(settingsFile))
            {
                Console.WriteLine($"File \"{settingsFile}\" is not exist!");
                return false;
            }

            var fileText = System.IO.File.ReadAllText(faqFile);
            try
            {
                Faq = JsonSerializer.Deserialize<QA[]>(fileText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }


            if (!System.IO.File.Exists(faqFile))
            {
                Console.WriteLine($"File \"{faqFile}\" is not exist!");
                return false;
            }

            fileText = System.IO.File.ReadAllText(settingsFile);

            try
            {
               BotSettings = JsonSerializer.Deserialize<BotSettings>(fileText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
    }
}
