using Discord;
using Discord.WebSocket;

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
            if (!BotReference.GetConnectionState())
            {
                BotReference.SetConnectionState(true);
                Log.WriteLine("Bot is connected!", LogLevel.DEBUG);

                // !!!
                // ONLY FOR TESTING, DELETES ALL CHANNELS AND CATEGORIES
                // !!!

                
                var guild = BotReference.GetGuildRef();
                foreach (var cat in guild.CategoryChannels)
                {
                    if (cat.Name.ToLower().Contains("main-category")) continue;

                    Log.WriteLine("deleting category: " + cat.Name, LogLevel.DEBUG);
                    await cat.DeleteAsync();
                }

                foreach (SocketGuildChannel item in guild.Channels)
                {
                    Log.WriteLine(item.Name, LogLevel.DEBUG);

                    if (item.Name == "info" || item.Name == "test" || item.Name == "main-category")
                    {
                        continue;
                    }
                    await item.DeleteAsync();
                }

                // Delete the old tacviews so it doesn't throw error from old files
                string pathToDelete = @"C:\AirCombatMatchmakerBot\Data\Tacviews";
                if (Directory.Exists(pathToDelete)) Directory.Delete(pathToDelete, true);

                // Delete db here
                await SerializationManager.HandleDatabaseCreationOrLoading("0");

                // Delete roles here
                foreach (var item in guild.Roles)
                {
                    Log.WriteLine("on role: " + item.Name, LogLevel.VERBOSE);

                    if (item.Name == "Developer" || item.Name == "Server Booster" || 
                        item.Name == "AirCombatMatchmakerBotDev" || item.Name == "Discord Me" ||
                        item.Name == "@everyone" || item.Name == "@here")
                    {
                        continue;
                    }

                    Log.WriteLine("Deleting role: " + item.Name, LogLevel.DEBUG);

                    await item.DeleteAsync();
                }

                // !!!
                // ONLY FOR TESTING, DELETES ALL CHANNELS AND CATEGORIES
                // !!!

                /*
                foreach (var item in guild.Emotes)
                {
                    Log.WriteLine("Emoji: " + item.Name + " id: " + item.Id, LogLevel.DEBUG);
                }*/

                // Creates the league references to the database
                // (must be run before creating the channels)
                await LeagueManager.CreateLeaguesOnStartupIfNecessary();

                // Creates the categories and the channels from the interfaces
                // (dependant on the data from CreateLeaguesOnStartupIfNecessary())
                await CategoryAndChannelManager.CreateCategoriesAndChannelsForTheDiscordServer();

                // Checks the users that left during down time and sets their teams active
                await DowntimeManager.CheckForUsersThatLeftDuringDowntime();

                client.UserJoined += UserManager.HandleUserJoin;
                client.ButtonExecuted += ButtonHandler.HandleButtonPress;
                //client.MessageUpdated += UserManager.MessageUpdated;

                // If a member's nickname changes
                client.GuildMemberUpdated += Database.Instance.PlayerData.HandleRegisteredMemberUpdated;
                client.UserLeft += UserManager.HandleUserLeaveDelegate;

                //client.ChannelCreated += ChannelManager.FinishChannelCreationFromDelegate;

                await DowntimeManager.CheckForUsersThatJoinedAfterDowntime();

                await SerializationManager.SerializeUsersOnTheServer();
                await SerializationManager.SerializeDB();

                await CommandHandler.InstallCommandsAsync();
            }
            else
            {
                Log.WriteLine("Bot was already connected!", LogLevel.WARNING);

                client.UserJoined -= UserManager.HandleUserJoin;
                client.ButtonExecuted -= ButtonHandler.HandleButtonPress;
                //client.MessageUpdated += UserManager.MessageUpdated;

                // If a member's nickname changes
                client.GuildMemberUpdated -= Database.Instance.PlayerData.HandleRegisteredMemberUpdated;
                client.UserLeft -= UserManager.HandleUserLeaveDelegate;


                client.UserJoined += UserManager.HandleUserJoin;
                client.ButtonExecuted += ButtonHandler.HandleButtonPress;
                //client.MessageUpdated += UserManager.MessageUpdated;

                // If a member's nickname changes
                client.GuildMemberUpdated += Database.Instance.PlayerData.HandleRegisteredMemberUpdated;
                client.UserLeft += UserManager.HandleUserLeaveDelegate;
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
}