using Discord.WebSocket;
using Discord;

public class BotRuntimeManager
{
    bool initDone = false;

    public async Task BotRuntimeTask()
    {
        LogLevelNormalization.InitLogLevelNormalizationStrings();

        // Do not use the logging system before this

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

        // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
        var token = File.ReadAllText("token.txt");
        // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

        await BotReference.clientRef.LoginAsync(TokenType.Bot, token);
        await BotReference.clientRef.StartAsync();

        BotReference.clientRef.Ready += async () =>
        {
            BotReference.connected = true;
            Log.WriteLine("Bot is connected!", LogLevel.DEBUG);

            if (!initDone)
            {
                await CategoryAndChannelInitiator.CreateCategoriesAndChannelsForTheDiscordServer();

                //await LeagueManager.CreateLeaguesOnStartup();

                await DowntimeManager.CheckForUsersThatLeftDuringDowntime();

                await SerializationManager.SerializeUsersOnTheServer();

                BotReference.clientRef.UserJoined += UserManager.HandleUserJoin;

                BotReference.clientRef.ButtonExecuted += ButtonHandler.HandleButtonPress;

                //BotReference.clientRef.MessageUpdated += UserManager.MessageUpdated;

                BotReference.clientRef.GuildMemberUpdated += UserManager.HandleGuildMemberUpdated;

                BotReference.clientRef.UserLeft += UserManager.HandleUserLeaveDelegate;

                //BotReference.clientRef.ChannelCreated += ChannelManager.FinishChannelCreationFromDelegate;

                await DowntimeManager.CheckForUsersThatJoinedAfterDowntime();

                initDone = true; 
            }
        };

        // Listens for the commandService
        await CommandHandler.InstallCommandsAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
}