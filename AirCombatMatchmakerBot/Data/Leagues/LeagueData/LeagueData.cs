using Discord;
using System.Runtime.Serialization;

[DataContract]
public class LeagueData : logClass<LeagueData>
{
    public Teams Teams
    {
        get => teams.GetValue();
        set => teams.SetValue(value);
    }

    public ChallengeStatus ChallengeStatus
    {
        get => challengeStatus.GetValue();
        set => challengeStatus.SetValue(value);
    }

    public MatchScheduler MatchScheduler
    {
        get => matchScheduler.GetValue();
        set => matchScheduler.SetValue(value);
    }

    public Matches Matches
    {
        get => matches.GetValue();
        set => matches.SetValue(value);
    }

    [DataMember] private Teams teams = new Teams();
    [DataMember] private ChallengeStatus challengeStatus = new ChallengeStatus();
    [DataMember] private MatchScheduler matchScheduler = new MatchScheduler();
    [DataMember] private Matches matches = new Matches();

    public InterfaceLeague interfaceLeagueRef;

    // Loaded during the serialization
    public LeagueData(){ }

    // TODO: Create a method for this where everyclass implementing an interface maybe? does this

    public void SetReferences(InterfaceLeague _interfaceLeague)
    {
        //interfaceLeagueRef = Database.Instance.Leagues.GetILeagueByCategoryId(InterfaceLeagueCategoryId);
        interfaceLeagueRef = _interfaceLeague;
        Teams.interfaceLeagueRef = _interfaceLeague;
        ChallengeStatus.interfaceLeagueRef = _interfaceLeague;
        MatchScheduler.interfaceLeagueRef = _interfaceLeague;
        Matches.SetInterfaceLeagueReferencesForTheMatches(_interfaceLeague);
    }

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
            Log.WriteLine(nameof(foundTeam) + " was null!", LogLevel.CRITICAL);
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