using System.Runtime.Serialization;

[DataContract]
public class LeagueMatchSchedulerEvent : ScheduledEvent
{
    LeagueCategoryComponents lcc;
    public LeagueMatchSchedulerEvent() { }

    public LeagueMatchSchedulerEvent(
        int _timeFromNowToExecuteOn, ulong _leagueCategoryId)
    {
        Log.WriteLine("Creating event: " + nameof(DeleteChannelEvent) + " with: " + _timeFromNowToExecuteOn + "|" +
            _leagueCategoryId, LogLevel.VERBOSE);

        base.SetupScheduledEvent(_timeFromNowToExecuteOn);
        LeagueCategoryIdCached = _leagueCategoryId;

        Log.WriteLine("Done creating event: " + nameof(DeleteChannelEvent) + " with: " + _timeFromNowToExecuteOn + "|" +
            _leagueCategoryId, LogLevel.DEBUG);
    }

    public override async Task ExecuteTheScheduledEvent(bool _serialize = true)
    {
        ulong categoryId = LeagueCategoryIdCached;

        Log.WriteLine("Starting to execute event: " + EventId + " named " + nameof(DeleteChannelEvent) + " with: " +
            categoryId, LogLevel.WARNING);

        lcc = new LeagueCategoryComponents(LeagueCategoryIdCached);
        if (lcc.interfaceLeagueCached == null)
        {
            Log.WriteLine(nameof(lcc) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(lcc) + " was null!");
        }

        /* NOT IMPLEMENTED YET
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
                //Log.WriteLine("Finished an event without deleting the channel, because it didn't exist!", LogLevel.WARNING);
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

        await SerializationManager.SerializeDB();*/
    }

    public override void CheckTheScheduledEventStatus()
    {
        try
        {
            /*
            InterfaceMessage confirmationMessage =
                Database.Instance.Categories.FindInterfaceCategoryWithId(
                    LeagueCategoryIdCached).FindInterfaceChannelWithIdInTheCategory(
                        MatchChannelIdCached).FindInterfaceMessageWithNameInTheChannel(
                            MessageName.CONFIRMATIONMESSAGE);

            Log.WriteLine("Found: " + confirmationMessage.MessageId + " with content: " +
                confirmationMessage.MessageDescription, LogLevel.DEBUG);

            //var timeLeft = TimeToExecuteTheEventOn - (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();

            confirmationMessage.GenerateAndModifyTheMessage();*/
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }
    }
}