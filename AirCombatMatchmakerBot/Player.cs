using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

[Serializable]
public class Player
{
    public int skillRating { get; set; }
    public string playerName { get; set; }
    public ulong playerId { get; set; }

    public Player()
    {
        skillRating = 1600;
    }

    public Player(ulong _playerID, string _playerName)
    {
        skillRating = 1600;
        playerName = _playerName;
        playerId = _playerID;
    }
}