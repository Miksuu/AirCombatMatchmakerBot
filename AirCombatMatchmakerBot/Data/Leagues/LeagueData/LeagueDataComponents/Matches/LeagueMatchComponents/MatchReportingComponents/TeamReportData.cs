using System.Runtime.Serialization;

[DataContract]
public class TeamReportData
{
    public string TeamName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamName), LogLevel.GET_VERBOSE);
            return teamName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamName)
                + " to: " + value, LogLevel.SET_VERBOSE);
            teamName = value;
        }
    }

    public ReportField ReportedScore
    {
        get
        {
            Log.WriteLine("Getting " + nameof(reportedScore), LogLevel.GET_VERBOSE);
            return reportedScore;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(reportedScore)
                + " to: " + value, LogLevel.SET_VERBOSE);
            reportedScore = value;
        }
    }

    public ReportField TacviewLink
    {
        get
        {
            Log.WriteLine("Getting " + nameof(tacviewLink), LogLevel.GET_VERBOSE);
            return tacviewLink;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(tacviewLink)
                + " to: " + value, LogLevel.SET_VERBOSE);
            tacviewLink = value;
        }
    }

    public ReportField CommentsByTheTeamMembers
    {
        get
        {
            Log.WriteLine("Getting " + nameof(commentsByTheTeamMembers), LogLevel.GET_VERBOSE);
            return commentsByTheTeamMembers;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(commentsByTheTeamMembers)
                + " to: " + value, LogLevel.SET_VERBOSE);
            commentsByTheTeamMembers = value;
        }
    }

    public ReportField SelectedUnitsByTheTeamMembers
    {
        get
        {
            Log.WriteLine("Getting " + nameof(selectedUnitsByTheTeamMembers), LogLevel.GET_VERBOSE);
            return selectedUnitsByTheTeamMembers;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(selectedUnitsByTheTeamMembers)
                + " to: " + value, LogLevel.SET_VERBOSE);
            selectedUnitsByTheTeamMembers = value;
        }
    }

    public float FinalEloDelta
    {
        get
        {
            Log.WriteLine("Getting " + nameof(finalEloDelta) + ": " + finalEloDelta, LogLevel.GET_VERBOSE);
            return finalEloDelta;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(finalEloDelta)
                + " to: " + value, LogLevel.SET_VERBOSE);
            finalEloDelta = value;
        }
    }

    public bool ConfirmedMatch
    {
        get
        {
            Log.WriteLine("Getting " + nameof(confirmedMatch) + ": " + confirmedMatch, LogLevel.GET_VERBOSE);
            return confirmedMatch;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(confirmedMatch)
                + " to: " + value, LogLevel.SET_VERBOSE);
            confirmedMatch = value;
        }
    }

    [DataMember] private string teamName { get; set; }
    [DataMember] private ReportField reportedScore { get; set; }
    [DataMember] private ReportField tacviewLink { get; set; }
    [DataMember] private ReportField commentsByTheTeamMembers { get; set; }
    [DataMember] private ReportField selectedUnitsByTheTeamMembers { get; set; }
    [DataMember] private float finalEloDelta { get; set; }
    [DataMember] private bool confirmedMatch { get; set; }

    public TeamReportData(string _reportingTeamName)
    {
        teamName = _reportingTeamName;      
        reportedScore = new ReportField("Reported score", EmojiName.REDSQUARE, true);
        tacviewLink = new ReportField("Tacview link", EmojiName.REDSQUARE, true);
        commentsByTheTeamMembers = new ReportField("Comment", EmojiName.YELLOWSQUARE, false);
        selectedUnitsByTheTeamMembers = new ReportField("Selected plane", EmojiName.REDSQUARE, false);
    }
}