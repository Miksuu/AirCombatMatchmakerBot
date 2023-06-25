using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class EventManager : logClass<EventManager>
{
    public ConcurrentBag<ScheduledEvent> ClassScheduledEvents
    {
        get => classScheduledEvents.GetValue();
        set => classScheduledEvents.SetValue(value);
    }

    public EventManager() { }

    [DataMember] private logConcurrentBag<ScheduledEvent> classScheduledEvents = new logConcurrentBag<ScheduledEvent>();

    public void HandleEvents(ulong _currentUnixTime)
    {
        // Replace this with looping through leagues
        foreach (ScheduledEvent scheduledEvent in ClassScheduledEvents)
        {
            bool eventCanBeRemoved = scheduledEvent.CheckIfTheEventCanBeExecuted(_currentUnixTime);

            if (!eventCanBeRemoved)
            {
                continue;
            }

            // Event succesfully executed
            var scheduledEventsToRemove = ClassScheduledEvents.Where(e => e.EventId == scheduledEvent.EventId).ToList();
            foreach (var item in scheduledEventsToRemove)
            {
                Log.WriteLine("event: " + item.EventId + " scheduledEventsToRemove: " + item.EventId, LogLevel.VERBOSE);
            }

            RemoveEventsFromTheScheduledEventsBag(scheduledEventsToRemove);
        }
    }

    public void RemoveEventsFromTheScheduledEventsBag(List<ScheduledEvent> _scheduledEventsToRemove)
    {
        var updatedScheduledEvents = new ConcurrentBag<ScheduledEvent>();

        foreach (var item in ClassScheduledEvents)
        {
            if (!_scheduledEventsToRemove.Contains(item))
            {
                updatedScheduledEvents.Add(item);
            }
        }

        ClassScheduledEvents = updatedScheduledEvents;

        foreach (var item in _scheduledEventsToRemove)
        {
            if (!ClassScheduledEvents.Contains(item))
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