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
            "Adding Team: " + _Team + " (" + _Team.teamId + ") to the queue", LogLevel.VERBOSE);
        TeamsInTheQueue.Add(_Team);
        Log.WriteLine("Done adding the team to the queue. Count is now: " + TeamsInTheQueue.Count, LogLevel.VERBOSE);
    }

    public List<Team> GetListOfTeamsInTheQueue()
    {
        Log.WriteLine("Getting list of Teams in queue with count of: " + TeamsInTheQueue.Count, LogLevel.VERBOSE);
        return TeamsInTheQueue;
    }
}