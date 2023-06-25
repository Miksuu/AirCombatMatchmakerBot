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

    public override Task<string> GenerateMessage()
    {
        string finalMessage = string.Empty;
        List<Team> sortedTeamListByElo = new List<Team>();

        InterfaceLeague interfaceLeague = 
            Database.Instance.Leagues.GetILeagueByCategoryId(thisInterfaceMessage.MessageCategoryId);

        sortedTeamListByElo =
            new List<Team>(interfaceLeague.LeagueData.Teams.TeamsConcurrentBag.OrderByDescending(
                x => x.SkillRating));

        foreach (Team team in sortedTeamListByElo)
        {
            finalMessage += "[" + team.SkillRating + "] " + team.TeamName + " | " + team.GetTeamStats() + "\n";
        }
        if (sortedTeamListByElo.Count > 0)
        {
            Log.WriteLine("Generated the leaderboard (" + sortedTeamListByElo.Count + "): " + finalMessage);
        }
        else
        {
            Log.WriteLine("Generated the leaderboard: " + finalMessage);
        }

        return Task.FromResult(finalMessage);
    }
}