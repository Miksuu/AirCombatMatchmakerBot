using System.Runtime.Serialization;

[DataContract]
public class Player
{
    public ulong PlayerDiscordId
    {
        get => playerDiscordId.GetValue();
        set => playerDiscordId.SetValue(value);
    }


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

    [DataMember] private logUlong playerDiscordId = new logUlong();
    [DataMember] private logString playerNickName { get; set; }

    public Player()
    {

        PlayerDiscordId = 0;
        playerNickName = string.Empty;
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