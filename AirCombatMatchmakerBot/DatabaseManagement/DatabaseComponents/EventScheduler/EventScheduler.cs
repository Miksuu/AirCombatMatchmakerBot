using System.Collections.Concurrent;
using System.Runtime.Serialization;

public class EventScheduler : logClass<EventScheduler>, InterfaceLoggableClass
{
    public ConcurrentBag<ScheduledEvent> ScheduledEvents
    {
        get => scheduledEvents.GetValue();
        set => scheduledEvents.SetValue(value);
    }

    public ulong LastUnixTimeCheckedOn
    {
        get => lastUnixTimeExecutedOn.GetValue();
        set => lastUnixTimeExecutedOn.SetValue(value);
    }

    public int EventCounter
    {
        get => eventCounter.GetValue();
        set => eventCounter.SetValue(value);
    }

    [DataMember] private logConcurrentBag<ScheduledEvent> scheduledEvents = new logConcurrentBag<ScheduledEvent>();
    [DataMember] private logClass<ulong> lastUnixTimeExecutedOn = new logClass<ulong>();
    [DataMember] private logClass<int> eventCounter = new logClass<int>();

    public List<string> GetClassParameters()
    {
        return new List<string> { scheduledEvents.GetLoggingClassParameters(), lastUnixTimeExecutedOn.GetParameter(),
            eventCounter.GetParameter() };
    }

    public EventScheduler() { }

    public Task CheckCurrentTimeAndExecuteScheduledEvents()
    {
        ulong currentUnixTime = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();

        Log.WriteLine("Time: " + currentUnixTime, LogLevel.VERBOSE);

        // Might get caused by the daylight savings
        if (currentUnixTime < LastUnixTimeCheckedOn)
        {
            Log.WriteLine("Current unix time was smaller than last unix time that was checked on!", LogLevel.ERROR);
        }

        LastUnixTimeCheckedOn = currentUnixTime;

        foreach (ScheduledEvent scheduledEvent in ScheduledEvents)
        {
            Log.WriteLine("Loop on event: " + scheduledEvent.EventId, LogLevel.VERBOSE);

            if (scheduledEvent.EventIsBeingExecuted)
            {
                Log.WriteLine("Event: " + scheduledEvent.EventId + " was being executed already, continuing.", LogLevel.VERBOSE);
                continue;
            }

            if (currentUnixTime >= scheduledEvent.TimeToExecuteTheEventOn)
            {
                scheduledEvent.EventIsBeingExecuted = true;

                InterfaceEventType interfaceEventType = (InterfaceEventType)scheduledEvent;
                interfaceEventType.ExecuteTheScheduledEvent();

                foreach (var item in ScheduledEvents.Where(
                    e=> e.EventId == scheduledEvent.EventId))
                {
                    ScheduledEvents.TryTake(out ScheduledEvent? _result);
                    Log.WriteLine("Removed id: " + _result.EventId, LogLevel.DEBUG);
                }
            }
        }

        return Task.CompletedTask;
    }

    public void EventSchedulerLoop()
    {
        int waitTimeInMs = 1000;

        Log.WriteLine("Starting to execute " + nameof(EventSchedulerLoop), LogLevel.DEBUG);

        while (true)
        {
            Log.WriteLine("Executing " + nameof(CheckCurrentTimeAndExecuteScheduledEvents), LogLevel.VERBOSE);

            CheckCurrentTimeAndExecuteScheduledEvents();
            Log.WriteLine("Done executing " + nameof(CheckCurrentTimeAndExecuteScheduledEvents) +
                ", waiting " + waitTimeInMs + "ms", LogLevel.VERBOSE);

            Thread.Sleep(waitTimeInMs);

            Log.WriteLine("Wait done.", LogLevel.VERBOSE);
        }
    }
}
