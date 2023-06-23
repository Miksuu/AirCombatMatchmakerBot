using Discord;
using System.Collections.Concurrent;
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

    public bool CheckIfTheEventCanBeExecuted(
        ulong _currentUnixTime, ConcurrentBag<ScheduledEvent> _scheduledEvents, bool _clearEventOnTheStartup = false)
    {
        Log.WriteLine("Loop on event: " + EventId + " type: " + GetType() + " with times: " +
            _currentUnixTime + " >= " + TimeToExecuteTheEventOn, LogLevel.VERBOSE);

        if (_currentUnixTime >= TimeToExecuteTheEventOn)
        {
            Log.WriteLine("Attempting to execute event: " + EventId, LogLevel.VERBOSE);

            if (EventIsBeingExecuted && !_clearEventOnTheStartup)
            {
                Log.WriteLine("Event: " + EventId + " was being executed already, continuing.", LogLevel.VERBOSE);
                return false;
            }

            EventIsBeingExecuted = true;

            Log.WriteLine("Executing event: " + EventId, LogLevel.DEBUG);

            //InterfaceEventType interfaceEventType = (InterfaceEventType)scheduledEvent;
            //Log.WriteLine("event: " + EventId + " cast", LogLevel.VERBOSE);
            ExecuteTheScheduledEvent();
            Log.WriteLine("event: " + EventId + " after execute await", LogLevel.VERBOSE);

            var scheduledEventsToRemove = ScheduledEvents.Where(e => e.EventId == EventId).ToList();
            foreach (var item in scheduledEventsToRemove)
            {
                Log.WriteLine("event: " + EventId + " scheduledEventsToRemove: " + item.EventId, LogLevel.VERBOSE);
            }

            RemoveEventsFromTheScheduledEventsBag(scheduledEventsToRemove);
        }
        else if (_currentUnixTime % 5 == 0 && _currentUnixTime <= TimeToExecuteTheEventOn)
        {
            Log.WriteLine("event: " + EventId + " going to check the event status", LogLevel.VERBOSE);
            CheckTheScheduledEventStatus();
        }
        else
        {
            Log.WriteLine("event: " + EventId + " ended up in else", LogLevel.VERBOSE);
        }

        Log.WriteLine("Done with if statement on event: " + EventId + " type: " + GetType() + " with times: " +
            _currentUnixTime + " >= " + TimeToExecuteTheEventOn, LogLevel.VERBOSE);
    }

    protected void SetupScheduledEvent(ulong _timeFromNowToExecuteOn)
    {
        Log.WriteLine("Setting " + typeof(ScheduledEvent) + "' TimeToExecuteTheEventOn: " +
            _timeFromNowToExecuteOn + " seconds from now", LogLevel.VERBOSE);

        ulong currentUnixTime = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();
        TimeToExecuteTheEventOn = currentUnixTime + (ulong)_timeFromNowToExecuteOn;
        EventId = ++Database.Instance.EventScheduler.EventCounter;

        // Replace this with league of match specific ScheduledEvents list
        Database.Instance.EventScheduler.ScheduledEvents.Add(this);

        Log.WriteLine(typeof(ScheduledEvent) + "' TimeToExecuteTheEventOn is now: " +
            TimeToExecuteTheEventOn + " with id event: " + EventId, LogLevel.VERBOSE);
    }

    public abstract Task ExecuteTheScheduledEvent(bool _serialize = true);
    public abstract void CheckTheScheduledEventStatus();
}