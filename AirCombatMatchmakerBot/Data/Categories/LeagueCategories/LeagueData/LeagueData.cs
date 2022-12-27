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


    [DataMember] private Teams teams { get; set; }
    [DataMember] private ChallengeStatus challengeStatus { get; set; }
    [DataMember] private bool matchmakerActive { get; set; }
    public LeagueData()
    {
        teams = new();
        challengeStatus = new();
    }

    public Team? FindActiveTeamByPlayerIdInAPredefinedLeague(ulong _playerId)
    {
        Log.WriteLine("Starting to find a active team by player id: " + _playerId, LogLevel.VERBOSE);

        foreach (Team team in teams.TeamsList)
        {
            Team? foundTeam = team.CheckIfTeamIsActiveAndContainsAPlayer(_playerId);

            if (foundTeam != null)
            {
                return foundTeam;
            }
        }

        Log.WriteLine("Team not found! Admin trying to access challenge" +
            " of a league that he's not registered to?", LogLevel.WARNING);

        return null;
    }

    public void PostChallengeToThisLeague(ulong _playerId, int _leaguePlayerCountPerTeam)
    {
        Team? team = FindActiveTeamByPlayerIdInAPredefinedLeague(_playerId);

        if (team == null)
        {
            Log.WriteLine(nameof(team) +
                " was null! Could not find the team.", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Team found: " + team.GetTeamName() + " (" + team.GetTeamId() + ")" +
            " adding it to the challenge queue: " +
            challengeStatus.TeamsInTheQueue,
            LogLevel.VERBOSE);

        challengeStatus.AddToTeamsInTheQueue(team);

        Log.WriteLine(challengeStatus.ReturnTeamsInTheQueueOfAChallenge(
            _leaguePlayerCountPerTeam), LogLevel.VERBOSE);
    }
}