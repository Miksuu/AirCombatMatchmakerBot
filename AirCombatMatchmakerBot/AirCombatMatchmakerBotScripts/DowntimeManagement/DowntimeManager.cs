using Discord.WebSocket;
using System.Collections.Concurrent;
using System.Collections.Generic;

public static class DowntimeManager
{
    public static async Task CheckForUsersThatJoinedAfterDowntime()
    {
        Log.WriteLine("Checking for Users that entered the discord during " +
            "the bot's downtime and that are not on the registration List");

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
                Log.WriteLine("User is null!", LogLevel.ERROR);
                return;
            }

            if (user.IsBot)
            {
                Log.WriteLine("User " + user.Username +
                    " is a bot, disregarding");
            }
            else
            {
                if (!Database.GetInstance<ApplicationDatabase>().PlayerData.CheckIfUserHasPlayerProfile(user.Id))
                {
                    Log.WriteLine(
                        user.Username + "(" + user.Id +
                        ") was not found, disregarding");
                }
                // Run handle user join that will server the same purpose than the
                // new player joining when the bot is up (if he was registered)
                else
                    Log.WriteLine(user.Username + " (" + user.Id + ") " + "was not found!" +
                        " adding user to the List!", LogLevel.DEBUG);
                foundUsers.Add(user);
            }
        }

        Log.WriteLine("Starting to go through foundUsers: " + foundUsers.Count);

        foreach (SocketGuildUser user in foundUsers)
        {
            Log.WriteLine(user.Username + "(" + user.Id + ")" + "was not found!" +
                " handling user join during downtime.", LogLevel.DEBUG);
            await UserManager.HandleUserJoin(user);
        }

        //await SerializationManager.SerializeDB();
    }

    public static Task CheckForUsersThatLeftDuringDowntime()
    {
        Log.WriteLine(
            "Starting to check for users that left during the downtime.");

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
                Log.WriteLine("User was null!", LogLevel.ERROR);
                return Task.CompletedTask;
            }

            string userNameWithId = user.Username + " (" + user.Id + ")";

            Log.WriteLine("Looping on: " + userNameWithId);

            if (!user.IsBot)
            {
                Log.WriteLine("Found user: " + userNameWithId +
                    " adding it to List.");

                usersOnTheServerAfterDowntime.Add(user.Id);

                Log.WriteLine("The List count is now: " +
                    usersOnTheServerAfterDowntime.Count);
            }
            else Log.WriteLine("User " + user.Username +
                " was a bot, diregarding");
        }

        Log.WriteLine("Looping done with: " + usersOnTheServerAfterDowntime.Count +
            " users on the temp List", LogLevel.DEBUG);

        var difference = GetDifferenceBetweenTheCurrentAndCachedUsers(
                Database.GetInstance<ApplicationDatabase>().CachedUsers.CachedUserIDs.ToList(),
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
            "Printing a List of CURRENT user's ID's count of: ",
            _currentUsers, LogLevel.VERBOSE);

        PrintUlongListOfUsers(
            "Printing a List of DATABASE user's ID's count of: ",
            _databaseUsers, LogLevel.VERBOSE);
        //ConcurrentBag<ulong> difference = _currentUsers.Except(_databaseUsers);

        List<ulong> difference = new List<ulong>(_currentUsers.Where(item => !_databaseUsers.Contains(item)));

        if (difference.Count > 0)
        {
            PrintUlongListOfUsers("Here's the difference results: ", difference, LogLevel.DEBUG);
            Log.WriteLine("test", LogLevel.DEBUG);

        }
        else Log.WriteLine("No difference detected.");

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