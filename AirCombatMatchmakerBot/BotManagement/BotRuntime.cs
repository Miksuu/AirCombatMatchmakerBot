using Discord.WebSocket;
using Discord;

// The main class to what the bot's functions revolve around
public class BotRuntimeManager
{
    public async Task BotRuntimeTask()
    {
        LogLevelNormalization.InitLogLevelNormalizationStrings();
        // Do not use the logging system before this !!!

        // Load the data from the file
        await SerializationManager.DeSerializeDB();

        // Set up client and return it
        var client = BotReference.SetClientRefAndReturnIt();

        // Reads token from the same directory as the .exe
        var token = File.ReadAllText("token.txt");
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        client.Ready += async () =>
        {
            BotReference.SetConnectionState(true);
            Log.WriteLine("Bot is connected!", LogLevel.DEBUG);

            //ONLY FOR TESTING, DELETES ALL CHANNELS AND CATEGORIES
            
            
            
            var guild = BotReference.GetGuildRef();
            foreach (var ch in guild.Channels)
            {
                if (ch.Name == "info") continue;

                Log.WriteLine("deleting " + ch.Name, LogLevel.DEBUG);
                await ch.DeleteAsync();
            }
            foreach (var cat in guild.CategoryChannels)
            {
                Log.WriteLine("deleting category: " + cat.Name, LogLevel.DEBUG);
                await cat.DeleteAsync();
            }

            // Creates the league references to the database
            // (must be run before creating the channels)
            await LeagueManager.CreateLeaguesOnStartupIfNecessary();

            // Creates the categories and the channels from the interfaces
            // (dependant on the data from CreateLeaguesOnStartupIfNecessary())
            await CategoryAndChannelManager.CreateCategoriesAndChannelsForTheDiscordServer();

            // Checks the users that left during down time and sets their teams active
            await DowntimeManager.CheckForUsersThatLeftDuringDowntime();
            await SerializationManager.SerializeUsersOnTheServer();

            client.UserJoined += UserManager.HandleUserJoin;
            client.ButtonExecuted += ButtonHandler.HandleButtonPress;
            //client.MessageUpdated += UserManager.MessageUpdated;

            // If a member's nickname changes
            client.GuildMemberUpdated += Database.Instance.PlayerData.HandleGuildMemberUpdated;
            client.UserLeft += UserManager.HandleUserLeaveDelegate;

            //client.ChannelCreated += ChannelManager.FinishChannelCreationFromDelegate;

            await DowntimeManager.CheckForUsersThatJoinedAfterDowntime();
        };

        // Listens for the commandService
        await CommandHandler.InstallCommandsAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
}