using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public abstract class BaseReportingObject : InterfaceReportingObject
{
    TypeOfTheReportingObject InterfaceReportingObject.TypeOfTheReportingObject
    {
        get => typeOfTheReportingObject.GetValue();
        set => typeOfTheReportingObject.SetValue(value);
    }

    EmojiName InterfaceReportingObject.CachedDefaultStatus
    {
        get => cachedDefaultStatus.GetValue();
        set => cachedDefaultStatus.SetValue(value);
    }

    EmojiName InterfaceReportingObject.CurrentStatus
    {
        get => currentStatus.GetValue();
        set => currentStatus.SetValue(value);
    }

    bool InterfaceReportingObject.HiddenBeforeConfirmation
    {
        get => hiddenBeforeConfirmation.GetValue();
        set => hiddenBeforeConfirmation.SetValue(value);
    }

    string InterfaceReportingObject.ObjectValue
    {
        get => objectValue.GetValue();
        set => objectValue.SetValue(value);
    }

    ConcurrentBag<MatchState> InterfaceReportingObject.AllowedMatchStatesToProcessOn
    {
        get => allowedMatchStatesToProcessOn.GetValue();
        set => allowedMatchStatesToProcessOn.SetValue(value);
    }

    [DataMember] private logClass<TypeOfTheReportingObject> typeOfTheReportingObject =
        new logClass<TypeOfTheReportingObject>(new TypeOfTheReportingObject());
    [DataMember] private logClass<EmojiName> currentStatus = new logClass<EmojiName>(new EmojiName());
    [DataMember] private logClass<EmojiName> cachedDefaultStatus = new logClass<EmojiName>(new EmojiName());
    [DataMember] private logClass<bool> hiddenBeforeConfirmation = new logClass<bool>();
    [DataMember] private logString objectValue = new logString();
    logConcurrentBag<MatchState> allowedMatchStatesToProcessOn = new logConcurrentBag<MatchState>(); // This could be just a logList

    protected InterfaceReportingObject thisReportingObject;

    public BaseReportingObject()
    {
        thisReportingObject = this;
    }

    public void SetObjectValue(string _value, EmojiName _optionalEmojiName = EmojiName.WHITECHECKMARK)
    {
        Log.WriteLine("Setting " + nameof(BaseReportingObject) + "'s value to: " + _value, LogLevel.VERBOSE);

        thisReportingObject.ObjectValue = _value;
        thisReportingObject.CurrentStatus = _optionalEmojiName;
    }

    // Perhaps temp
    public TypeOfTheReportingObject GetTypeOfTheReportingObject()
    {
        return thisReportingObject.TypeOfTheReportingObject;
    }

    public abstract string ProcessTheReportingObjectAction(
        string _reportedObjectByThePlayer, ConcurrentDictionary<int, ReportData>? _TeamIdsWithReportData = null, int _reportingTeamId = 0);
    public abstract void CancelTheReportingObjectAction();
}
