using Discord.WebSocket;
using System.Runtime.Serialization;

[DataContract]
public class MatchReporting
{
    public Dictionary<int, int> TeamIdWithReportedResult
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamIdWithReportedResult) + " with count of: " +
                teamIdWithReportedResult.Count, LogLevel.VERBOSE);
            return teamIdWithReportedResult;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamIdWithReportedResult)
                + " to: " + value, LogLevel.VERBOSE);
            teamIdWithReportedResult = value;
        }
    }

    [DataMember] private Dictionary<int, int> teamIdWithReportedResult { get; set; }

    public MatchReporting()
    {
        teamIdWithReportedResult = new Dictionary<int, int>();
    }
}