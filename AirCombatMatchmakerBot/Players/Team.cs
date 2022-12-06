using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

[Serializable]
public class Team
{
    public int skillRating { get; set; }
    public string teamName { get; set; }
    public List<Player> players { get; set; }

    public Team()
    {
        skillRating = 1600;
    }

    public Team(List<Player> _players, string _teamName)
    {
        skillRating = 1600;
        teamName = _teamName;
        players = _players;
    }
}