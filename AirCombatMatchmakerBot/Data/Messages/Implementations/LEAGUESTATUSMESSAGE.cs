using System.Data;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

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
        ConcurrentBag<Team> sortedTeamConcurrentBagByElo = new ConcurrentBag<Team>();

        InterfaceLeague? interfaceLeague = Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(messageCategoryId);

        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.ERROR);
            return nameof(interfaceLeague) + " was null!";
        }

        sortedTeamConcurrentBagByElo =
            new ConcurrentBag<Team>(interfaceLeague.LeagueData.Teams.TeamsConcurrentBag.OrderByDescending(
                x => x.SkillRating));

        foreach (Team team in sortedTeamConcurrentBagByElo)
        {
            finalMessage += "[" + team.SkillRating + "] " + team.TeamName + "\n";
        }

        Log.WriteLine("Generated the leaderboard: " + finalMessage, LogLevel.VERBOSE);

        return finalMessage;
    }
}