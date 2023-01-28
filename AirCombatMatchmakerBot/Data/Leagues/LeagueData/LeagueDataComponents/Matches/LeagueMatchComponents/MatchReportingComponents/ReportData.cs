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

    public ReportObject ReportedScore
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

    public ReportObject TacviewLink
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

    public ReportObject CommentByTheUser
    {
        get
        {
            Log.WriteLine("Getting " + nameof(commentByTheUser), LogLevel.VERBOSE);
            return commentByTheUser;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(commentByTheUser)
                + " to: " + value, LogLevel.VERBOSE);
            commentByTheUser = value;
        }
    }

    [DataMember] private string teamName { get; set; }
    [DataMember] private ReportObject reportedScore { get; set; }
    [DataMember] private ReportObject tacviewLink { get; set; }
    [DataMember] private ReportObject commentByTheUser { get; set; }

    public ReportData(string _reportingTeamName)
    {
        teamName = _reportingTeamName;      
        reportedScore = new ReportObject("Reported score", true);
        tacviewLink = new ReportObject("Tacview link", true);
        commentByTheUser = new ReportObject("Comment", false);
    }
}