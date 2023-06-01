using System.Runtime.Serialization;

[DataContract]
public class DeleteChannelEvent : ScheduledEvent, InterfaceLoggableClass
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

    public string NameMustContain
    {
        get => nameMustContain.GetValue();
        set => nameMustContain.SetValue(value);
    }

    [DataMember] private logClass<ulong> categoryIdToDeleteChannelOn = new logClass<ulong>();
    [DataMember] private logClass<ulong> channelIdToDelete = new logClass<ulong>();
    [DataMember] private logString nameMustContain = new logString();

    public List<string> GetClassParameters()
    {
        return new List<string> {
            timeToExecuteTheEventOn.GetParameter(), eventId.GetParameter(),

            categoryIdToDeleteChannelOn.GetParameter(), channelIdToDelete.GetParameter(),
            nameMustContain.GetValue() };
    }

    public DeleteChannelEvent() { }

    public DeleteChannelEvent(
        int _timeFromNowToExecuteOn, ulong _categoryIdToDeleteChannelOn, ulong _channelIdToDelete, string _nameMustContain)
    {
        Log.WriteLine("Creating event: " + nameof(DeleteChannelEvent) + " with: " + _timeFromNowToExecuteOn + "|" +
            _categoryIdToDeleteChannelOn + "|" + _channelIdToDelete + "|" + _nameMustContain, LogLevel.VERBOSE);

        base.SetupScheduledEvent(_timeFromNowToExecuteOn);
        CategoryIdToDeleteChannelOn = _categoryIdToDeleteChannelOn;
        ChannelIdToDelete = _channelIdToDelete;
        if (_nameMustContain == "")
        {
            Log.WriteLine("nameMustContain was empty!", LogLevel.ERROR);
            _nameMustContain = "WontDelete";
        }
        NameMustContain = _nameMustContain;

        Log.WriteLine("Done creating event: " + nameof(DeleteChannelEvent) + " with: " + _timeFromNowToExecuteOn + "|" +
            _categoryIdToDeleteChannelOn + "|" + _channelIdToDelete + "|" + _nameMustContain, LogLevel.DEBUG);
    }

    public override async Task ExecuteTheScheduledEvent()
    {
        ulong categoryId = CategoryIdToDeleteChannelOn;
        ulong channelId = ChannelIdToDelete;
        string nameMustContain = NameMustContain;

        Log.WriteLine("Starting to execute event: " + nameof(DeleteChannelEvent) + " with: " +
            categoryId + "|" + channelId + "|" + nameMustContain, LogLevel.VERBOSE);

        InterfaceCategory interfaceCategory;

        try
        {
            interfaceCategory =
                Database.Instance.Categories.FindInterfaceCategoryWithId(categoryId);
        }
        catch(Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }

        if (interfaceCategory.FindIfInterfaceChannelExistsWithIdInTheCategory(channelId))
        {      
            InterfaceChannel interfaceChannel;
            try
            {
                interfaceChannel = interfaceCategory.FindInterfaceChannelWithIdInTheCategory(channelId);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                return;
            }

            await interfaceChannel.DeleteThisChannel(interfaceCategory, interfaceChannel, nameMustContain);
        }
        else
        {
            Log.WriteLine("Finished an event without deleting the channel, because it didn't exist!", LogLevel.WARNING);
        }

        Log.WriteLine("Done executing event: " + nameof(DeleteChannelEvent) + " with: " +
            categoryId + "|" + channelId + "|" + nameMustContain, LogLevel.DEBUG);

        await SerializationManager.SerializeDB();
    }

    public override void CheckTheScheduledEventStatus()
    {
        //throw new NotImplementedException();
    }
}