using System.Runtime.Serialization;

[DataContract]
public class Teams
{
    [DataMember] private List<Team> TeamsList { get; set; }
    [DataMember] private int currentTeamInt { get; set; }

    public Teams() 
    {
        TeamsList = new List<Team>();
        currentTeamInt = 1;
    }

    public void AddToTeams(Team _Team)
    {
        Log.WriteLine(
            "Adding Team: " + _Team + " (" + _Team.GetTeamId() + ") to the Teams list", LogLevel.VERBOSE);
        TeamsList.Add(_Team);
        Log.WriteLine("Done adding the team. Count is now: " + TeamsList.Count, LogLevel.VERBOSE);
    }

    public List<Team> GetListOfTeams()
    {
        Log.WriteLine("Getting list of Teams with count of: " + TeamsList.Count, LogLevel.VERBOSE);
        return TeamsList;
    }

    public int GetCurrentTeamInt()
    {
        Log.WriteLine("Getting currentTeamInt: " + currentTeamInt, LogLevel.VERBOSE);
        return currentTeamInt;
    }

    public void IncrementCurrentTeamInt()
    {
        Log.WriteLine("Incrementing current team int that has count of: " + currentTeamInt, LogLevel.VERBOSE);
        currentTeamInt++;
    }

    public bool CheckIfPlayerIsAlreadyInATeamById(ulong _idToSearchFor)
    {
        foreach (Team team in TeamsList)
        {
            List<Player> Players = team.GetListOfPlayersInATeam();

            Log.WriteLine("Searching team: " + team.GetTeamName() +
                " with " + Players.Count, LogLevel.VERBOSE);

            foreach (Player teamPlayer in Players)
            {
                Log.WriteLine("Checking player: " + teamPlayer.GetPlayerNickname() +
                    " (" + teamPlayer.GetPlayerDiscordId() + ")", LogLevel.VERBOSE);

                if (teamPlayer.GetPlayerDiscordId() == _idToSearchFor)
                {
                    return true;
                }
            }
        }

        Log.WriteLine("Did not find any teams that the player was in the league", LogLevel.VERBOSE);

        return false;
    }

    // Always run CheckIfPlayerIsAlreadyInATeamById() before!
    public Team ReturnTeamThatThePlayerIsIn(ulong _idToSearchFor)
    {
        foreach (Team team in TeamsList)
        {
            List<Player> Players = team.GetListOfPlayersInATeam();

            Log.WriteLine("Searching team: " + team.GetTeamName() +
                " with " + Players.Count, LogLevel.VERBOSE);

            foreach (Player teamPlayer in Players)
            {
                Log.WriteLine("Checking player: " + teamPlayer.GetPlayerNickname() +
                    " (" + teamPlayer.GetPlayerDiscordId() + ")", LogLevel.VERBOSE);

                if (teamPlayer.GetPlayerDiscordId() == _idToSearchFor)
                {
                    return team;
                }
            }
        }

        Log.WriteLine("Did not find any teams that the player was in the league", LogLevel.CRITICAL);

        return new Team();
    }
}