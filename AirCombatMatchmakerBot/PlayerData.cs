using Discord;
using Discord.WebSocket;
using System;
using System.Numerics;
using System.Threading.Tasks;

[Serializable]
public class PlayerData
{
    public Dictionary<ulong, Player> PlayerIDs { get; set; }
    public PlayerData()
    {
        PlayerIDs = new Dictionary<ulong, Player>();
    }
}