using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;
using System.Collections.Concurrent;

[DataContract]
public class LEAGUESTATUSMESSAGE : BaseMessage
{
    public LEAGUESTATUSMESSAGE()
    {
        messageName = MessageName.LEAGUESTATUSMESSAGE;
        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int> 
        {
        };
        message = "Leaderboard:\n";
    }

    public override string GenerateMessage()
    {
        string finalMessage = "Leaderboard:\n";
        List<Team> sortedTeamListByElo = new List<Team>();

        InterfaceLeague? interfaceLeague = Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(messageCategoryId);

        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.ERROR);
            return nameof(interfaceLeague) + " was null!";
        }

        sortedTeamListByElo = interfaceLeague.LeagueData.Teams.TeamsList.OrderByDescending(x => x.SkillRating).ToList();

        foreach (Team team in sortedTeamListByElo)
        {
            finalMessage += "[" + team.SkillRating + "] " + team.TeamName + "\n";
        }

        Log.WriteLine("Generated the leaderboard", LogLevel.VERBOSE);

        return finalMessage;
    }
}