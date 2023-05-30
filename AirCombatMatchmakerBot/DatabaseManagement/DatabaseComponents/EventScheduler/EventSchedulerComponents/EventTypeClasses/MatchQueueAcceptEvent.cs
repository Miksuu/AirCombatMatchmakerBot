using System.Runtime.Serialization;

[DataContract]
public class MatchQueueAcceptEvent : ScheduledEvent, InterfaceLoggableClass, InterfaceEventType
{
    MatchChannelComponents mcc = new MatchChannelComponents();
    public ulong LeagueCategoryIdCached
    {
        get => leagueCategoryIdCached.GetValue();
        set => leagueCategoryIdCached.SetValue(value);
    }

    public ulong MatchChannelIdCached
    {
        get => matchChannelIdCached.GetValue();
        set => matchChannelIdCached.SetValue(value);
    }

    [DataMember] private logClass<ulong> leagueCategoryIdCached = new logClass<ulong>();
    [DataMember] private logClass<ulong> matchChannelIdCached = new logClass<ulong>();

    public List<string> GetClassParameters()
    {
        return new List<string> {
            timeToExecuteTheEventOn.GetParameter(), eventId.GetParameter(),

            leagueCategoryIdCached.GetParameter(), matchChannelIdCached.GetParameter() };
    }

    // Perhaps need to insert mmc get here too
    public MatchQueueAcceptEvent() { }

    public MatchQueueAcceptEvent(
        int _timeFromNowToExecuteOn, ulong _leagueCategoryIdCached, ulong _matchChannelIdCached)
    {
        Log.WriteLine("Creating event: " + nameof(DeleteChannelEvent) + " with: " + _timeFromNowToExecuteOn + "|" +
            _leagueCategoryIdCached + "|" + _matchChannelIdCached, LogLevel.VERBOSE);

        mcc = new MatchChannelComponents(_leagueCategoryIdCached, _matchChannelIdCached);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            return; //Task.FromResult(new Response(nameof(mcc) + " was null!", false));
        }


        base.SetupScheduledEvent(_timeFromNowToExecuteOn);
        LeagueCategoryIdCached = _leagueCategoryIdCached;
        MatchChannelIdCached = _matchChannelIdCached;

        Log.WriteLine("Done creating event: " + nameof(DeleteChannelEvent) + " with: " + 
            _timeFromNowToExecuteOn + "|" + _leagueCategoryIdCached + "|" + _matchChannelIdCached, LogLevel.DEBUG);
    }

    public async void ExecuteTheScheduledEvent()//bool _clearEventOnTheStartup = false)
    {
        //ulong categoryId = CategoryIdToDeleteChannelOn;
        //ulong channelId = ChannelIdToDelete;
        //string nameMustContain = NameMustContain;

        /*
        Log.WriteLine("Starting to execute event: " + nameof(DeleteChannelEvent) + " with: " +
            categoryId + "|" + channelId + "|" + nameMustContain, LogLevel.VERBOSE);

        */

        InterfaceCategory interfaceCategory;

        /*
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

            //await interfaceChannel.DeleteThisChannel(interfaceCategory, interfaceChannel, nameMustContain);
        }
        else
        {
            Log.WriteLine("Finished an event without deleting the channel, because it didn't exist!", LogLevel.WARNING);
        }

        /*
        Log.WriteLine("Done executing event: " + nameof(DeleteChannelEvent) + " with: " +
            categoryId + "|" + channelId + "|" + nameMustContain, LogLevel.DEBUG);
        */

        await SerializationManager.SerializeDB();
    }
}