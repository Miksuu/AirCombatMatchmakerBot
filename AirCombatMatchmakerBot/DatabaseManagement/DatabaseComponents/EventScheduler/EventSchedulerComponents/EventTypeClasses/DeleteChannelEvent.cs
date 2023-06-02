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

    // Event that will be executed instantly
    public DeleteChannelEvent(ulong _categoryIdToDeleteChannelOn, ulong _channelIdToDelete, string _nameMustContain)
    {
        Log.WriteLine("Creating instantly executable event: " + nameof(DeleteChannelEvent) + " with: " + "|" +
            _categoryIdToDeleteChannelOn + "|" + _channelIdToDelete + "|" + _nameMustContain, LogLevel.VERBOSE);

        //base.SetupScheduledEvent(_timeFromNowToExecuteOn);
        CategoryIdToDeleteChannelOn = _categoryIdToDeleteChannelOn;
        ChannelIdToDelete = _channelIdToDelete;
        if (_nameMustContain == "")
        {
            Log.WriteLine("nameMustContain was empty!", LogLevel.ERROR);
            _nameMustContain = "WontDelete";
        }
        NameMustContain = _nameMustContain;

        Log.WriteLine("Done creating instantly executable event: " + nameof(DeleteChannelEvent) + " with: " +
            _categoryIdToDeleteChannelOn + "|" + _channelIdToDelete + "|" + _nameMustContain, LogLevel.DEBUG);
    }

    public override async Task ExecuteTheScheduledEvent(bool _serialize = true)
    {
        ulong categoryId = CategoryIdToDeleteChannelOn;
        ulong channelId = ChannelIdToDelete;
        string nameMustContain = NameMustContain;

        Log.WriteLine("Starting to execute event: " + EventId + " named " + nameof(DeleteChannelEvent) + " with: " +
            categoryId + "|" + channelId + "|" + nameMustContain, LogLevel.VERBOSE);

        InterfaceCategory interfaceCategory;

        try
        {
            interfaceCategory =
                Database.Instance.Categories.FindInterfaceCategoryWithId(categoryId);

            Log.WriteLine("Event: " + EventId + " before " +
                nameof(interfaceCategory.FindIfInterfaceChannelExistsWithIdInTheCategory), LogLevel.VERBOSE);

            if (interfaceCategory.FindIfInterfaceChannelExistsWithIdInTheCategory(channelId))
            {
                InterfaceChannel interfaceChannel;


                Log.WriteLine("Event: " + EventId + " inside " +
                    nameof(interfaceCategory.FindIfInterfaceChannelExistsWithIdInTheCategory), LogLevel.VERBOSE);

                interfaceChannel = interfaceCategory.FindInterfaceChannelWithIdInTheCategory(channelId);


                Log.WriteLine("Event: " + EventId + " found: " + interfaceChannel.ChannelName, LogLevel.VERBOSE);
                await interfaceChannel.DeleteThisChannel(interfaceCategory, interfaceChannel, nameMustContain);
                Log.WriteLine("Event: " + EventId + " after deletion of: " + interfaceChannel.ChannelName, LogLevel.VERBOSE);
            }
            else
            {
                Log.WriteLine("Finished an event without deleting the channel, because it didn't exist!", LogLevel.WARNING);
            }
        }
        catch(Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Done executing event: " + nameof(DeleteChannelEvent) + " with: " +
            categoryId + "|" + channelId + "|" + nameMustContain, LogLevel.DEBUG);

        if (!_serialize) return;

        await SerializationManager.SerializeDB();
    }

    public override void CheckTheScheduledEventStatus()
    {
        //throw new NotImplementedException();
    }
}