using System.Diagnostics.Tracing;
using System.Runtime.Serialization;

[DataContract]
public abstract class ScheduledEvent : logClass, InterfaceEventType
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

    [DataMember] protected logVar<ulong> timeToExecuteTheEventOn = new logVar<ulong>();
    [DataMember] protected logVar<int> eventId = new logVar<int>();
    [DataMember] protected logVar<bool> eventIsBeingExecuted = new logVar<bool>();

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