using System.Runtime.Serialization;

[DataContract]
public class ReportField
{
    public string? FieldNameDisplay
    {
        get
        {
            Log.WriteLine("Getting " + nameof(fieldNameDisplay), LogLevel.GET_VERBOSE);
            return fieldNameDisplay;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(fieldNameDisplay)
                + " to: " + value, LogLevel.SET_VERBOSE);
            fieldNameDisplay = value;
        }
    }

    public EmojiName CachedDefaultStatus
    {
        get
        {
            Log.WriteLine("Getting " + nameof(cachedDefaultStatus), LogLevel.GET_VERBOSE);
            return cachedDefaultStatus;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(cachedDefaultStatus)
                + " to: " + value, LogLevel.SET_VERBOSE);
            cachedDefaultStatus = value;
        }
    }

    public Dictionary<ulong, PlayerReportData> PlayerReportDatas
    {
        get
        {
            Log.WriteLine("Getting " + nameof(playerReportDatas) +
                " with count: " + playerReportDatas.Count, LogLevel.GET_VERBOSE);
            return playerReportDatas;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(playerReportDatas)
                + " to: " + value + " with count: " + playerReportDatas.Count, LogLevel.SET_VERBOSE);
            playerReportDatas = value;
        }
    }

    public bool IsTeamSpecific
    {
        get
        {
            Log.WriteLine("Getting " + nameof(isTeamSpecific), LogLevel.GET_VERBOSE);
            return isTeamSpecific;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(isTeamSpecific)
                + " to: " + value, LogLevel.SET_VERBOSE);
            isTeamSpecific = value;
        }
    }

    [DataMember] private string? fieldNameDisplay { get; set; }
    [DataMember] private EmojiName cachedDefaultStatus { get; set; }

    [DataMember] private Dictionary<ulong, PlayerReportData> playerReportDatas { get; set; }
    [DataMember] private bool isTeamSpecific { get; set; }

    public ReportField()
    {

    }

    public ReportField(string _fieldNameDisplay, EmojiName _cachedDefaultStatus, bool _isTeamSpecific)
    {
        fieldNameDisplay = _fieldNameDisplay;
        cachedDefaultStatus = _cachedDefaultStatus;
        IsTeamSpecific = _isTeamSpecific;
    }
}