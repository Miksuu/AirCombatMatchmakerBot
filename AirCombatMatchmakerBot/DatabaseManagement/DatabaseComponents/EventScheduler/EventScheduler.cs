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

    public Task CheckCurrentTimeAndExecuteScheduledEvents(bool _clearEventOnTheStartup = false)
    {
        ulong currentUnixTime = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();

        Log.WriteLine("Time: " + currentUnixTime + " with: " +
            nameof(_clearEventOnTheStartup) + ": " +_clearEventOnTheStartup, LogLevel.VERBOSE);

        // Might get caused by the daylight savings
        if (currentUnixTime < LastUnixTimeCheckedOn)
        {
            Log.WriteLine("Current unix time was smaller than last unix time that was checked on!", LogLevel.ERROR);
        }

        LastUnixTimeCheckedOn = currentUnixTime;

        foreach (ScheduledEvent scheduledEvent in ScheduledEvents)
        {
            Log.WriteLine("Loop on event: " + scheduledEvent.EventId, LogLevel.VERBOSE);

            if (currentUnixTime >= scheduledEvent.TimeToExecuteTheEventOn)
            {
                if (scheduledEvent.EventIsBeingExecuted && !_clearEventOnTheStartup)
                {
                    Log.WriteLine("Event: " + scheduledEvent.EventId + " was being executed already, continuing.", LogLevel.VERBOSE);
                    continue;
                }

                scheduledEvent.EventIsBeingExecuted = true;

                Log.WriteLine("Executing event: " + scheduledEvent.EventId, LogLevel.DEBUG);

                InterfaceEventType interfaceEventType = (InterfaceEventType)scheduledEvent;
                interfaceEventType.ExecuteTheScheduledEvent();

                var scheduledEventsToRemove = ScheduledEvents.Where(e => e.EventId == scheduledEvent.EventId).ToList();
                RemoveEventsFromTheScheduledEventsBag(scheduledEventsToRemove);

                /*
                foreach (var item in ScheduledEvents.Where(
                    e=> e.EventId == scheduledEvent.EventId))
                {
                    Log.WriteLine("Trying to remove: " + item.EventId, LogLevel.VERBOSE);

                    if (ScheduledEvents.TryTake(out ScheduledEvent? _result))
                    {
                        if (_result == null)
                        {
                            Log.WriteLine(nameof(_result) + " was null! with eventId: " + scheduledEvent.EventId, LogLevel.CRITICAL);
                            return Task.CompletedTask;
                        }
                        Log.WriteLine("Removed id: " + _result.EventId, LogLevel.DEBUG);
                    }
                    else
                    {
                        Log.WriteLine("Failed to remove: " + item.EventId, LogLevel.ERROR);
                    }
                }*/
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

    public void RemoveEventsFromTheScheduledEventsBag(List<ScheduledEvent> _scheduledEventsToRemove)
    {
        foreach (var item in _scheduledEventsToRemove)
        {
            bool removed = ScheduledEvents.TryTake(out ScheduledEvent? removedItem);
            if (removed)
            {
                if (removedItem == null)
                {
                    Log.WriteLine(nameof(removedItem) + " was null! with eventId: " + item.EventId, LogLevel.CRITICAL);
                    return;
                }
                Log.WriteLine("Removed id: " + removedItem.EventId, LogLevel.DEBUG);
            }
            else
            {
                Log.WriteLine("Failed to remove: " + item.EventId, LogLevel.ERROR);
            }
        }
    }
}