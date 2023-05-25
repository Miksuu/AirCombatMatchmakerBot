﻿using System.Diagnostics.Tracing;
using System.Runtime.Serialization;

[DataContract]
public class ScheduledEvent : logClass<ScheduledEvent>
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

    [DataMember] protected logClass<ulong> timeToExecuteTheEventOn = new logClass<ulong>();
    [DataMember] protected logClass<int> eventId = new logClass<int>();
    [DataMember] protected logClass<bool> eventIsBeingExecuted = new logClass<bool>();

    public ScheduledEvent() { }

    protected void SetupScheduledEvent(int _timeFromNowToExecuteOn)
    {
        Log.WriteLine("Setting " + typeof(ScheduledEvent) + "' TimeToExecuteTheEventOn: " +
            _timeFromNowToExecuteOn + " seconds from now", LogLevel.VERBOSE);

        ulong currentUnixTime = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();
        TimeToExecuteTheEventOn = currentUnixTime + (ulong)_timeFromNowToExecuteOn;
        EventId = ++Database.Instance.EventScheduler.EventCounter;

        Log.WriteLine(typeof(ScheduledEvent) + "' TimeToExecuteTheEventOn is now: " +
            TimeToExecuteTheEventOn + " with id: " + EventId, LogLevel.VERBOSE);
    }
}