using System.Diagnostics.Tracing;
using System.Runtime.Serialization;

[DataContract]
public abstract class ScheduledEvent : logClass<ScheduledEvent>, InterfaceEventType
{
    public ulong TimeToExecuteTheEventOn
    {
        get => timeToExecuteTheEventOn.GetValue();
        set => timeToExecuteTheEventOn.SetValue(value);
    }

    public int EventId
    {
        get => eventId.GetValue();
        set => eventId.SetValue(value);
    }

    public bool EventIsBeingExecuted
    {
        get => eventIsBeingExecuted.GetValue();
        set => eventIsBeingExecuted.SetValue(value);
    }

    public ulong LeagueCategoryIdCached
    {
        get => leagueCategoryIdCached.GetValue();
        set => leagueCategoryIdCached.SetValue(value);
    }

    public ulong MatchChannelIdCached
    {
        get => matchChannelIdCached.GetValue();
        set => matchChannelIdCached.SetValue(value);
    }

    [DataMember] protected logClass<ulong> timeToExecuteTheEventOn = new logClass<ulong>();
    [DataMember] protected logClass<int> eventId = new logClass<int>();
    [DataMember] protected logClass<bool> eventIsBeingExecuted = new logClass<bool>();
    [DataMember] protected logClass<ulong> leagueCategoryIdCached = new logClass<ulong>();
    [DataMember] protected logClass<ulong> matchChannelIdCached = new logClass<ulong>();

    public ScheduledEvent() { }

    protected void SetupScheduledEvent(int _timeFromNowToExecuteOn)
    {
        Log.WriteLine("Setting " + typeof(ScheduledEvent) + "' TimeToExecuteTheEventOn: " +
            _timeFromNowToExecuteOn + " seconds from now", LogLevel.VERBOSE);

        ulong currentUnixTime = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();
        TimeToExecuteTheEventOn = currentUnixTime + (ulong)_timeFromNowToExecuteOn;
        EventId = ++Database.Instance.EventScheduler.EventCounter;

        Database.Instance.EventScheduler.ScheduledEvents.Add(this);

        Log.WriteLine(typeof(ScheduledEvent) + "' TimeToExecuteTheEventOn is now: " +
            TimeToExecuteTheEventOn + " with id event: " + EventId, LogLevel.VERBOSE);
    }

    public abstract Task ExecuteTheScheduledEvent(bool _serialize = true);
    public abstract void CheckTheScheduledEventStatus();
}