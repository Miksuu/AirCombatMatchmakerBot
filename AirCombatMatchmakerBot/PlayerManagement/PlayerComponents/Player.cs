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
}