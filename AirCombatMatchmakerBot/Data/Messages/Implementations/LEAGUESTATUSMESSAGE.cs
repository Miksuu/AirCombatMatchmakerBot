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
        messageEmbedTitle = "Leaderboard:\n"; ;
        messageDescription = "";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override string GenerateMessage()
    {
        string finalMessage = string.Empty;
        ConcurrentBag<Team> sortedTeamConcurrentBagByElo = new ConcurrentBag<Team>();

        InterfaceLeague? interfaceLeague = Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(messageCategoryId);

        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.ERROR);
            return nameof(interfaceLeague) + " was null!";
        }

        sortedTeamConcurrentBagByElo =
            new ConcurrentBag<Team>(interfaceLeague.LeagueData.Teams.TeamsConcurrentBag.OrderBy(
                x => x.SkillRating));

        foreach (Team team in sortedTeamConcurrentBagByElo)
        {
            finalMessage += "[" + team.SkillRating + "] " + team.TeamName + "\n";
        }
        if (sortedTeamConcurrentBagByElo.Count > 0)
        {
            Log.WriteLine("Generated the leaderboard (" + sortedTeamConcurrentBagByElo.Count + "): " + finalMessage, LogLevel.VERBOSE);
        }
        else
        {
            Log.WriteLine("Generated the leaderboard: " + finalMessage, LogLevel.VERBOSE);
        }

        return finalMessage;
    }
}