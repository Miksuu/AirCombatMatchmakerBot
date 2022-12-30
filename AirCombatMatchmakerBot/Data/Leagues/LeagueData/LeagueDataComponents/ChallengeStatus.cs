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
}