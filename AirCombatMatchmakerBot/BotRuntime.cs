using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

public class BotRuntime
{
    public async Task BotRuntimeTask()
    {
        // Required after some discord API change
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All
        };

        // Make the reference to a static class that has the main bot reference
        BotReference.clientRef = new DiscordSocketClient(config);

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

        BotReference.clientRef.Ready += async() =>
        {
            BotReference.connected = true;
            Log.WriteLine("Bot is connected!", LogLevel.DEBUG);

            // Listens for the commandService
            await CommandHandler.InstallCommandsAsync();

            BotReference.clientRef.UserJoined += PlayerRegisteration.HandleUserJoin;

            return;
        };

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    /*
    private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        // If the message was not in the cache, downloading it will result in getting a copy of `after`.
        var message = await before.GetOrDownloadAsync();
        //Log.WriteLine($"{message} -> {after}");
    } */
}