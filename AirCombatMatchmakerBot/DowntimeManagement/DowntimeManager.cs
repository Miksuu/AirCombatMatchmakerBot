using Discord.WebSocket;
using System.Collections.Concurrent;
using System.Collections.Generic;

public static class DowntimeManager
{
    public static async Task CheckForUsersThatJoinedAfterDowntime()
    {
        Log.WriteLine("Checking for Users that entered the discord during " +
            "the bot's downtime and that are not on the registration ConcurrentBag", LogLevel.VERBOSE);

        ConcurrentBag<SocketGuildUser> foundUsers = new ConcurrentBag<SocketGuildUser>();

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
                if (!Database.Instance.PlayerData.CheckIfUserHasPlayerProfile(user.Id))
                {
                    Log.WriteLine(
                        user.Username + "(" + user.Id +
                        ") was not found, disregarding", LogLevel.VERBOSE);
                }
                // Run handle user join that will server the same purpose than the
                // new player joining when the bot is up (if he was registered)
                else
                    Log.WriteLine(user.Username + " (" + user.Id + ") " + "was not found!" +
                        " adding user to the ConcurrentBag!", LogLevel.DEBUG);
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

        ConcurrentBag<ulong> usersOnTheServerAfterDowntime = new ConcurrentBag<ulong>();

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
                    " adding it to ConcurrentBag.", LogLevel.VERBOSE);

                usersOnTheServerAfterDowntime.Add(user.Id);

                Log.WriteLine("The ConcurrentBag count is now: " +
                    usersOnTheServerAfterDowntime.Count, LogLevel.VERBOSE);
            }
            else Log.WriteLine("User " + user.Username +
                " was a bot, diregarding", LogLevel.VERBOSE);
        }

        Log.WriteLine("Looping done with: " + usersOnTheServerAfterDowntime.Count +
            " users on the temp ConcurrentBag", LogLevel.DEBUG);

        var difference = GetDifferenceBetweenTheCurrentAndCachedUsers(
                Database.Instance.CachedUsers.CachedUserIDs,
                usersOnTheServerAfterDowntime);

        if (difference.Count > 0)
        {
            UserManager.HandleQuitUsersDuringDowntimeFromIdConcurrentBag(difference);
        }

        Log.WriteLine("Done checking for users that left during the downtime.", LogLevel.DEBUG);

        return Task.CompletedTask;
    }

    private static ConcurrentBag<ulong> GetDifferenceBetweenTheCurrentAndCachedUsers(
        ConcurrentBag<ulong> _currentUsers, ConcurrentBag<ulong> _databaseUsers)
    {
        PrintUlongConcurrentBagOfUsers(
            "Printing a ConcurrentBag of CURRENT user's ID's count of: ",
            _currentUsers, LogLevel.VERBOSE);

        PrintUlongConcurrentBagOfUsers(
            "Printing a ConcurrentBag of DATABASE user's ID's count of: ",
            _databaseUsers, LogLevel.VERBOSE);
        //ConcurrentBag<ulong> difference = _currentUsers.Except(_databaseUsers);

        ConcurrentBag<ulong> difference = new ConcurrentBag<ulong>(_currentUsers.Where(item => !_databaseUsers.Contains(item)));

        if (difference.Count > 0)
        {
            PrintUlongConcurrentBagOfUsers("Here's the difference results: ", difference, LogLevel.DEBUG);
            Log.WriteLine("test", LogLevel.DEBUG);

        }
        else Log.WriteLine("No difference detected.", LogLevel.VERBOSE);

        return difference;
    }

    private static void PrintUlongConcurrentBagOfUsers(
        string _printMessage, ConcurrentBag<ulong> _users, LogLevel _logLevel)
    {
        Log.WriteLine(_printMessage + _users.Count, _logLevel);
        foreach (ulong userId in _users) { Log.WriteLine("UserId: " + userId, _logLevel); }
        Log.WriteLine("End of print.", _logLevel);
    }
}