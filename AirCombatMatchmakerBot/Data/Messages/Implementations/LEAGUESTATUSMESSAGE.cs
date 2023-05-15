using System.Data;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class LEAGUESTATUSMESSAGE : BaseMessage
{
    public LEAGUESTATUSMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.LEAGUESTATUSMESSAGE;
        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>
        {
        };
        thisInterfaceMessage.MessageEmbedTitle = "Leaderboard:\n"; ;
        thisInterfaceMessage.MessageDescription = "";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override string GenerateMessage()
    {
        string finalMessage = string.Empty;
        List<Team> sortedTeamListgByElo = new List<Team>();

        InterfaceLeague? interfaceLeague = 
            Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(thisInterfaceMessage.MessageCategoryId);

        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.ERROR);
            return nameof(interfaceLeague) + " was null!";
        }

        sortedTeamListgByElo =
            new List<Team>(interfaceLeague.LeagueData.Teams.TeamsConcurrentBag.OrderBy(
                x => x.SkillRating));

        foreach (Team team in sortedTeamListgByElo)
        {
            finalMessage += "[" + team.SkillRating + "] " + team.TeamName + "\n";
        }
        if (sortedTeamListgByElo.Count > 0)
        {
            Log.WriteLine("Generated the leaderboard (" + sortedTeamListgByElo.Count + "): " + finalMessage, LogLevel.VERBOSE);
        }
        else
        {
            Log.WriteLine("Generated the leaderboard: " + finalMessage, LogLevel.VERBOSE);
        }

        return finalMessage;
    }
}