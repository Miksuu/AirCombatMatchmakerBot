using Discord;
using Discord.WebSocket;

public class ProgramRuntime
{
    static public EventManager eventManager = new EventManager();

    // !!!
    // ONLY FOR TESTING, DELETES ALL CHANNELS AND CATEGORIES
    // !!!
    static public bool devModeOn = true;
    // !!!
    // ONLY FOR TESTING, DELETES ALL CHANNELS AND CATEGORIES
    // !!!

    public async Task ProgramRuntimeTask()
    {
        LogLevelNormalization.InitLogLevelNormalizationStrings();
        // Do not use the logging system before this !!!

        // Load the data from the file
        if (!devModeOn)
        {
            SerializationManager.DeSerializeDatabases();
        }
        
        // Set up client and return it
        DiscordSocketClient client = BotReference.SetClientRefAndReturnIt();

        // Reads token from the same directory as the .exe
        var token = File.ReadAllText("token.txt");
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        client.Ready += async () =>
        {
            if (!BotReference.Instance.ConnectionState)
            {
                BotReference.Instance.ConnectionState = true;
                Log.WriteLine("Bot is connected!", LogLevel.DEBUG);

                // !!!
                // ONLY FOR TESTING, DELETES ALL CHANNELS AND CATEGORIES
                // !!!

                if (devModeOn)
                {
                    await DevTools.DeleteAllCategoriesChannelsAndRoles();
                }

                // !!!
                // ONLY FOR TESTING, DELETES ALL CHANNELS AND CATEGORIES
                // !!!

                SetupProgramListenersAndSchedulers();
            }
            else
            {
                HandleBotReconnection();
            }
        };

        // Receiving the tacview files
        client.MessageReceived += async (_socketMessage) =>
        {
            await MessageReceiver.ReceiveMessage(_socketMessage);
        };

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private async void SetupProgramListenersAndSchedulers()
    {
        // Creates the league references to the database
        await LeagueManager.CreateLeaguesOnStartupIfNecessary();

        // Creates the categories and the channels from the interfaces
        await CategoryAndChannelManager.CreateCategoriesAndChannelsForTheDiscordServer();

        // Checks the users that left during down time and sets their teams active
        await DowntimeManager.CheckForUsersThatLeftDuringDowntime();

        SetupListeners();

        await DowntimeManager.CheckForUsersThatJoinedAfterDowntime();

        await DiscordBotDatabase.Instance.EventScheduler.CheckCurrentTimeAndExecuteScheduledEvents(true);

        //await SerializationManager.SerializeUsersOnTheServer();
        await SerializationManager.SerializeDB();

        await CommandHandler.InstallCommandsAsync();

        Thread secondThread = new Thread(DiscordBotDatabase.Instance.EventScheduler.EventSchedulerLoop);
        secondThread.Start();
    }

    private void HandleBotReconnection() 
    { 
        Log.WriteLine("Bot was already connected!", LogLevel.WARNING);

        var client = BotReference.GetClientRef();

        client.UserJoined -= UserManager.HandleUserJoin;
        client.ButtonExecuted -= ButtonHandler.HandleButtonPress;

        client.GuildMemberUpdated -= ApplicationDatabase.Instance.PlayerData.HandleRegisteredMemberUpdated;
        client.UserLeft -= UserManager.HandleUserLeaveDelegate;

        SetupListeners();
    }

    private void SetupListeners()
    {
        var client = BotReference.GetClientRef();

        client.UserJoined += UserManager.HandleUserJoin;
        client.ButtonExecuted += ButtonHandler.HandleButtonPress;

        // If a member's nickname changes
        client.GuildMemberUpdated += ApplicationDatabase.Instance.PlayerData.HandleRegisteredMemberUpdated;
        client.UserLeft += UserManager.HandleUserLeaveDelegate;
    }
}