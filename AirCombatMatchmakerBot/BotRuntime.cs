using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class BotRuntime
{
    public async Task BotRuntimeTask()
    {
        await SerializationManager.DeSerializeDB();

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

        //PlayerRegisteration.channelCreationQueue = new();

        BotReference.clientRef.Ready += async () =>
        {
            BotReference.connected = true;
            Log.WriteLine("Bot is connected!", LogLevel.DEBUG);

            await DowntimeManager.CheckForUsersThatLeftDuringDowntime();

            await SerializationManager.SerializeUsersOnTheServer();

            BotReference.clientRef.UserJoined += UserManager.HandleUserJoin;

            BotReference.clientRef.ButtonExecuted += ButtonHandler.HandleButtonPress; 

            //BotReference.clientRef.MessageUpdated += UserManager.MessageUpdated;

            BotReference.clientRef.GuildMemberUpdated += UserManager.HandleGuildMemberUpdated;

            BotReference.clientRef.UserLeft += UserManager.HandleUserLeaveDelegate;

            BotReference.clientRef.ChannelCreated += ChannelManager.FinishChannelCreationFromDelegate;

            await DowntimeManager.CheckForUsersThatAreNotRegisteredAfterDowntime();
        };

        // Listens for the commandService
        await CommandHandler.InstallCommandsAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

}