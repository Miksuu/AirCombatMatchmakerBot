using Discord;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
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

    public async Task CheckCurrentTimeAndExecuteScheduledEvents(bool _clearEventOnTheStartup = false)
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
            Log.WriteLine("Loop on event: " + scheduledEvent.EventId + " type: " + scheduledEvent.GetType() + " with times: " +
                currentUnixTime + " >= " + scheduledEvent.TimeToExecuteTheEventOn, LogLevel.VERBOSE);

            if (currentUnixTime >= scheduledEvent.TimeToExecuteTheEventOn)
            {
                Log.WriteLine("Attempting to execute event: " + scheduledEvent.EventId, LogLevel.VERBOSE);

                if (scheduledEvent.EventIsBeingExecuted && !_clearEventOnTheStartup)
                {
                    Log.WriteLine("Event: " + scheduledEvent.EventId + " was being executed already, continuing.", LogLevel.VERBOSE);
                    continue;
                }

                scheduledEvent.EventIsBeingExecuted = true;

                Log.WriteLine("Executing event: " + scheduledEvent.EventId, LogLevel.DEBUG);

                InterfaceEventType interfaceEventType = (InterfaceEventType)scheduledEvent;
                Log.WriteLine("event: " + scheduledEvent.EventId + " cast", LogLevel.VERBOSE);
                await interfaceEventType.ExecuteTheScheduledEvent();
                Log.WriteLine("event: " + scheduledEvent.EventId + " after execute await", LogLevel.VERBOSE);

                var scheduledEventsToRemove = ScheduledEvents.Where(e => e.EventId == scheduledEvent.EventId).ToList();
                foreach (var item in scheduledEventsToRemove)
                {
                    Log.WriteLine("event: " + scheduledEvent.EventId + " scheduledEventsToRemove: " + item.EventId, LogLevel.VERBOSE);
                }

                RemoveEventsFromTheScheduledEventsBag(scheduledEventsToRemove);
            }
            else if (currentUnixTime % 5 == 0 && currentUnixTime <= scheduledEvent.TimeToExecuteTheEventOn)
            {
                Log.WriteLine("event: " + scheduledEvent.EventId + " going to check the event status", LogLevel.VERBOSE);
                scheduledEvent.CheckTheScheduledEventStatus();
            }
            else
            {
                Log.WriteLine("event: " + scheduledEvent.EventId + " ended up in else", LogLevel.VERBOSE);
            }

            Log.WriteLine("Done with if statement on event: " + scheduledEvent.EventId + " type: " + scheduledEvent.GetType() + " with times: " +
                currentUnixTime + " >= " + scheduledEvent.TimeToExecuteTheEventOn, LogLevel.VERBOSE);
        }
    }

    public async void EventSchedulerLoop()
    {
        int waitTimeInMs = 1000;

        Log.WriteLine("Starting to execute " + nameof(EventSchedulerLoop), LogLevel.DEBUG);

        while (true)
        {
            Log.WriteLine("Executing " + nameof(CheckCurrentTimeAndExecuteScheduledEvents), LogLevel.VERBOSE);

            await CheckCurrentTimeAndExecuteScheduledEvents();
            Log.WriteLine("Done executing " + nameof(CheckCurrentTimeAndExecuteScheduledEvents) +
                ", waiting " + waitTimeInMs + "ms", LogLevel.VERBOSE);

            Thread.Sleep(waitTimeInMs);

            Log.WriteLine("Wait done.", LogLevel.VERBOSE);
        }
    }

    public void RemoveEventsFromTheScheduledEventsBag(List<ScheduledEvent> _scheduledEventsToRemove)
    {
        var updatedScheduledEvents = new ConcurrentBag<ScheduledEvent>();

        foreach (var item in ScheduledEvents)
        {
            if (!_scheduledEventsToRemove.Contains(item))
            {
                updatedScheduledEvents.Add(item);
            }
        }

        ScheduledEvents = updatedScheduledEvents;

        foreach (var item in _scheduledEventsToRemove)
        {
            if (!ScheduledEvents.Contains(item))
            {
                Log.WriteLine("event: " + item.EventId + " removed", LogLevel.DEBUG);
            }
            else
            {
                Log.WriteLine("event: " + item.EventId + ", failed to remove", LogLevel.ERROR);
            }
        }
    }
}