using System.Runtime.Serialization;

[DataContract]
public class Player
{
    [DataMember] private string playerNickName { get; set; }
    [DataMember] private ulong playerDiscordId { get; set; }

    public Player()
    {
        playerNickName= string.Empty;
        playerDiscordId= 0;
    }

    public Player(ulong _playerID, string _playerNickName)
    {
        playerNickName = _playerNickName;
        playerDiscordId = _playerID;
    }

    public string GetPlayerNickname()
    {
        Log.WriteLine("Getting " + nameof(playerNickName) + ": " + playerNickName, LogLevel.VERBOSE);
        return playerNickName;
    }
    public void SetPlayerNickname(string _nickNameToSetTo)
    {
        Log.WriteLine("Setting " + nameof(playerNickName) + ": " + playerNickName + " to:" +
            _nickNameToSetTo, LogLevel.VERBOSE);
        playerNickName = _nickNameToSetTo;
    }

    public ulong GetPlayerDiscordId()
    {
        Log.WriteLine("Getting " + nameof(playerDiscordId) + ": " + playerDiscordId, LogLevel.VERBOSE);
        return playerDiscordId;
    }
}