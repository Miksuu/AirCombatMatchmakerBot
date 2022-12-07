using Discord;
using Discord.WebSocket;
using System;
using System.Numerics;
using System.Threading.Tasks;

[Serializable]
public class LeagueData
{
    public List<Team> Teams { get; set; }
    public bool active { get; set; }
    public ulong leagueChannelMessageId { get; set; }
    public LeagueData()
    {
        Teams = new();
    }
}