using System.Runtime.Serialization;

[DataContract]
public class DeleteChannelEvent : logClass<DeleteChannelEvent>, InterfaceLoggableClass, InterfaceEventType
{
    public ulong CategoryIdToDeleteChannelOn
    {
        get => categoryIdToDeleteChannelOn.GetValue();
        set => categoryIdToDeleteChannelOn.SetValue(value);
    }

    public ulong ChannelIdToDelete
    {
        get => channelIdToDelete.GetValue();
        set => channelIdToDelete.SetValue(value);
    }

    [DataMember] private logClass<ulong> categoryIdToDeleteChannelOn = new logClass<ulong>();
    [DataMember] private logClass<ulong> channelIdToDelete = new logClass<ulong>();

    public List<string> GetClassParameters()
    {
        return new List<string> { categoryIdToDeleteChannelOn.GetParameter(), channelIdToDelete.GetParameter() };
    }

    public void ExecuteTheScheduledEvent()
    {
        Log.WriteLine("Starting to execute event: " + nameof(DeleteChannelEvent) + " with: " +
            categoryIdToDeleteChannelOn + "|" + channelIdToDelete, LogLevel.VERBOSE);


    }
}