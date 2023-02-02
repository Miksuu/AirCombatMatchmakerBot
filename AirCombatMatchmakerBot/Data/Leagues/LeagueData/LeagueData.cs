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

    public Team? FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(ulong _playerId)
    {
        Log.WriteLine("Starting to find a active team by player id: " + _playerId, LogLevel.VERBOSE);

        foreach (Team team in Teams.TeamsList)
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

        Team? foundTeam = Teams.TeamsList.FirstOrDefault(t => t.TeamId == _teamId && t.TeamActive);

        if (foundTeam != null)
        {
            return foundTeam;
        }

        Log.WriteLine("Found team: " + foundTeam.TeamName + " with id: " + _teamId, LogLevel.VERBOSE);

        return foundTeam;
    }

    /*
    public Team? FindTeamInWhichThePlayerIsActiveIn(InterfaceLeague _interfaceLeague, ulong _playerId)
    {
        // Find the teams that the players is in the league that was selected earlier
        List<Team> teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn =
            _interfaceLeague.LeagueData.Teams.TeamsList.Where(
                t => t.CheckIfATeamContainsAPlayerById(_playerId)).ToList();

        if (teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn == null ||
            teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn.Count < 1)
        {
            Log.WriteLine("Error! " + nameof(teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn) +
                " was null or empty!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("count: " + teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn.Count +
            " of all teams: ", LogLevel.VERBOSE);

        foreach (Team team in teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn)
        {
            Log.WriteLine("Team: " + team.TeamName + " with id: " + team.TeamId +
                " active: " + team.TeamActive, LogLevel.VERBOSE);
        }

        List<Team> isActiveInTeams =
            teamsWithThePlayerIdInTheLeagueThatThePlayerIsIn.Where(
                t => t.TeamActive).ToList();

        if (isActiveInTeams == null || isActiveInTeams.Count < 1)
        {
            Log.WriteLine("Error! " + nameof(isActiveInTeams) + " was null or empty!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("count: " + isActiveInTeams.Count + " of all teams: ", LogLevel.VERBOSE);

        // Should be always only one, because the player should be able to parcipiate only with one team at the time
        if (isActiveInTeams.Count != 1)
        {
            Log.WriteLine("count was not 1!!", LogLevel.DEBUG);

            foreach (Team team in isActiveInTeams)
            {
                Log.WriteLine("Team: " + team.TeamName + " with id: " + team.TeamId +
                    " active: " + team.TeamActive, LogLevel.DEBUG);
            }

            Log.WriteLine("Error! The player was active in two teams at the same time.", LogLevel.ERROR);

            // Handle error, take in the ID's of the current teams in the match and pick the player from there
            // Might be something that doesn't need to be handled if the system is proven to work fine
        }

        Team? foundTeam = isActiveInTeams.FirstOrDefault();

        if (foundTeam == null)
        {
            Log.WriteLine(nameof(foundTeam) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found team: " + foundTeam.TeamName + " with id: " + foundTeam.TeamId +
            " that should be active: " + foundTeam.TeamActive, LogLevel.DEBUG);

        return foundTeam;
    }*/
}