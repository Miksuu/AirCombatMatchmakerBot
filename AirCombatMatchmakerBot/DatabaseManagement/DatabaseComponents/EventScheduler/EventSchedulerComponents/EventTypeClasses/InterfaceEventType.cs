public interface InterfaceEventType
{
    public abstract Task ExecuteTheScheduledEvent(bool _serialize = true, int _eventIdFrom = 0);
    public abstract void CheckTheScheduledEventStatus();
}