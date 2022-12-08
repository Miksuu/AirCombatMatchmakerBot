[Serializable]
public class Player
{
    public string playerNickName { get; set; }
    public ulong playerDiscordId { get; set; }

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