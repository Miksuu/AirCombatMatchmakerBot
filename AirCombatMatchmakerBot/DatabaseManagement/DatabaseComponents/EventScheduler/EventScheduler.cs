using System.Collections.Concurrent;
using System.Runtime.Serialization;

public class EventScheduler<T> : logClass<EventScheduler<T>>, InterfaceLoggableClass
{
    public ConcurrentBag<ScheduledEvent<T>> ScheduledEvents
    {
        get => scheduledEvents.GetValue();
        set => scheduledEvents.SetValue(value);
    }

    [DataMember]
    private logConcurrentBag<ScheduledEvent<T>> scheduledEvents = new logConcurrentBag<ScheduledEvent<T>>();

    public List<string> GetClassParameters()
    {
        return new List<string> { scheduledEvents.GetLoggingClassParameters() };
    }


}
