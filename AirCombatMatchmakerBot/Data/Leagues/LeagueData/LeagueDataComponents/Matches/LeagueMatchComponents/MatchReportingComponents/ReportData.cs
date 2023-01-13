using System.Runtime.Serialization;

[DataContract]
public class ReportData
{
    public int ReportedResult
    {
        get
        {
            Log.WriteLine("Getting " + nameof(reportedResult), LogLevel.VERBOSE);
            return reportedResult;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(reportedResult)
                + " to: " + value, LogLevel.VERBOSE);
            reportedResult = value;
        }
    }

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

    public string TacviewLink
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

    [DataMember] private int reportedResult { get; set; }
    [DataMember] private string teamName { get; set; }
    [DataMember] private string tacviewLink { get; set; }

    public ReportData(int _playerReportedResult, string _reportingTeamName)
    {
        reportedResult = _playerReportedResult;
        teamName = _reportingTeamName;
    }
}