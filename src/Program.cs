using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var apiKeyFromEnv = Environment.GetEnvironmentVariable("ApiKey");
            string faqFilePath = Directory.GetCurrentDirectory() + "/etc/faq.json";
            string settingsFilePath = Directory.GetCurrentDirectory() + "/etc/appsettings.json";
            
            using var cts = new CancellationTokenSource();

            var bot = new FaqBot.FaqBot(settingsFilePath, faqFilePath, apiKeyFromEnv);
            if (!bot.InitStatus)
            {
                Console.WriteLine("Error! Check your json files.");
                return;
            }
            await bot.BotRun(cts);           
        }
    }
}