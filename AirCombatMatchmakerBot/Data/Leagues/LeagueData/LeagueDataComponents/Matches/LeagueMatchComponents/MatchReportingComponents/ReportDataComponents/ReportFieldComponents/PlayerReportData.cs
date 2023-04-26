using System.Runtime.Serialization;

[DataContract]
public class PlayerReportData
{
    public string? ObjectValue
    {
        get
        {
            Log.WriteLine("Getting " + nameof(objectValue), LogLevel.GET_VERBOSE);
            return objectValue;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(objectValue)
                + " to: " + value, LogLevel.SET_VERBOSE);
            objectValue = value;
        }
    }

    public EmojiName CurrentStatus
    {
        get
        {
            Log.WriteLine("Getting " + nameof(currentStatus), LogLevel.GET_VERBOSE);
            return currentStatus;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(currentStatus)
                + " to: " + value, LogLevel.SET_VERBOSE);
            currentStatus = value;
        }
    }

    
    [DataMember] private string? objectValue { get; set; }
    [DataMember] private EmojiName currentStatus { get; set; }


    public PlayerReportData()
    {
    }

    public PlayerReportData(EmojiName _defaultStateEmoji)
    {
        currentStatus = _defaultStateEmoji;
    }

    public void SetObjectValueAndFieldBool(string _value, EmojiName _currentStatus)
    {
        Log.WriteLine("Setting " + nameof(PlayerReportData) + "'s value to: " + _value
            + " with bool: " + _currentStatus, LogLevel.VERBOSE);

        objectValue = _value;
        currentStatus = _currentStatus;
    }
}