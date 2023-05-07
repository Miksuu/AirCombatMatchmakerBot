using System.Runtime.Serialization;

[DataContract]
public class ReportObject
{
    public string FieldNameDisplay
    {
        get => fieldNameDisplay.GetValue();
        set => fieldNameDisplay.SetValue(value);
    }

    public string ObjectValue
    {
        get => objectValue.GetValue();
        set => objectValue.SetValue(value);
    }

    public EmojiName CurrentStatus
    {
        get
        {
            Log.WriteLine("Getting " + nameof(currentStatus), LogLevel.VERBOSE);
            return currentStatus;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(currentStatus)
                + " to: " + value, LogLevel.VERBOSE);
            currentStatus = value;
        }
    }
    public EmojiName CachedDefaultStatus
    {
        get
        {
            Log.WriteLine("Getting " + nameof(cachedDefaultStatus), LogLevel.VERBOSE);
            return cachedDefaultStatus;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(cachedDefaultStatus)
                + " to: " + value, LogLevel.VERBOSE);
            cachedDefaultStatus = value;
        }
    }

    [DataMember] private logString fieldNameDisplay = new logString();
    [DataMember] private logString objectValue = new logString();
    [DataMember] private EmojiName currentStatus { get; set; }
    [DataMember] private EmojiName cachedDefaultStatus { get; set; }

    public ReportObject()
    {
    }

    public ReportObject(string _FieldNameDisplay, EmojiName _defaultStateEmoji)
    {
        FieldNameDisplay = _FieldNameDisplay;
        cachedDefaultStatus = _defaultStateEmoji;
        currentStatus = _defaultStateEmoji;
    }

    public void SetObjectValueAndFieldBool(string _value, EmojiName _currentStatus)
    {
        Log.WriteLine("Setting " + nameof(ReportObject) + "'s value to: " + _value
            + " with bool: " + _currentStatus, LogLevel.VERBOSE);

        ObjectValue = _value;
        CurrentStatus = _currentStatus;
    }
}