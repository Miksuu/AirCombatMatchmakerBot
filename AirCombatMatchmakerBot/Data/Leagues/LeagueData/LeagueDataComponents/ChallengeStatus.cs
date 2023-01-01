using System.Runtime.Serialization;
using System.Threading.Channels;

[DataContract]
public class ChallengeStatus
{
    public List<int> TeamsInTheQueue
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamsInTheQueue) +  " with count of: " +
                teamsInTheQueue.Count, LogLevel.VERBOSE);
            return teamsInTheQueue;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamsInTheQueue) + teamsInTheQueue
                + " to: " + value, LogLevel.VERBOSE);
            teamsInTheQueue = value;
        }
    }

    [DataMember] private List<int> teamsInTheQueue { get; set; }

    public ChallengeStatus()
    {
        teamsInTheQueue = new List<int>();
    }

    public void AddToTeamsInTheQueue(Team _Team)
    {
        Log.WriteLine("Adding Team: " + _Team + " (" + 
            _Team.GetTeamId() + ") to the queue", LogLevel.VERBOSE);
        teamsInTheQueue.Add(_Team.GetTeamId());
        Log.WriteLine("Done adding the team to the queue. Count is now: " +
            teamsInTheQueue.Count, LogLevel.VERBOSE);
    }

    // Returns the teams in the queue as a string
    // (useful for printing, in log on the challenge channel)
    public string ReturnTeamsInTheQueueOfAChallenge(int _leagueTeamSize, LeagueData _leagueData)
    {
        string teamsString = string.Empty;
        foreach (int teamInt in teamsInTheQueue)
        {
            Team team = _leagueData.Teams.FindTeamById(teamInt);
            teamsString += "[" + team.GetTeamSkillRating() + "] " + team.GetTeamName();
            if (_leagueTeamSize > 1)
            {
                teamsString += " (" + team.GetTeamMembersInAString() + ")";
            }
            teamsString += "\n";
        }
        return teamsString;
    }

    public async Task<string> PostChallengeToThisLeague(
        ulong _playerId, int _leaguePlayerCountPerTeam, InterfaceLeague _interfaceLeague)
    {
        Team? team =
            _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeague(_playerId);

        if (team == null)
        {
            Log.WriteLine(nameof(team) +
                " was null! Could not find the team.", LogLevel.CRITICAL);
            return "Error! Team not found";
        }

        Log.WriteLine("Team found: " + team.GetTeamName() + " (" + team.GetTeamId() + ")" +
            " adding it to the challenge queue: " +
            TeamsInTheQueue,
            LogLevel.VERBOSE);

        if (TeamsInTheQueue.Any(x => x == team.GetTeamId()))
        {
            Log.WriteLine("Team " + team.GetTeamName() + " (" + team.GetTeamId() + ")" +
                " was already in queue!", LogLevel.DEBUG);
            return "alreadyInQueue";
        }

        AddToTeamsInTheQueue(team);

        await CheckChallengeStatus(_interfaceLeague);

        string teamsInTheQueue =
            ReturnTeamsInTheQueueOfAChallenge(_leaguePlayerCountPerTeam, _interfaceLeague.LeagueData);

        Log.WriteLine("Teams in the queue: " + teamsInTheQueue, LogLevel.VERBOSE);

        return teamsInTheQueue;
    }

    public Task CheckChallengeStatus(InterfaceLeague _interfaceLeague)
    {
        int[] teamsToFormMatchOn = new int[2];

        Log.WriteLine("Checking challenge status with team amount: " +
            TeamsInTheQueue.Count, LogLevel.VERBOSE);

        // Replace this with some method later on that calculates ELO between the teams in the queue
        if (TeamsInTheQueue.Count < 2)
        {
            Log.WriteLine(nameof(teamsInTheQueue) + " count: " + TeamsInTheQueue.Count +
                " is smaller than 2, returning.", LogLevel.DEBUG);
            return Task.CompletedTask;
        }

        Log.WriteLine(nameof(teamsInTheQueue) + " count: " + teamsInTheQueue.Count +
            ", match found!", LogLevel.DEBUG);

        for (int t = 0; t < 2; t++)
        {
            Log.WriteLine("Looping on team index: " + t, LogLevel.VERBOSE);
            teamsToFormMatchOn[t] = teamsInTheQueue.First();
            Log.WriteLine("Done adding to " + nameof(teamsToFormMatchOn) +
                ", Length: " + teamsToFormMatchOn.Length, LogLevel.VERBOSE);
            teamsInTheQueue.RemoveAt(0);
            Log.WriteLine("Done removing from " + nameof(teamsInTheQueue) +
                ", count: " + teamsInTheQueue.Count, LogLevel.VERBOSE);
        }

        Log.WriteLine("Done looping.", LogLevel.VERBOSE);

        _interfaceLeague.LeagueData.Matches.CreateAMatch(_interfaceLeague, teamsToFormMatchOn);

        return Task.CompletedTask;
    }
}