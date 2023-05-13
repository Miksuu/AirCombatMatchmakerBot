using System.Runtime.Serialization;

[DataContract]
public class LeagueData : logClass<LeagueData>, InterfaceLoggableClass
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

    public Matches Matches
    {
        get => matches.GetValue();
        set => matches.SetValue(value);
    }

    [DataMember] private logClass<Teams> teams = new logClass<Teams>(new Teams());
    [DataMember] private logClass<ChallengeStatus> challengeStatus = new logClass<ChallengeStatus>(new ChallengeStatus());
    [DataMember] private logClass<Matches> matches = new logClass<Matches>(new Matches());

    public List<string> GetClassParameters()
    {
        return new List<string> { teams.GetParameter(), challengeStatus.GetParameter(), matches.GetParameter() };
    }

    public Team? FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(ulong _playerId)
    {
        Log.WriteLine("Starting to find a active team by player id: " + _playerId, LogLevel.VERBOSE);

        foreach (Team team in Teams.TeamsConcurrentBag)
        {
            Team? foundTeam = team.CheckIfTeamIsActiveAndContainsAPlayer(_playerId);

            if (foundTeam != null)
            {
                Log.WriteLine("Found team: " + foundTeam.TeamName +
                    " with id: " + foundTeam.TeamId, LogLevel.DEBUG);
                return foundTeam;
            }
        }

        Log.WriteLine("Team not found! Admin trying to access challenge" +
            " of a league that he's not registered to?", LogLevel.WARNING);

        return null;
    }

    public Team? FindActiveTeamWithTeamId(int _teamId)
    {
        Log.WriteLine("Starting to find team with id: " + _teamId, LogLevel.VERBOSE);

        Team? foundTeam = Teams.TeamsConcurrentBag.FirstOrDefault(t => t.TeamId == _teamId && t.TeamActive);

        if (foundTeam == null)
        {
            Log.WriteLine(nameof(foundTeam) + " was null!", LogLevel.CRITICAL);
            return foundTeam;
        }

        Log.WriteLine("Found team: " + foundTeam.TeamName + " with id: " + _teamId, LogLevel.VERBOSE);

        return foundTeam;
    }

    public bool CheckIfPlayerIsParcipiatingInTheLeague(ulong _playerId)
    {
        Log.WriteLine("Checking if: " + _playerId + " is participiating in league.", LogLevel.VERBOSE);

        foreach (Team team in Teams.TeamsConcurrentBag)
        {
            Team? foundTeam = team.CheckIfTeamIsActiveAndContainsAPlayer(_playerId);

            if (foundTeam != null)
            {
                Log.WriteLine("Found team: " + foundTeam.TeamName +
                    " with id: " + foundTeam.TeamId, LogLevel.DEBUG);
                return true;
            }

            Log.WriteLine(nameof(team) + " was not found (null), continuing", LogLevel.VERBOSE);
        }

        Log.WriteLine("Didn't find that " + _playerId + " was participiating.", LogLevel.VERBOSE);

        return false;
    }
}