using System.Runtime.Serialization;

[DataContract]
public class Teams
{
    public List<Team> TeamsList
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamsList) + " with count of: " +
                teamsList.Count, LogLevel.VERBOSE);
            return teamsList;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamsList) + teamsList
                + " to: " + value, LogLevel.VERBOSE);
            teamsList = value;
        }
    }

    public int CurrentTeamInt
    {
        get
        {
            Log.WriteLine("Getting " + nameof(currentTeamInt) + currentTeamInt, LogLevel.VERBOSE);
            return currentTeamInt;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(currentTeamInt) + currentTeamInt
                + " to: " + value, LogLevel.VERBOSE);
            currentTeamInt = value;
        }
    }

    [DataMember] private List<Team> teamsList { get; set; }
    [DataMember] private int currentTeamInt { get; set; }

    public Teams() 
    {
        teamsList = new List<Team>();
        currentTeamInt = 1;
    }

    public void AddToListOfTeams(Team _Team)
    {
        Log.WriteLine(
            "Adding Team: " + _Team + " (" + _Team.TeamId + ") to the Teams list", LogLevel.VERBOSE);
        teamsList.Add(_Team);
        Log.WriteLine("Done adding the team. Count is now: " + teamsList.Count, LogLevel.VERBOSE);
    }

    /*
    public List<Team> GetListOfTeams()
    {
        Log.WriteLine("Getting list of teams with count of: " + teamsList.Count, LogLevel.VERBOSE);
        return teamsList;
    }

    public int GetCurrentTeamInt()
    {
        Log.WriteLine("Getting currentTeamInt: " + currentTeamInt, LogLevel.VERBOSE);
        return currentTeamInt;
    }*/

    public void IncrementCurrentTeamInt()
    {
        Log.WriteLine("Incrementing current team int that has count of: " +
            currentTeamInt, LogLevel.VERBOSE);
        currentTeamInt++;
    }

    public bool CheckIfPlayerIsAlreadyInATeamById(
        int _leagueTeamSize, ulong _idToSearchFor)
    {
        foreach (Team team in teamsList)
        {
            //List<Player> Players = team.Players;

            Log.WriteLine("Searching team: " + team.GetTeamName(_leagueTeamSize), LogLevel.VERBOSE);

            foreach (Player teamPlayer in team.Players)
            {
                Log.WriteLine("Checking player: " + teamPlayer.PlayerNickName +
                    " (" + teamPlayer.PlayerDiscordId + ")", LogLevel.VERBOSE);

                if (teamPlayer.PlayerDiscordId == _idToSearchFor)
                {
                    return true;
                }
            }
        }

        Log.WriteLine("Did not find any teams that the player was in the league", LogLevel.VERBOSE);

        return false;
    }

    // Always run CheckIfPlayerIsAlreadyInATeamById() before!
    public Team ReturnTeamThatThePlayerIsIn(
        int _leagueTeamSize, ulong _idToSearchFor)
    {
        foreach (Team team in teamsList)
        {
            List<Player> Players = team.Players;

            Log.WriteLine("Searching team: " + team.GetTeamName(
                _leagueTeamSize) + " with " + Players.Count, LogLevel.VERBOSE);

            foreach (Player teamPlayer in Players)
            {
                Log.WriteLine("Checking player: " + teamPlayer.PlayerNickName +
                    " (" + teamPlayer.PlayerDiscordId + ")", LogLevel.VERBOSE);

                if (teamPlayer.PlayerDiscordId == _idToSearchFor)
                {
                    return team;
                }
            }
        }

        Log.WriteLine("Did not find any teams that the player was in the league", LogLevel.CRITICAL);

        return new Team();
    }


    public Team FindTeamById(int _leagueTeamSize, int _id)
    {
        Log.WriteLine("Finding team by id: " + _id, LogLevel.VERBOSE);

        if (!teamsList.Any(x => x.TeamId == _id))
        {
            Log.WriteLine("Trying to get a team by id: + " + _id +
                " that is nonexistent!", LogLevel.ERROR);
            return new Team();
        }

        Team foundTeam = teamsList.FirstOrDefault(x => x.TeamId == _id);

        Log.WriteLine("Found team: " + foundTeam.GetTeamName(
            _leagueTeamSize) +" id: " + foundTeam.TeamId + 
            " with members: " + foundTeam.GetTeamMembersInAString(), LogLevel.DEBUG);

        return foundTeam;
    }
}