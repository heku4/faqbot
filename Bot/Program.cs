using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Bot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient("");
            var me = await botClient.GetMeAsync();
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );
        }
    }
}
