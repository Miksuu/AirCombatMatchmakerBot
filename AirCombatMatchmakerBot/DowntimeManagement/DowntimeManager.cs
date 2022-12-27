using Discord.WebSocket;

public static class DowntimeManager
{
    //public static bool useWaitingChannels = true;
    public static async Task CheckForUsersThatJoinedAfterDowntime()
    {
        Log.WriteLine("Checking for Users that entered the discord during " +
            "the bot's downtime and that are not on the registration list", LogLevel.VERBOSE);

        List<SocketGuildUser> foundUsers = new List<SocketGuildUser>();

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        // Loop through the users
        foreach (var user in guild.Users)
        {
            if (user == null)
            {
                Log.WriteLine("User is null!", LogLevel.CRITICAL);
                return;
            }

            if (user.IsBot)
            {
                Log.WriteLine("User " + user.Username +
                    " is a bot, disregarding", LogLevel.VERBOSE);
            }
            else
            {
                if (!UserManager.CheckIfUserHasPlayerProfile(user.Id))
                {
                    Log.WriteLine(
                        user.Username + "(" + user.Id +
                        ") was not found, disregarding", LogLevel.VERBOSE);
                }
                // Run handle user join that will server the same purpose than the
                // new player joining when the bot is up (if he was registered)
                else
                    Log.WriteLine(user.Username + " (" + user.Id + ") " + "was not found!" +
                        " adding user to the list!", LogLevel.DEBUG);
                foundUsers.Add(user);
            }
        }

        Log.WriteLine("Starting to go through foundUsers: " + foundUsers.Count, LogLevel.VERBOSE);

        foreach (SocketGuildUser user in foundUsers)
        {
            Log.WriteLine(user.Username + "(" + user.Id + ")" + "was not found!" +
                " handling user join during downtime.", LogLevel.DEBUG);
            await UserManager.HandleUserJoin(user);
        }

        await SerializationManager.SerializeDB();
    }

    public static Task CheckForUsersThatLeftDuringDowntime()
    {
        Log.WriteLine(
            "Starting to check for users that left during the downtime.", LogLevel.VERBOSE);

        List<ulong> usersOnTheServerAfterDowntime = new List<ulong>();

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return Task.CompletedTask;
        }

        foreach (SocketGuildUser user in guild.Users)
        {
            if (user == null)
            {
                Log.WriteLine("User was null!", LogLevel.CRITICAL);
                return Task.CompletedTask;
            }

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
            else Log.WriteLine("User " + user.Username +
                " was a bot, diregarding", LogLevel.VERBOSE);
        }

        Log.WriteLine("Looping done with: " + usersOnTheServerAfterDowntime.Count +
            " users on the temp list", LogLevel.DEBUG);

        var difference = GetDifferenceBetweenTheCurrentAndCachedUsers(
                Database.Instance.CachedUsers.GetListOfCachedUserIds(),
                usersOnTheServerAfterDowntime);

        if (difference.Count > 0)
        {
            UserManager.HandleQuitUsersDuringDowntimeFromIdList(difference);
        }

        Log.WriteLine("Done checking for users that left during the downtime.", LogLevel.DEBUG);

        return Task.CompletedTask;
    }

    private static List<ulong> GetDifferenceBetweenTheCurrentAndCachedUsers(
        List<ulong> _currentUsers, List<ulong> _databaseUsers)
    {
        PrintUlongListOfUsers(
            "Printing a list of CURRENT user's ID's count of: ",
            _currentUsers, LogLevel.VERBOSE);

        PrintUlongListOfUsers(
            "Printing a list of DATABASE user's ID's count of: ",
            _databaseUsers, LogLevel.VERBOSE);
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
}