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
    [DataMember] private bool matchmakerActive { get; set; }
    public LeagueData()
    {
        teams = new();
        challengeStatus = new();
        matches = new Matches();
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
}