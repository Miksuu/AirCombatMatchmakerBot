﻿using Discord;
using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

[DataContract]
public class PlayerData : logClass, InterfaceLoggableClass
{
    [IgnoreDataMember]
    public ConcurrentDictionary<ulong, Player> PlayerIDs
    {
        get => playerIDs.GetValue();
        set => playerIDs.SetValue(value);
    }

    [DataMember] private logConcurrentDictionary<ulong, Player> playerIDs = new logConcurrentDictionary<ulong, Player>();

    public List<string> GetClassParameters()
    {
        return new List<string> { playerIDs.GetLoggingClassParameters() };
    }

    public void AddAPlayerProfile(Player _Player)
    {
        Log.WriteLine("Adding a player profile: " + _Player.PlayerNickName + " (" +
            _Player.PlayerDiscordId + ") to the PlayerIDs ConcurrentDictionary", LogLevel.VERBOSE);

        PlayerIDs.TryAdd(_Player.PlayerDiscordId, _Player);
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

        try
        {
            var SocketGuildUser = GetSocketGuildUserById(_id);

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
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return ex.Message;
        }
    }

    // Gets the user by the discord UserId. This may not be present in the Database.
    public SocketGuildUser GetSocketGuildUserById(ulong _id)
    {
        Log.WriteLine("Getting SocketGuildUser by id: " + _id, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {        
            throw new InvalidOperationException(Exceptions.BotGuildRefNull());
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
            "'s profile, no valid player found (not registered probably) ", LogLevel.DEBUG);
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

    public Task DeletePlayerProfile(ulong _playerDiscordId)
    {
        Log.WriteLine("Starting to remove the player profile: " + _playerDiscordId, LogLevel.VERBOSE);
        if (!CheckIfUserHasPlayerProfile(_playerDiscordId))
        {
            Log.WriteLine("Did not find ID: " + _playerDiscordId + "in the local database.", LogLevel.DEBUG);
            return Task.CompletedTask;
        }

        Log.WriteLine("Deleting a player profile " + _playerDiscordId, LogLevel.DEBUG);
        Database.Instance.PlayerData.PlayerIDs.TryRemove(_playerDiscordId, out Player? _player);

        var user = GetSocketGuildUserById(_playerDiscordId);
        // If the user is in the server
        if (user == null)
        {
            Log.WriteLine("User with id: " + _playerDiscordId + " was null!!" +
                " Was not found in the server probably", LogLevel.DEBUG);
            return Task.CompletedTask;
        }

        Log.WriteLine("User found in the server", LogLevel.VERBOSE);

        Log.WriteLine("Done removing the player profile: " + _playerDiscordId, LogLevel.DEBUG);

        return Task.CompletedTask;
    }
}