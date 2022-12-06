using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

[Serializable]
public class Player
{
    public int skillRating { get; set; }
    public string playerNickName { get; set; }
    public ulong playerDiscordId { get; set; }

    public Player()
    {
        skillRating = 1600;
    }

    public Player(ulong _playerID, string _playerNickName)
    {
        skillRating = 1600;
        playerNickName = _playerNickName;
        playerDiscordId = _playerID;
    }
}