using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class Teams
{
    public ConcurrentBag<Team> TeamsConcurrentBag
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamsConcurrentBag) + " with count of: " +
                teamsConcurrentBag.Count, LogLevel.VERBOSE);
            return teamsConcurrentBag;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamsConcurrentBag) + teamsConcurrentBag
                + " to: " + value, LogLevel.VERBOSE);
            teamsConcurrentBag = value;
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

    [DataMember] private ConcurrentBag<Team> teamsConcurrentBag { get; set; }
    [DataMember] private int currentTeamInt { get; set; }

    public Teams() 
    {
        teamsConcurrentBag = new ConcurrentBag<Team>();
        currentTeamInt = 1;
    }

    public void AddToConcurrentBagOfTeams(Team _Team)
    {
        Log.WriteLine(
            "Adding Team: " + _Team + " (" + _Team.TeamId + ") to the Teams ConcurrentBag", LogLevel.VERBOSE);
        teamsConcurrentBag.Add(_Team);
        Log.WriteLine("Done adding the team. Count is now: " + teamsConcurrentBag.Count, LogLevel.VERBOSE);
    }

    /*
    public ConcurrentBag<Team> GetConcurrentBagOfTeams()
    {
        Log.WriteLine("Getting ConcurrentBag of teams with count of: " + teamsConcurrentBag.Count, LogLevel.VERBOSE);
        return teamsConcurrentBag;
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
        foreach (Team team in teamsConcurrentBag)
        {
            //ConcurrentBag<Player> Players = team.Players;

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

    public bool CheckIfPlayersTeamIsActiveById(
        int _leagueTeamSize, ulong _idToSearchFor)
    {
        foreach (Team team in teamsConcurrentBag)
        {
            Log.WriteLine("Searching team: " + team.GetTeamName(_leagueTeamSize), LogLevel.VERBOSE);

            foreach (Player teamPlayer in team.Players)
            {
                Log.WriteLine("Checking player: " + teamPlayer.PlayerNickName +
                    " (" + teamPlayer.PlayerDiscordId + ")", LogLevel.VERBOSE);

                if (teamPlayer.PlayerDiscordId == _idToSearchFor)
                {
                    if (team.TeamActive) return true;
                }
            }
        }

        Log.WriteLine("Did not find any teams that the player was active in the league", LogLevel.VERBOSE);

        return false;
    }

    // Always run CheckIfPlayerIsAlreadyInATeamById() before!
    public Team ReturnTeamThatThePlayerIsIn(
        int _leagueTeamSize, ulong _idToSearchFor)
    {
        foreach (Team team in teamsConcurrentBag)
        {
            ConcurrentBag<Player> Players = team.Players;

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

        if (!teamsConcurrentBag.Any(x => x.TeamId == _id))
        {
            Log.WriteLine("Trying to get a team by id: + " + _id +
                " that is nonexistent!", LogLevel.CRITICAL);
            return new Team();
        }

        Team? foundTeam = teamsConcurrentBag.FirstOrDefault(x => x.TeamId == _id);

        if (foundTeam == null)
        {
            Log.WriteLine(nameof(foundTeam) + " was null!", LogLevel.CRITICAL);
            return new Team();
        }

        Log.WriteLine("Found team: " + foundTeam.GetTeamName(
            _leagueTeamSize) +" id: " + foundTeam.TeamId + 
            " with members: " + foundTeam.GetTeamMembersInAString(), LogLevel.DEBUG);

        return foundTeam;
    }
}