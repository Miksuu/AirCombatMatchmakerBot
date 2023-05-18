using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.Serialization;

[DataContract]
public class ReportData
{
    public string TeamName
    {
        get => teamName.GetValue();
        set => teamName.SetValue(value);
    }

    [IgnoreDataMember]
    public ConcurrentDictionary<ulong, ReportObject> TeamMemberIdsWithSelectedPlanesByTheTeam
    {
        get => teamMemberIdsWithSelectedPlanesByTheTeam.GetValue();
        set => teamMemberIdsWithSelectedPlanesByTheTeam.SetValue(value);
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

    public ReportObject CommentsByTheTeamMembers
    {
        get
        {
            Log.WriteLine("Getting " + nameof(commentsByTheTeamMembers), LogLevel.VERBOSE);
            return commentsByTheTeamMembers;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(commentsByTheTeamMembers)
                + " to: " + value, LogLevel.VERBOSE);
            commentsByTheTeamMembers = value;
        }
    }
    public float FinalEloDelta
    {
        get => finalEloDelta.GetValue();
        set => finalEloDelta.SetValue(value);
    }

    public bool ConfirmedMatch
    {
        get => confirmedMatch.GetValue();
        set => confirmedMatch.SetValue(value);
    }

    [DataMember] private logString teamName = new logString();
    [DataMember] private logConcurrentDictionary<ulong, ReportObject> teamMemberIdsWithSelectedPlanesByTheTeam = new logConcurrentDictionary<ulong, ReportObject>();
    [DataMember] private ReportObject reportedScore = new ReportObject("Reported score", EmojiName.REDSQUARE);
    [DataMember] private ReportObject tacviewLink = new ReportObject("Tacview link", EmojiName.REDSQUARE);
    [DataMember] private ReportObject commentsByTheTeamMembers = new ReportObject("Comment", EmojiName.YELLOWSQUARE);
    [DataMember] private logClass<float> finalEloDelta = new logClass<float>();
    [DataMember] private logClass<bool> confirmedMatch = new logClass<bool>();

    public ReportData(string _reportingTeamName, List<Player> _players)
    {
        TeamName = _reportingTeamName;

        foreach (var player in _players)
        {
            Log.WriteLine("Adding player: " + player.PlayerDiscordId, LogLevel.VERBOSE);
            var newReportObject = new ReportObject("Plane", EmojiName.REDSQUARE, true);
            TeamMemberIdsWithSelectedPlanesByTheTeam.TryAdd(player.PlayerDiscordId, newReportObject);
        }

        Log.WriteLine("done adding players: " + TeamMemberIdsWithSelectedPlanesByTheTeam.Count + " on team: " + _reportingTeamName, LogLevel.VERBOSE);
    }
}