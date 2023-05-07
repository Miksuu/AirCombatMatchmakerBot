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
            return playerDiscordId.GetValue();
        }
        set
        {
            playerDiscordId.SetValue(value);
        }
    }

    [DataMember] private string playerNickName { get; set; }
    [DataMember] private logUlong playerDiscordId = new logUlong();

    public Player()
    {
        playerNickName = string.Empty;
        PlayerDiscordId = 0;
    }

    public Player(ulong _playerDiscordID, string _playerNickName)
    {

        PlayerDiscordId = _playerDiscordID;
        playerNickName = _playerNickName;
    }

    public string GetPlayerIdAsMention()
    {
        Log.WriteLine("Getting player: " + playerNickName + " (" +
            PlayerDiscordId + ") as a mention", LogLevel.VERBOSE);

        return "<@" + PlayerDiscordId.ToString() + ">";
    }
}