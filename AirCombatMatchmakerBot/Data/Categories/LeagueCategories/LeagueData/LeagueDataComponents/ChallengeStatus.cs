using System.Runtime.Serialization;

[DataContract]
public class ChallengeStatus
{
    [DataMember] private List<Team> TeamsInTheQueue { get; set; }
    public ChallengeStatus()
    {
        TeamsInTheQueue = new List<Team>();
    }

    public void AddToTeamsInTheQueue(Team _Team)
    {
        Log.WriteLine(
            "Adding Team: " + _Team + " (" + _Team.GetTeamId() + ") to the queue", LogLevel.VERBOSE);
        TeamsInTheQueue.Add(_Team);
        Log.WriteLine("Done adding the team to the queue. Count is now: " + TeamsInTheQueue.Count, LogLevel.VERBOSE);
    }

    public List<Team> GetListOfTeamsInTheQueue()
    {
        Log.WriteLine("Getting list of Teams in queue with count of: " + TeamsInTheQueue.Count, LogLevel.VERBOSE);
        return TeamsInTheQueue;
    }

    // Returns the teams in the queue as a string
    // (useful for printing, in log on the challenge channel)
    public string ReturnTeamsInTheQueueOfAChallenge(int _leagueTeamSize)
    {
        string teamsString = string.Empty;
        foreach (Team team in TeamsInTheQueue)
        {
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