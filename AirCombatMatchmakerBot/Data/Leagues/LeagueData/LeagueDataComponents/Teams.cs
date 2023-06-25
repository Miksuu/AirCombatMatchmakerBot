using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class Teams : logClass<Teams>
{
    public ConcurrentBag<Team> TeamsConcurrentBag
    {
        get => teamsConcurrentBag.GetValue();
        set => teamsConcurrentBag.SetValue(value);
    }
    public int CurrentTeamInt
    {
        get => currentTeamInt.GetValue();
        set => currentTeamInt.SetValue(value);
    }

    [DataMember] private logConcurrentBag<Team> teamsConcurrentBag = new logConcurrentBag<Team>();
    [DataMember] private logClass<int> currentTeamInt = new logClass<int>(1);

    public InterfaceLeague interfaceLeagueRef;

    public Teams() { }

    public void AddToConcurrentBagOfTeams(Team _Team)
    {
        Log.WriteLine(
            "Adding Team: " + _Team + " (" + _Team.TeamId + ") to the Teams ConcurrentBag");
        TeamsConcurrentBag.Add(_Team);
        Log.WriteLine("Done adding the team. Count is now: " + TeamsConcurrentBag.Count);
    }

    public void IncrementCurrentTeamInt()
    {
        Log.WriteLine("Incrementing current team int that has count of: " +
            CurrentTeamInt);
        CurrentTeamInt++;
    }

    public bool CheckIfPlayerIsAlreadyInATeamById(ulong _idToSearchFor)
    {
        foreach (Team team in TeamsConcurrentBag)
        {
            Log.WriteLine("Searching team: " + team.GetTeamName(interfaceLeagueRef.LeaguePlayerCountPerTeam));

            foreach (Player teamPlayer in team.Players)
            {
                Log.WriteLine("Checking player: " + teamPlayer.PlayerNickName +
                    " (" + teamPlayer.PlayerDiscordId + ")");

                if (teamPlayer.PlayerDiscordId == _idToSearchFor)
                {
                    return true;
                }
            }
        }

        Log.WriteLine("Did not find any teams that the player was in the league");

        return false;
    }

    public Team CheckIfPlayersTeamIsActiveByIdAndReturnThatTeam(ulong _playerIdToSearchFor)
    {
        foreach (Team team in TeamsConcurrentBag)
        {
            Log.WriteLine("Searching team: " + team.GetTeamName(interfaceLeagueRef.LeaguePlayerCountPerTeam));

            foreach (Player teamPlayer in team.Players)
            {
                Log.WriteLine("Checking player: " + teamPlayer.PlayerNickName +
                    " (" + teamPlayer.PlayerDiscordId + ")");

                if (teamPlayer.PlayerDiscordId == _playerIdToSearchFor)
                {
                    if (team.TeamActive) return team;
                }
            }
        }

        Log.WriteLine("Did not find any teams that the player was active in the league");

        return new Team();
    }

    // Always run CheckIfPlayerIsAlreadyInATeamById() before!
    public Team ReturnTeamThatThePlayerIsIn(ulong _idToSearchFor)
    {
        foreach (Team team in TeamsConcurrentBag)
        {
            List<Player> Players = team.Players.ToList();

            Log.WriteLine("Searching team: " + team.GetTeamName(
                interfaceLeagueRef.LeaguePlayerCountPerTeam) + " with " + Players.Count);

            foreach (Player teamPlayer in Players)
            {
                Log.WriteLine("Checking player: " + teamPlayer.PlayerNickName +
                    " (" + teamPlayer.PlayerDiscordId + ")");

                if (teamPlayer.PlayerDiscordId == _idToSearchFor)
                {
                    return team;
                }
            }
        }

        Log.WriteLine("Did not find any teams that the player was in the league", LogLevel.CRITICAL);

        return new Team();
    }

    public Team FindTeamById( int _id)
    {
        Log.WriteLine("Finding team by id: " + _id);

        Team? foundTeam = TeamsConcurrentBag.FirstOrDefault(x => x.TeamId == _id);
        if (foundTeam == null)
        {
            Log.WriteLine(nameof(foundTeam) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(foundTeam) + " was null!");
        }

        Log.WriteLine("Found team: " + foundTeam.GetTeamName(
            interfaceLeagueRef.LeaguePlayerCountPerTeam) +" id: " + foundTeam.TeamId + 
            " with members: " + foundTeam.GetTeamMembersInAString(), LogLevel.DEBUG);

        return foundTeam;
    }
}