using Discord;
using System.Runtime.Serialization;

[DataContract]
public class LeagueData
{
    [DataMember] public Teams Teams = new Teams();
    [DataMember] public ChallengeStatus ChallengeStatus = new ChallengeStatus();
    [DataMember] public MatchScheduler MatchScheduler = new MatchScheduler();
    [DataMember] public Matches Matches = new Matches();

    // Loaded during the serialization
    public LeagueData(){ }

    // TODO: Create a method for this where everyclass implementing an interface maybe? does this

    // public void SetReferences(InterfaceLeague _interfaceLeague)
    // {
    //     //Database.GetInstance<ApplicationDatabase>().Leagues.GetILeagueByCategoryId(InterfaceLeagueCategoryId);
    //     // Teams.interfaceLeagueRef = _interfaceLeague;
    //     // ChallengeStatus.interfaceLeagueRef = _interfaceLeague;
    //     // MatchScheduler.interfaceLeagueRef = _interfaceLeague;
    //     Matches.SetInterfaceLeagueReferencesForTheMatches(_interfaceLeague);
    // }

    public Team FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(ulong _playerId)
    {
        Log.WriteLine("Starting to find a active team by player id: " + _playerId +
            " with team count: " + Teams.TeamsConcurrentBag.Count, LogLevel.DEBUG);

        foreach (var item in Teams.TeamsConcurrentBag)
        {
            Log.WriteLine(item.TeamName + "|" + item.TeamId + "|" + item.TeamActive, LogLevel.DEBUG);
        }

        foreach (Team team in Teams.TeamsConcurrentBag)
        {
            var foundTeam = team.CheckIfTeamIsActiveAndContainsAPlayer(_playerId);

            if (foundTeam.Item1 != null && foundTeam.Item2)
            {
                Log.WriteLine("Found team: " + foundTeam.Item1.TeamName +
                    " with id: " + foundTeam.Item1.TeamId, LogLevel.DEBUG);
                return foundTeam.Item1;
            }
        }

        Log.WriteLine("Team not found! Admin trying to access challenge" +
            " of a league that he's not registered to?", LogLevel.WARNING);

        throw new InvalidOperationException("Team not found!");
    }

    public Team FindActiveTeamWithTeamId(int _teamId)
    {
        Log.WriteLine("Starting to find team with id: " + _teamId);

        Team? foundTeam = Teams.TeamsConcurrentBag.FirstOrDefault(t => t.TeamId == _teamId && t.TeamActive);
        if (foundTeam == null)
        {
            Log.WriteLine(nameof(foundTeam) + " was null!", LogLevel.ERROR);
            throw new InvalidOperationException(nameof(foundTeam) + " was null!");
        }

        Log.WriteLine("Found team: " + foundTeam.TeamName + " with id: " + _teamId);

        return foundTeam;
    }

    public bool CheckIfPlayerIsParcipiatingInTheLeague(ulong _playerId)
    {
        Log.WriteLine("Checking if: " + _playerId + " is participiating in league.");

        foreach (Team team in Teams.TeamsConcurrentBag)
        {
            var foundTeam = team.CheckIfTeamIsActiveAndContainsAPlayer(_playerId);

            if (foundTeam.Item1 != null && foundTeam.Item2)
            {
                Log.WriteLine("Found team: " + foundTeam.Item1.TeamName +
                    " with id: " + foundTeam.Item1.TeamId, LogLevel.DEBUG);
                return true;
            }

            Log.WriteLine(nameof(team) + " was not found (null), continuing");
        }

        Log.WriteLine("Didn't find that " + _playerId + " was participiating.");

        return false;
    }
}