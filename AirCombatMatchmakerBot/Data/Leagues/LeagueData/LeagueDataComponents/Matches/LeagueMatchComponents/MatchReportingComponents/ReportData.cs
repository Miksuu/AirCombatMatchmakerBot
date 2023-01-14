using System.Runtime.Serialization;

[DataContract]
public class ReportData
{
    public string TeamName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamName), LogLevel.VERBOSE);
            return teamName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamName)
                + " to: " + value, LogLevel.VERBOSE);
            teamName = value;
        }
    }

    public (int, bool) ReportedScore
    {
        get
        {
            Log.WriteLine("Getting " + nameof(reportedScore), LogLevel.VERBOSE);
            return reportedScore;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(reportedScore)
                + " to: " + value, LogLevel.VERBOSE);
            reportedScore = value;
        }
    }

    public (string, bool) TacviewLink
    {
        get
        {
            Log.WriteLine("Getting " + nameof(tacviewLink), LogLevel.VERBOSE);
            return tacviewLink;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(tacviewLink)
                + " to: " + value, LogLevel.VERBOSE);
            tacviewLink = value;
        }
    }

    [DataMember] private string teamName { get; set; }
    [DataMember] private (int, bool) reportedScore { get; set; }
    [DataMember] private (string, bool) tacviewLink { get; set; }
    [DataMember] private (string, bool) commentByTheUser { get; set; }

    public ReportData(string _reportingTeamName)
    {
        teamName = _reportingTeamName;
    }
}