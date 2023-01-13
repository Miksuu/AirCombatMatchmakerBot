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
            if (!BotReference.GetConnectionState())
            {
                BotReference.SetConnectionState(true);
                Log.WriteLine("Bot is connected!", LogLevel.DEBUG);

                //ONLY FOR TESTING, DELETES ALL CHANNELS AND CATEGORIES

                /*
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
                }*/

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
                client.GuildMemberUpdated += Database.Instance.PlayerData.HandleRegisteredMemberUpdated;
                client.UserLeft += UserManager.HandleUserLeaveDelegate;

                //client.ChannelCreated += ChannelManager.FinishChannelCreationFromDelegate;

                await DowntimeManager.CheckForUsersThatJoinedAfterDowntime();
            }
            else
            {
                Log.WriteLine("Bot was already connected!", LogLevel.WARNING);
            }

        };
        
        // Receiving the tacview files
        client.MessageReceived += async (_socketMessage) =>
        {
            /*
            Log.WriteLine("Looking for: " + _socketMessage.Channel.Id, LogLevel.DEBUG);
            foreach (var item in CategoryAndChannelManager.matchChannelsIdWithCategoryId)
            {
                Log.WriteLine(item.Key + " | " + item.Value, LogLevel.DEBUG);
            }*/

            // Disregards any message that's not inside the bot's match channels
            if (!Database.Instance.Categories.MatchChannelsIdWithCategoryId.ContainsKey(
                _socketMessage.Channel.Id))
            {
                return;
            }

            //Log.WriteLine("Found: " + _socketMessage.Channel.Id, LogLevel.DEBUG);

            // Checks for any attachments
            if (!_socketMessage.Attachments.Any())
            {
                return;
            }

            Log.WriteLine("Message: " + _socketMessage.Id + " + detected in: " +
                _socketMessage.Channel.Id + " by: " + _socketMessage.Author.Id, LogLevel.VERBOSE);

            //var socketMessageComponent = _socketMessage.ToComponent();

            // Check if the message contains a file and only one file
            if (_socketMessage.Attachments.Count != 1)
            {
                Log.WriteLine("Message: " + _socketMessage.Id +
                    " contained more than 1 attachment!", LogLevel.VERBOSE);

                _socketMessage.Channel.SendMessageAsync(
                    _socketMessage.Author.Mention + ", make sure only include one attachment in the message," +
                    " with the .acmi file of the match!");

                _socketMessage.DeleteAsync();

                return;
            }

            var attachment = _socketMessage.Attachments.FirstOrDefault();
            if (attachment == null)
            {
                Log.WriteLine(nameof(attachment) + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Found attachment: " + attachment.Filename, LogLevel.VERBOSE);

            if (!attachment.Filename.EndsWith(".acmi"))
            {
                Log.WriteLine(_socketMessage.Author.Id +
                    " tried to send a file that is not a .acmi file!" +
                    " URL:" + attachment.Url, LogLevel.WARNING);

                await _socketMessage.Channel.SendMessageAsync(
                          _socketMessage.Author.Mention +
                          ", make sure the attachment you are sending is in .acmi format!");

                await _socketMessage.DeleteAsync();

                return;
            }

            // Find the league with the cached category ID
            InterfaceLeague? interfaceLeague =
                Database.Instance.Leagues.GetILeagueByCategoryId(
                    Database.Instance.Categories.MatchChannelsIdWithCategoryId[_socketMessage.Channel.Id]);
            if (interfaceLeague == null)
            {
                Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.CRITICAL);
                return;
            }

            LeagueMatch? foundMatch =
                interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(
                    _socketMessage.Channel.Id);
            if (foundMatch == null)
            {
                Log.WriteLine("Match with: " + _socketMessage.Channel.Id +
                    " was not found.", LogLevel.CRITICAL);
                return;
            }

            await foundMatch.MatchReporting.ProcessTacviewSentByTheUser(
                     interfaceLeague, _socketMessage, attachment.Url);

            await SerializationManager.SerializeDB();

            return;
        };

        // Listens for the commandService
        await CommandHandler.InstallCommandsAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
}