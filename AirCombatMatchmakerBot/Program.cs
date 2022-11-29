using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Net.WebSockets;
using System.IO.Pipes;

public class Program
{
    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All
        };

        BotReference.clientRef = new DiscordSocketClient(config);

        // Listens for the commands
        BotReference.clientRef.MessageReceived += CommandHandler.HandleCommand;

        //GlobalVariables.clientRef.ReactionAdded += ReactionManager.Instance.HandleReactionAddTask;
        //GlobalVariables.clientRef.ReactionRemoved += ReactionManager.Instance.HandleReactionRemove;

        //_client.Log += Log;

        //  You can assign your bot token to a string, and pass that in to connect.
        //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
        //var token = "";

        // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
        // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
        var token = File.ReadAllText("token.txt");
        // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

        await BotReference.clientRef.LoginAsync(TokenType.Bot, token);
        await BotReference.clientRef.StartAsync();

        //_client.MessageUpdated += MessageUpdated;

        BotReference.clientRef.Ready += () =>
        {
            Console.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        };

        Console.WriteLine("INIT DONE");

        /*
        while (true)
        {
            await PipeCommunication.ReceiveDataFromBackend();
        } */

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        // If the message was not in the cache, downloading it will result in getting a copy of `after`.
        var message = await before.GetOrDownloadAsync();
        //Console.WriteLine($"{message} -> {after}");
    }
}