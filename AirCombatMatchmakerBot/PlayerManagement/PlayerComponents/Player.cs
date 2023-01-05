using System.Runtime.Serialization;

[DataContract]
public class Player
{
    public string PlayerNickName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(playerNickName), LogLevel.VERBOSE);
            return playerNickName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(playerNickName)
                + " to: " + value, LogLevel.VERBOSE);
            playerNickName = value;
        }
    }

    public ulong PlayerDiscordId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(playerDiscordId), LogLevel.VERBOSE);
            return playerDiscordId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(playerDiscordId)
                + " to: " + value, LogLevel.VERBOSE);
            playerDiscordId = value;
        }
    }

    [DataMember] private string playerNickName { get; set; }
    [DataMember] private ulong playerDiscordId { get; set; }

    public Player()
    {
        playerNickName = string.Empty;
        playerDiscordId = 0;
    }

    public Player(ulong _playerID, string _playerNickName)
    {
        playerNickName = _playerNickName;
        playerDiscordId = _playerID;
    }

    public string GetPlayerIdAsMention()
    {
        Log.WriteLine("Getting player: " + playerNickName + " (" +
            playerDiscordId + ") as a mention", LogLevel.VERBOSE);

        return "<@" + playerDiscordId.ToString() + ">";
    }
}