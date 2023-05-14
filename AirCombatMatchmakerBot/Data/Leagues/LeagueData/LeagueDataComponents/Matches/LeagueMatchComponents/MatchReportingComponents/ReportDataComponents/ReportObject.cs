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
        get => currentStatus.GetValue();
        set => currentStatus.SetValue(value);
    }

    public EmojiName CachedDefaultStatus
    {
        get => cachedDefaultStatus.GetValue();
        set => cachedDefaultStatus.SetValue(value);
    }

    [DataMember] private logString fieldNameDisplay = new logString();
    [DataMember] private logString objectValue = new logString();
    [DataMember] private logClass<EmojiName> currentStatus = new logClass<EmojiName>(new EmojiName());
    [DataMember] private logClass<EmojiName> cachedDefaultStatus = new logClass<EmojiName>(new EmojiName());

    public ReportObject()
    {
    }

    public ReportObject(string _FieldNameDisplay, EmojiName _defaultStateEmoji)
    {
        FieldNameDisplay = _FieldNameDisplay;
        CachedDefaultStatus = _defaultStateEmoji;
        CurrentStatus = _defaultStateEmoji;
    }

    public void SetObjectValueAndFieldBool(string _value, EmojiName _currentStatus)
    {
        Log.WriteLine("Setting " + nameof(ReportObject) + "'s value to: " + _value
            + " with bool: " + _currentStatus, LogLevel.VERBOSE);

        ObjectValue = _value;
        CurrentStatus = _currentStatus;
    }
}