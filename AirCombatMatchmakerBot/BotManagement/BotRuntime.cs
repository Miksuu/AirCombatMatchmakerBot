using Discord.WebSocket;
using Discord;

public class BotRuntimeManager
{
    public async Task BotRuntimeTask()
    {
        LogLevelNormalization.InitLogLevelNormalizationStrings();

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

        // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
        // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
        var token = File.ReadAllText("token.txt");
        // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

        await BotReference.clientRef.LoginAsync(TokenType.Bot, token);
        await BotReference.clientRef.StartAsync();

        BotReference.clientRef.Ready += async () =>
        {
            BotReference.connected = true;
            Log.WriteLine("Bot is connected!", LogLevel.DEBUG);

            await LeagueManager.CreateLeaguesOnStartup();

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