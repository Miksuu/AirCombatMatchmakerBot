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

        //await CreateARegisterationProfileForTheUser(_user, userNameWithNickName);
        //await AddUserToCache(userNameWithNickName, _user.Id);

        // Check if the user is already in the database
        if (!CheckIfUserHasPlayerProfile(_user.Id))
        {
            Log.WriteLine("User is not in the PlayerID's list," +
                " disregarding any further action", LogLevel.VERBOSE);
            return;
        }

        Log.WriteLine("User: " + _user.Username + " (" + _user.Id + ")" +
            " joined with previous profile, adding him to the cache.", LogLevel.DEBUG);

        await HandleUserRegisterationToCache(_user.Id);

        await RoleManager.GrantUserAccess(_user.Id, "Member");

        Log.WriteLine("Adding " + _user.Id + " to the cache done.", LogLevel.VERBOSE);
    }

    public static bool CheckIfUserHasPlayerProfile(ulong _userId)
    {
        return Database.Instance.PlayerData.PlayerIDs.Any(x => x.Key == _userId);
    }

    // For the new users that join the discord, need to add them to the cache too
    public static async Task HandleUserRegisterationToCache(ulong _userId)
    {
        await AddUserToCache(_userId);
    }

    // Add the user to the cached users list,
    // this doesn't happen to the terminated users as they are already in the server
    private static async Task AddUserToCache(ulong _userId)
    {
        SerializationManager.AddUserIdToCachedList(_userId);
        await SerializationManager.SerializeDB();
    }

    public static async Task HandleUserLeaveDelegate(SocketGuild _guild, SocketUser _user)
    {
        await HandleUserLeave(_user.Username, _user.Id);
    }

    public static async Task HandleUserLeave(
        string _userName, // Just for printing purposes right now 
        ulong _userId)
    {
        Log.WriteLine(_userName + " (" + _userId +
            ") bailed out! Handling deleting registration channels etc.", LogLevel.DEBUG);

        await HandleSettingTeamsInactiveThatUserWasIn(_userId);

        SerializationManager.RemoveUserFromTheCachedList(_userName, _userId);
    }

    private static async Task HandleSettingTeamsInactiveThatUserWasIn(ulong _userId)
    {
        Log.WriteLine("Starting to set teams inactive that " + _userId + " was in.", LogLevel.VERBOSE);

        foreach (ILeague storedLeague in
            Database.Instance.Leagues.GetListOfStoredLeagues())
        {
            Log.WriteLine("Looping through league: " + storedLeague.LeagueCategoryName, LogLevel.VERBOSE);

            bool teamFound = false;

            if (storedLeague == null)
            {
                Log.WriteLine("storedLeague was null!", LogLevel.CRITICAL);
                continue;
            }

            string? storedLeagueString = storedLeague.ToString();

            foreach (Team team in storedLeague.LeagueData.Teams)
            {
                if (!teamFound)
                {
                    foreach (Player player in team.players)
                    {
                        Log.WriteLine("Looping through player: " + player.playerNickName + " (" +
                            player.playerDiscordId + ")", LogLevel.VERBOSE);
                        if (player.playerDiscordId == _userId)
                        {
                            team.teamActive = false;

                            teamFound = true;
                            Log.WriteLine("Set team: " + team.teamName + " deactive in league: " +
                                storedLeague.LeagueCategoryName + " because " + player.playerNickName +
                                " left", LogLevel.DEBUG);

                            if (storedLeagueString == null)
                            {
                                Log.WriteLine("storedLeagueString was null!", LogLevel.CRITICAL);
                                continue;
                            }

                            var findLeagueCategoryType =
                                Database.Instance.Leagues.GetILeagueByString(storedLeagueString);
                            CategoryName leagueCategoryName = findLeagueCategoryType.LeagueCategoryName;

                            var leagueInterface = 
                                LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueCategoryName);
                            Log.WriteLine("Found " + nameof(leagueInterface) + ": " +
                                leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

                            ILeague? dbLeagueInstance = 
                                LeagueManager.FindLeagueAndReturnInterfaceFromDatabase(leagueInterface);

                            if (dbLeagueInstance == null)
                            {
                                Log.WriteLine("dbLeagueInstance was null!", LogLevel.CRITICAL);
                                continue;
                            }

                            await MessageManager.ModifyLeagueRegisterationChannelMessage(dbLeagueInstance);

                            break;
                        }
                    }
                }
                else
                {
                    Log.WriteLine("The team was already found in the league, breaking and proceeding" +
                        " to the next one.", LogLevel.VERBOSE);
                    break;
                }
            }
        }
    }

    public static async Task<bool> AddNewPlayerToTheDatabaseById(ulong _playerId)
    {
        Log.WriteLine("Start of the addnewplayer with: " + _playerId, LogLevel.VERBOSE);

        var nickName = CheckIfNickNameIsEmptyAndReturnUsername(_playerId);

        Log.WriteLine("Adding a new player: " + nickName + " (" + _playerId + ").", LogLevel.DEBUG);

        // Checks if the player is already in the database, just in case
        if (!UserManager.CheckIfUserHasPlayerProfile(_playerId))
        {
            Log.WriteLine("Player doesn't exist in the database: " + _playerId, LogLevel.VERBOSE);

            // Add to the profile
            Database.Instance.PlayerData.PlayerIDs.Add(_playerId, new Player(_playerId, nickName));

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
        var playerValue = Database.Instance.PlayerData.PlayerIDs.First(
            x => x.Key == _socketGuildUserAfter.Id).Value;

        if (playerValue == null)
        {
            Log.WriteLine("Trying to update " + _socketGuildUserAfter.Username +
            "'s profile, no valid player found (not registered?) ", LogLevel.DEBUG);
            return;
        }

        // This should not be empty, since it's being looked up from the database
        string playerValueNickName = playerValue.playerNickName;

        string socketGuildUserAfterNickName =
            CheckIfNickNameIsEmptyAndReturnUsername(_socketGuildUserAfter.Id);

        Log.WriteLine("Updating user: " + _socketGuildUserAfter.Username + " (" 
            + _socketGuildUserAfter.Id + ")" + " | name: " + playerValueNickName +
            " -> " + socketGuildUserAfterNickName, LogLevel.DEBUG);

        playerValue.playerNickName = socketGuildUserAfterNickName;
        await SerializationManager.SerializeDB();
    }

    public static string CheckIfNickNameIsEmptyAndReturnUsername(ulong _id)
    {
        Log.WriteLine("Checking if nickname is empty and return username with ID: " + _id, LogLevel.VERBOSE);

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

    public static async Task<bool> DeletePlayerProfile(string _dataValue)
    {
        ulong userId = UInt64.Parse(_dataValue);

        Log.WriteLine("Starting to remove the player profile: " + userId, LogLevel.VERBOSE);
        if (!UserManager.CheckIfUserHasPlayerProfile(userId))
        {
            Log.WriteLine("Did not find ID: " + userId + "in the local database.", LogLevel.DEBUG);
            return false;
        }

        Log.WriteLine("Deleting a player profile " + userId, LogLevel.DEBUG);
        Database.Instance.PlayerData.PlayerIDs.Remove(userId);

        var user = GetSocketGuildUserById(userId);

        // If the user is in the server
        if (user == null)
        {
            Log.WriteLine("User with id: " + userId + " was null!!" +
                " Was not found in the server?", LogLevel.DEBUG);
            return false;
        }

        Log.WriteLine("User found in the server", LogLevel.VERBOSE);

        // Remove user's access (back to the registration...)
        await RoleManager.RevokeUserAccess(userId, "Member");

        /*
        // After termination, create a regiteration profile for that player
        string userNameWithNickName = user.Username + " aka "
            + CheckIfNickNameIsEmptyAndReturnUsername(user.Id) +
            " (" + user.Id + ")";
        //await CreateARegisterationProfileForTheUser(user, userNameWithNickName); */

        Log.WriteLine("Done removing the player profile: " + userId, LogLevel.DEBUG);

        return true;
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
        LeagueManager.ReturnTeamThatThePlayerIsIn(
            _dbLeagueInstance.LeagueData.Teams, _playerId).teamActive = true;
        await RoleManager.GrantUserAccessWithId(
            _playerId, _dbLeagueInstance.DiscordLeagueReferences.GetLeagueRoleId());
    }
}