using Discord.WebSocket;

public static class DowntimeManager
{
    public static bool useWaitingChannels = true;
    public static async Task CheckForUsersThatAreNotRegisteredAfterDowntime()
    {
        List<SocketGuildUser> foundUsers = new List<SocketGuildUser>();

        Log.WriteLine("Checking for Users that entered the discord during " +
            "the bot's downtime and that are not on the registeration list", LogLevel.DEBUG);

        if (BotReference.clientRef != null)
        {
            var guild = BotReference.clientRef.GetGuild(BotReference.GuildID);
            Log.WriteLine("USERS COUNT: " + guild.Users.Count(), LogLevel.DEBUG);

            // Loop through the users
            foreach (var user in guild.Users)
            {
                if (user != null)
                {
                    if (!user.IsBot)
                    {
                        // Profile found, disregard
                        if (DatabaseMethods.CheckIfUserHasANonRegisterdUserProfile(user.Id) ||
                            DatabaseMethods.CheckIfUserIdExistsInTheDatabase(user.Id))
                        {
                            Log.WriteLine(
                                user.Username + "(" + user.Id + ") was found, disregarding", LogLevel.VERBOSE);
                        }
                        // Run handle user join that will server the same purpose than the new player joining
                        // when the bot is up
                        else
                        {
                            Log.WriteLine(user.Username + "(" + user.Id + ")" + "was not found!" +
                                " adding user to the list!", LogLevel.VERBOSE);
                            foundUsers.Add(user);
                        }
                    }
                    else Log.WriteLine("User " + user.Username + " is a bot, disregarding", LogLevel.VERBOSE);
                }
                else Log.WriteLine("User is null!", LogLevel.CRITICAL);
            }
        }
        else Exceptions.BotClientRefNull();

        Log.WriteLine("Starting to go through foundUsers: " + foundUsers.Count, LogLevel.DEBUG);

        Log.WriteLine("Done checking", LogLevel.DEBUG);

        foreach (SocketGuildUser user in foundUsers)
        {
            Log.WriteLine(user.Username + "(" + user.Id + ")" + "was not found!" +
                " handling user join during downtime.", LogLevel.DEBUG);
            await PlayerManager.HandleUserJoin(user);
        }

        useWaitingChannels = false;

        await ChannelManager.CreateChannelsFromWaitingChannels();
    }

    public static async Task CheckForUsersThatLeftDuringDowntime()
    {
        await SerializationManager.SerializeDB();
    }
}