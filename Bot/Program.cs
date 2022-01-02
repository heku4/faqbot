using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string faqFilePath = Directory.GetCurrentDirectory() + "/etc/faq.json";
            string settingsFilePath = Directory.GetCurrentDirectory() + "/etc/appsettings.json";

            using var cts = new CancellationTokenSource();

            var bot = new FaqBot.FaqBot(settingsFilePath, faqFilePath);
            if (bot.BotSettings.ApiKey is null || bot.Faq is null)
            {
                Console.WriteLine("Error! Check your json files.");
                return;
            }
            if (bot.BotSettings.ApiKey == "")
            {
                Console.WriteLine("Error! Your API-key is empty.");
                return;
            }
            await bot.BotRun(cts);           
        }
    }
}