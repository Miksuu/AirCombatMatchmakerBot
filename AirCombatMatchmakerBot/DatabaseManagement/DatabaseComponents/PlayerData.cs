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
        Log.WriteLine("Adding a player profile: " + _Player.GetPlayerNickname() + " (" +
            _Player.GetPlayerDiscordId() + ") to the PlayerIDs list", LogLevel.VERBOSE);

        PlayerIDs.Add(_Player.GetPlayerDiscordId(), _Player);
        Log.WriteLine("Done adding, count is now: " + PlayerIDs.Count, LogLevel.VERBOSE);
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
        Player FoundPlayer = PlayerIDs.First(x => x.Key == _playerId).Value;
        Log.WriteLine("Found: " + FoundPlayer.GetPlayerNickname() + " (" +
            FoundPlayer.GetPlayerDiscordId() + ")", LogLevel.VERBOSE);
        return FoundPlayer;
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

        var user = UserManager.GetSocketGuildUserById(userId);

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