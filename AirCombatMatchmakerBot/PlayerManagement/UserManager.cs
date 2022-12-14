using Discord;
using Discord.WebSocket;
using System.Numerics;

public static class UserManager
{
    // For the new users that join the discord, need to add them to the cache too
    public static async Task HandleUserJoin(SocketGuildUser _user)
    {
        Log.WriteLine("User " + _user.Username +
            " has joined the discord with id: " + _user.Id, LogLevel.VERBOSE);

        // Check if the user is already in the database
        if (!Database.Instance.PlayerData.CheckIfUserHasPlayerProfile(_user.Id))
        {
            Log.WriteLine("User is not in the PlayerID's list," +
                " disregarding any further action", LogLevel.VERBOSE);
            return;
        }

        Log.WriteLine("User: " + _user.Username + " (" + _user.Id + ")" +
            " joined with previous profile, adding him to the cache.", LogLevel.DEBUG);

        Database.Instance.CachedUsers.AddUserIdToCachedList(_user.Id);

        await RoleManager.GrantUserAccess(_user.Id, "Member");

        Log.WriteLine("Adding " + _user.Id + " to the cache done.", LogLevel.VERBOSE);

        await SerializationManager.SerializeDB();
    }

    public static async Task HandleUserLeaveDelegate(SocketGuild _guild, SocketUser _user)
    {
        await HandleUserLeave(_user.Username, _user.Id);
        await SerializationManager.SerializeDB();
    }

    public static Task HandleUserLeave(
        string _userName, // Just for printing purposes right now 
        ulong _userId)
    {
        Log.WriteLine(_userName + " (" + _userId +
            ") bailed out! Handling deleting registration channels etc.", LogLevel.DEBUG);

        Database.Instance.Leagues.HandleSettingTeamsInactiveThatUserWasIn(_userId);

        Database.Instance.CachedUsers.RemoveUserFromTheCachedList(_userName, _userId);

        return Task.CompletedTask;
    }


    public static async void HandleQuitUsersDuringDowntimeFromIdList(List<ulong> _userIds)
    {
        // Maybe make own log level for this
        Log.WriteLine("Handling " + _userIds.Count +
            " that left during the downtime", LogLevel.DEBUG);

        foreach (ulong userId in _userIds)
        {
            Log.WriteLine("On userId:" + userId, LogLevel.VERBOSE);

            await HandleUserLeave(
               "during downtime: userId: " + userId.ToString(), userId);
        }

        await SerializationManager.SerializeDB();
    }

    // Need to make this support teams
    public static async void SetTeamActiveAndGrantThePlayerRole(
        InterfaceLeague _interfaceLeague, ulong _playerId)
    {
       _interfaceLeague.LeagueData.Teams.ReturnTeamThatThePlayerIsIn(
           _interfaceLeague.LeaguePlayerCountPerTeam, _playerId).TeamActive = true;
        await RoleManager.GrantUserAccessWithId(
            _playerId, _interfaceLeague.DiscordLeagueReferences.LeagueRoleId);
    }
}