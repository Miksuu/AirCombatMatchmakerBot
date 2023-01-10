using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class PlayerData
{
    [DataMember] private Dictionary<ulong, Player> PlayerIDs { get; set; }
    public PlayerData()
    {
        PlayerIDs = new Dictionary<ulong, Player>();
    }

    public void AddAPlayerProfile(Player _Player)
    {
        Log.WriteLine("Adding a player profile: " + _Player.PlayerNickName + " (" +
            _Player.PlayerDiscordId + ") to the PlayerIDs list", LogLevel.VERBOSE);

        PlayerIDs.Add(_Player.PlayerDiscordId, _Player);
        Log.WriteLine("Done adding, count is now: " + PlayerIDs.Count, LogLevel.VERBOSE);
    }

    public async Task<bool> AddNewPlayerToTheDatabaseById(ulong _playerId)
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

    public string CheckIfNickNameIsEmptyAndReturnUsername(ulong _id)
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
    public SocketGuildUser? GetSocketGuildUserById(ulong _id)
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

    public bool CheckIfUserHasPlayerProfile(ulong _userId)
    {
        bool contains = false;
        Log.WriteLine("Checking if " + _userId + " has a player profile.", LogLevel.VERBOSE);
        contains = PlayerIDs.Any(x => x.Key == _userId);
        Log.WriteLine(_userId + " contains: " + contains, LogLevel.VERBOSE);
        return contains;
    }

    public bool CheckIfPlayerDataPlayerIDsContainsKey(ulong _userId)
    {
        bool contains = false;
        Log.WriteLine("Checking if " + _userId + " contains.", LogLevel.VERBOSE);
        contains = PlayerIDs.ContainsKey(_userId);
        Log.WriteLine(_userId + " contains: " + contains, LogLevel.VERBOSE);
        return contains;
    }

    public Player GetAPlayerProfileById(ulong _playerId)
    {
        Log.WriteLine("Getting Player by ID: " + _playerId, LogLevel.VERBOSE);

        Player FoundPlayer = PlayerIDs.FirstOrDefault(x => x.Key == _playerId).Value;
        Log.WriteLine("Found: " + FoundPlayer.PlayerNickName + " (" +
            FoundPlayer.PlayerDiscordId + ")", LogLevel.VERBOSE);

        return FoundPlayer;
    }

    public async Task HandleRegisteredMemberUpdated(
    Cacheable<SocketGuildUser, ulong> before, SocketGuildUser _socketGuildUserAfter)
    {
        if (!PlayerIDs.ContainsKey(_socketGuildUserAfter.Id))
        {
            Log.WriteLine("PlayerId's does not contain key: " + _socketGuildUserAfter.Id +
                " disregarding (player is not registered)", LogLevel.VERBOSE);
            return;
        }

        var playerValue =
            Database.Instance.PlayerData.GetAPlayerProfileById(_socketGuildUserAfter.Id);

        if (playerValue == null)
        {
            Log.WriteLine("Trying to update " + _socketGuildUserAfter.Username +
            "'s profile, no valid player found (not registered?) ", LogLevel.DEBUG);
            return;
        }

        // This should not be empty, since it's being looked up from the database
        string playerValueNickName = playerValue.PlayerNickName;

        string socketGuildUserAfterNickName =
            CheckIfNickNameIsEmptyAndReturnUsername(_socketGuildUserAfter.Id);

        Log.WriteLine("Updating user: " + _socketGuildUserAfter.Username + " ("
            + _socketGuildUserAfter.Id + ")" + " | name: " + playerValueNickName +
            " -> " + socketGuildUserAfterNickName, LogLevel.DEBUG);

        playerValue.PlayerNickName = socketGuildUserAfterNickName;
        await SerializationManager.SerializeDB();
    }

    public async Task<bool> DeletePlayerProfile(string _dataValue)
    {
        ulong userId = UInt64.Parse(_dataValue);

        Log.WriteLine("Starting to remove the player profile: " + userId, LogLevel.VERBOSE);
        if (!CheckIfUserHasPlayerProfile(userId))
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


        Log.WriteLine("Done removing the player profile: " + userId, LogLevel.DEBUG);

        return true;
    }

}