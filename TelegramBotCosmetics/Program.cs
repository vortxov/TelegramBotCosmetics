using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using TelegramBotCosmetics.Model;
using TelegramBotCosmetics.Service;

namespace TelegramBotCosmetics;

public static class Program
{
    private static TelegramBotClient? Bot;

    public static async Task Main()
    {
        Bot = new TelegramBotClient(Configuration.BotToken);

        User me = await Bot.GetMeAsync();
        Console.Title = me.Username ?? "My awesome Bot";

        using var cts = new CancellationTokenSource();

        ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
        Bot.StartReceiving(Handlers.HandleUpdateAsync,
                           Handlers.HandleErrorAsync,
                           receiverOptions,
                           cts.Token);


        Console.WriteLine($"Start listening for @{me.Username}");

        while (true)
        {
            var cons = Console.ReadLine();
            if (cons == "start")
            {
                Console.WriteLine("Start обработки сайта..");
                GoogleService googleService = new GoogleService();
                googleService.SearchItem();
            }
            if (cons == "updatewhitelist")
            {
                Console.WriteLine("Внесение белого списка...");
                await new WhiteFormulaService().UpdateWhiteList();
            }
            if (cons == "exit")
            {
                cts.Cancel();
                return;
            }
        }
    }
}
