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

    public static async Task HandleUserLeave(
        string _userName, // Just for printing purposes right now 
        ulong _userId)
    {
        Log.WriteLine(_userName + " (" + _userId +
            ") bailed out! Handling deleting registration channels etc.", LogLevel.DEBUG);

        Database.Instance.Leagues.HandleSettingTeamsInactiveThatUserWasIn(_userId);

        Database.Instance.CachedUsers.RemoveUserFromTheCachedList(_userName, _userId);
    }

    // Move to PlayerData
    public static async Task<bool> AddNewPlayerToTheDatabaseById(ulong _playerId)
    {
        Log.WriteLine("Start of the addnewplayer with: " + _playerId, LogLevel.VERBOSE);

        var nickName = CheckIfNickNameIsEmptyAndReturnUsername(_playerId);

        Log.WriteLine("Adding a new player: " + nickName + " (" + _playerId + ").", LogLevel.DEBUG);

        // Checks if the player is already in the database, just in case
        if (!Database.Instance.PlayerData.CheckIfUserHasPlayerProfile(_playerId))
        {
            Log.WriteLine("Player doesn't exist in the database: " + _playerId, LogLevel.VERBOSE);

            // Add to the profile
            Database.Instance.PlayerData.AddAPlayerProfile(new Player(_playerId, nickName));

            // Add the member role for access.
            await RoleManager.GrantUserAccess(_playerId, "Member");

            return true;
        }
        else
        {
            Log.WriteLine("Tried to add a player that was already in the database: " +
                _playerId, LogLevel.DEBUG);
            return false;
        }
    }

    public static async Task HandleGuildMemberUpdated(
        Cacheable<SocketGuildUser, ulong> before, SocketGuildUser _socketGuildUserAfter)
    {
        var playerValue = 
            Database.Instance.PlayerData.GetAPlayerProfileById(_socketGuildUserAfter.Id);

        if (playerValue == null)
        {
            Log.WriteLine("Trying to update " + _socketGuildUserAfter.Username +
            "'s profile, no valid player found (not registered?) ", LogLevel.DEBUG);
            return;
        }

        // This should not be empty, since it's being looked up from the database
        string playerValueNickName = playerValue.GetPlayerNickname();

        string socketGuildUserAfterNickName =
            CheckIfNickNameIsEmptyAndReturnUsername(_socketGuildUserAfter.Id);

        Log.WriteLine("Updating user: " + _socketGuildUserAfter.Username + " (" 
            + _socketGuildUserAfter.Id + ")" + " | name: " + playerValueNickName +
            " -> " + socketGuildUserAfterNickName, LogLevel.DEBUG);

        playerValue.SetPlayerNickname(socketGuildUserAfterNickName);
        await SerializationManager.SerializeDB();
    }

    public static string CheckIfNickNameIsEmptyAndReturnUsername(ulong _id)
    {
        Log.WriteLine("Checking if nickname is empty and return username with ID: " +
            _id, LogLevel.VERBOSE);

        var SocketGuildUser = GetSocketGuildUserById(_id);

        if (SocketGuildUser == null)
        {
            Log.WriteLine("SocketGuildUser by ID: " + _id + " is null!", LogLevel.ERROR);
            return "null";
        }

        Log.WriteLine("SocketGuildUser " + _id + " is not null", LogLevel.VERBOSE);

        string userName = SocketGuildUser.Username;
        string nickName = SocketGuildUser.Nickname;

        Log.WriteLine("Checking if " + userName + "'s (" + _id + ")" +
            " nickName: " + nickName + " | " + " is the same", LogLevel.VERBOSE);

        if (nickName == "" || nickName == userName || nickName == null)
        {
            Log.WriteLine("returning userName: " + userName, LogLevel.VERBOSE);
            return userName;
        }
        else
        {
            Log.WriteLine("returning nickName " + nickName, LogLevel.VERBOSE);
            return nickName;
        }
    }

    // Gets the user by the discord UserId. This may not be present in the Database.
    public static SocketGuildUser? GetSocketGuildUserById(ulong _id)
    {
        Log.WriteLine("Getting SocketGuildUser by id: " + _id, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotClientRefNull();
            return null;
        }

        return guild.GetUser(_id);
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

    public static async void SetPlayerActiveAndGrantHimTheRole(
        ILeague _dbLeagueInstance, ulong _playerId)
    {
       _dbLeagueInstance.LeagueData.Teams.ReturnTeamThatThePlayerIsIn(_playerId).SetTheActive(true);
        await RoleManager.GrantUserAccessWithId(
            _playerId, _dbLeagueInstance.DiscordLeagueReferences.LeagueRoleId);
    }
}