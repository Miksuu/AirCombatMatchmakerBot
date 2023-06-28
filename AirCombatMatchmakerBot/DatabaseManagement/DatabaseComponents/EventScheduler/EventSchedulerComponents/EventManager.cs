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

    public ScheduledEvent GetEventByType(Type _eventType)
    {
        try
        {
            if (ClassScheduledEvents == null)
            {
                Log.WriteLine("ClassScheduledEvents is null.", LogLevel.CRITICAL);
                throw new InvalidOperationException("ClassScheduledEvents is null.");
            }

            if (_eventType == null)
            {
                Log.WriteLine("eventType is null.", LogLevel.CRITICAL);
                throw new ArgumentNullException(nameof(_eventType));
            }

            if (!typeof(ScheduledEvent).IsAssignableFrom(_eventType))
            {
                Log.WriteLine("eventType is not a ScheduledEvent.", LogLevel.CRITICAL);
                throw new ArgumentException("eventType is not a ScheduledEvent.");
            }

            ScheduledEvent? eventOfType = ClassScheduledEvents.FirstOrDefault(e => e.GetType() == _eventType);
            if (eventOfType == null)
            {
                Log.WriteLine($"Event of type {_eventType.Name} does not exist in ClassScheduledEvents.", LogLevel.CRITICAL);
                throw new InvalidOperationException($"Event of type {_eventType.Name} does not exist in ClassScheduledEvents.");
            }

            return eventOfType;
        }
        catch (Exception ex)
        {
            Log.WriteLine("Error in GetEventByType: " + ex.Message, LogLevel.ERROR);
            throw; // Re-throw the exception to preserve the original exception stack trace
        }
    }

    public List<ScheduledEvent> GetListOfEventsByType(Type _eventType)
    {
        try
        {
            if (ClassScheduledEvents == null)
            {
                Log.WriteLine("ClassScheduledEvents is null.", LogLevel.CRITICAL);
                throw new InvalidOperationException("ClassScheduledEvents is null.");
            }

            if (_eventType == null)
            {
                Log.WriteLine("eventType is null.", LogLevel.CRITICAL);
                throw new ArgumentNullException(nameof(_eventType));
            }

            if (!typeof(ScheduledEvent).IsAssignableFrom(_eventType))
            {
                Log.WriteLine("eventType is not a ScheduledEvent.", LogLevel.CRITICAL);
                throw new ArgumentException("eventType is not a ScheduledEvent.");
            }

            List<ScheduledEvent> eventsOfType = ClassScheduledEvents.Where(e => e.GetType() == _eventType).ToList();

            if (eventsOfType.Count == 0)
            {
                Log.WriteLine($"No events of type {_eventType.Name} exist in ClassScheduledEvents.", LogLevel.CRITICAL);
                throw new InvalidOperationException($"No events of type {_eventType.Name} exist in ClassScheduledEvents.");
            }

            return eventsOfType;
        }
        catch (Exception ex)
        {
            Log.WriteLine("Error in GetEventsByType: " + ex.Message, LogLevel.ERROR);
            throw; // Re-throw the exception to preserve the original exception stack trace
        }
    }


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
                Log.WriteLine("event: " + item.EventId + " scheduledEventsToRemove: " + item.EventId);
            }

            RemoveEventsFromTheScheduledEventsBag(scheduledEventsToRemove);
        }
    }

    // Refactor this to take in single event id instead of list
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

    public void ClearCertainTypeOfEventsFromTheList(Type _type)
    {
        try
        {
            if (ClassScheduledEvents == null)
            {
                Log.WriteLine("ClassScheduledEvents is null.", LogLevel.CRITICAL);
                throw new InvalidOperationException("ClassScheduledEvents is null.");
            }

            List<ScheduledEvent> events = ClassScheduledEvents
                .Where(e => e.GetType() == _type)
                .ToList();

            foreach (ScheduledEvent @event in events)
            {
                ScheduledEvent eventToRemove = @event; // Create a temporary variable
                ClassScheduledEvents.TryTake(out eventToRemove);
            }

            Log.WriteLine($"{events.Count} {_type.Name}(s) removed from ClassScheduledEvents.");
        }
        catch (Exception ex)
        {
            Log.WriteLine("Error in ClearCertainTypeOfEventsFromTheList: " + ex.Message, LogLevel.ERROR);
            throw; // Re-throw the exception to preserve the original exception stack trace
        }
    }
}