using Discord.WebSocket;

public static class DowntimeManager
{
    public static bool useWaitingChannels = true;
    public static async Task CheckForUsersThatAreNotRegisteredAfterDowntime()
    {
        List<SocketGuildUser> foundUsers = new List<SocketGuildUser>();

        Log.WriteLine("Checking for Users that entered the discord during " +
            "the bot's downtime and that are not on the registeration list", LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild != null)
        {
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
        else Exceptions.BotGuildRefNull();

        Log.WriteLine("Starting to go through foundUsers: " + foundUsers.Count, LogLevel.DEBUG);

        Log.WriteLine("Done checking", LogLevel.DEBUG);

        foreach (SocketGuildUser user in foundUsers)
        {
            Log.WriteLine(user.Username + "(" + user.Id + ")" + "was not found!" +
                " handling user join during downtime.", LogLevel.DEBUG);
            await UserManager.HandleUserJoin(user);
        }

        useWaitingChannels = false;

        await ChannelManager.CreateChannelsFromWaitingChannels();
    }

    public static Task CheckForUsersThatLeftDuringDowntime()
    {
        Log.WriteLine("Starting to check for users that left during the downtime.", LogLevel.DEBUG);

        List<ulong> usersOnTheServerAfterDowntime = new List<ulong>();

        var guild = BotReference.GetGuildRef();

        if (guild != null)
        {
            foreach (SocketGuildUser user in guild.Users)
            {
                if (user != null)
                {
                    string userNameWithId = user.Username + " (" + user.Id + ")";

                    Log.WriteLine("Looping on: " + userNameWithId, LogLevel.VERBOSE);

                    if (!user.IsBot)
                    {
                        Log.WriteLine("Found user: " + userNameWithId +
                            " adding it to list.", LogLevel.VERBOSE);

                        usersOnTheServerAfterDowntime.Add(user.Id);

                        Log.WriteLine("The list count is now: " +
                            usersOnTheServerAfterDowntime.Count, LogLevel.VERBOSE);
                    }
                    else Log.WriteLine("User " + user.Username + " was a bot, diregarding", LogLevel.VERBOSE);
                }
                else { Log.WriteLine("User was null!", LogLevel.CRITICAL); }
            }
        }
        Log.WriteLine("Looping done with: " + usersOnTheServerAfterDowntime.Count +
            " users on the temp list", LogLevel.DEBUG);

        var difference = GetDifferenceBetweenTheCurrentAndCachedUsers(
                Database.Instance.cachedUserIDs, usersOnTheServerAfterDowntime);

        if (difference.Count > 0)
        {
            HandleQuitUsersDuringDowntimeFromIdList(difference);
        }

        return Task.CompletedTask;
    }

    private static List<ulong> GetDifferenceBetweenTheCurrentAndCachedUsers(
        List<ulong> _currentUsers, List<ulong> _databaseUsers)
    {
        PrintUlongListOfUsers(
            "Printing a list of CURRENT user's ID's count of: " , _currentUsers, LogLevel.VERBOSE);

        PrintUlongListOfUsers(
            "Printing a list of DATABASE user's ID's count of: ", _databaseUsers, LogLevel.VERBOSE);
        List<ulong> difference = _currentUsers.Except(_databaseUsers).ToList();

        if (difference.Count > 0)
        {
            PrintUlongListOfUsers("Here's the difference results: ", difference, LogLevel.DEBUG);
        }
        else Log.WriteLine("No difference detected.", LogLevel.VERBOSE);

        return difference;
    }

    private static void PrintUlongListOfUsers(
        string _printMessage, List<ulong> _users, LogLevel _logLevel)
    {
        Log.WriteLine(_printMessage + _users.Count, _logLevel);
        foreach (ulong userId in _users) { Log.WriteLine("UserId: " + userId, _logLevel); }
        Log.WriteLine("End of print.", _logLevel);
    }

    private static async void HandleQuitUsersDuringDowntimeFromIdList(List<ulong> _userIds)
    {
        // Maybe make own log level for this
        Log.WriteLine("Handling " + _userIds.Count +
            " that left during the downtime", LogLevel.WARNING);

        foreach (ulong userId in _userIds)
        {
            Log.WriteLine("On userId:" + userId, LogLevel.VERBOSE);
            if (BotReference.clientRef != null)
            {
                //var user = BotReference.clientRef.GetUserAsync(userId).Result;
                await UserManager.HandleUserLeave(
                   "during downtime: userId: " + userId.ToString(), userId);
            }
        }

        await SerializationManager.SerializeDB();
    }
}