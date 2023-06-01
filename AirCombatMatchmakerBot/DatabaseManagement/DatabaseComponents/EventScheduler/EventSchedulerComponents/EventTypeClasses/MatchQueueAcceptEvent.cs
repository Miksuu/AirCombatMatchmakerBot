using System.Runtime.Serialization;

[DataContract]
public class MatchQueueAcceptEvent : ScheduledEvent, InterfaceLoggableClass, InterfaceEventType
{
    MatchChannelComponents mcc;
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

    public MatchQueueAcceptEvent() { }

    public MatchQueueAcceptEvent(
        int _timeFromNowToExecuteOn, ulong _leagueCategoryIdCached, ulong _matchChannelIdCached)
    {
        Log.WriteLine("Creating event: " + nameof(DeleteChannelEvent) + " with: " + _timeFromNowToExecuteOn + "|" +
            _leagueCategoryIdCached + "|" + _matchChannelIdCached, LogLevel.VERBOSE);

        base.SetupScheduledEvent(_timeFromNowToExecuteOn);
        LeagueCategoryIdCached = _leagueCategoryIdCached;
        MatchChannelIdCached = _matchChannelIdCached;

        Log.WriteLine("Done creating event: " + nameof(DeleteChannelEvent) + " with: " + 
            _timeFromNowToExecuteOn + "|" + _leagueCategoryIdCached + "|" + _matchChannelIdCached, LogLevel.DEBUG);
    }

    public override async void ExecuteTheScheduledEvent()
    {
        mcc = new MatchChannelComponents(LeagueCategoryIdCached, MatchChannelIdCached);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            return;
        }

        ulong matchChannelId = mcc.leagueMatchCached.MatchChannelId;

        // Create the event and execute it instantly
        var newEvent = new DeleteChannelEvent(0, mcc.interfaceLeagueCached.LeagueCategoryId, matchChannelId, "match");
        newEvent.ExecuteTheScheduledEvent();

        mcc.interfaceLeagueCached.LeagueData.Matches.FindMatchAndRemoveItFromConcurrentBag(
            mcc.interfaceLeagueCached, matchChannelId);

        await SerializationManager.SerializeDB();
    }

    public async override void CheckTheScheduledEventStatus()
    {
        mcc = new MatchChannelComponents(LeagueCategoryIdCached, MatchChannelIdCached);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            return;
        }

        try
        {
            InterfaceMessage confirmMatchEntryMessage =
                Database.Instance.Categories.FindInterfaceCategoryWithId(
                    LeagueCategoryIdCached).FindInterfaceChannelWithIdInTheCategory(
                        MatchChannelIdCached).FindInterfaceMessageWithNameInTheChannel(
                            MessageName.CONFIRMMATCHENTRYMESSAGE);

            Log.WriteLine("Found: " + confirmMatchEntryMessage.MessageId + " with content: " +
                confirmMatchEntryMessage.MessageDescription, LogLevel.DEBUG);

            await confirmMatchEntryMessage.GenerateAndModifyTheMessage();
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }
    }
}