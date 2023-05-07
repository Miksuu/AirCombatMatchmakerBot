using Discord;
using System.Runtime.Serialization;

[DataContract]
public class LeagueData
{
    public Teams Teams
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teams), LogLevel.VERBOSE);
            return teams;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teams)
                + " to: " + value, LogLevel.VERBOSE);
            teams = value;
        }
    }

    public ChallengeStatus ChallengeStatus
    {
        get
        {
            Log.WriteLine("Getting " + nameof(challengeStatus), LogLevel.VERBOSE);
            return challengeStatus;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(challengeStatus)
                + " to: " + value, LogLevel.VERBOSE);
            challengeStatus = value;
        }
    }
    public Matches Matches
    {
        get
        {
            Log.WriteLine("Getting " + nameof(matches), LogLevel.VERBOSE);
            return matches;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(matches)
                + " to: " + value, LogLevel.VERBOSE);
            matches = value;
        }
    }

    [DataMember] private Teams teams { get; set; }
    [DataMember] private ChallengeStatus challengeStatus { get; set; }
    [DataMember] private Matches matches { get; set; }
    [DataMember] private logBool matchmakerActive { get; set; }
    public LeagueData()
    {
        teams = new();
        challengeStatus = new();
        matches = new Matches();
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