using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class TempQueueEvent : ScheduledEvent, InterfaceEventType
{
    MatchChannelComponents mcc;

    public ulong PlayerIdCached
    {
        get => playerIdCached.GetValue();
        set => playerIdCached.SetValue(value);
    }

    public TempQueueEvent() { }

    public TempQueueEvent(ulong _leagueCategoryId, ulong _matchChannelId, ulong _playerId,
        ConcurrentBag<ScheduledEvent> _scheduledEvents)
    {
        Log.WriteLine("Creating event: " + nameof(TempQueueEvent) + "|" +
            _leagueCategoryId + "|" + _matchChannelId);

        base.SetupScheduledEvent(300, _scheduledEvents);
        LeagueCategoryIdCached = _leagueCategoryId;
        MatchChannelIdCached = _matchChannelId;
        PlayerIdCached = _playerId;

        Log.WriteLine("Done creating event: " + EventId + " type of: " + nameof(TempQueueEvent) + " with: " +
            "|" + _leagueCategoryId + "|" + _matchChannelId, LogLevel.DEBUG);
    }

    [DataMember] private logVar<ulong> playerIdCached = new logVar<ulong>();

    public override async Task ExecuteTheScheduledEvent(bool _serialize = true)
    {
        mcc = new MatchChannelComponents(MatchChannelIdCached);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(mcc) + " was null!");
        }

        var matchReportData = mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData;
        foreach (var teamKvp in matchReportData)
        {
            PLAYERPLANE? playerPlane = teamKvp.Value.FindBaseReportingObjectOfType(TypeOfTheReportingObject.PLAYERPLANE) as PLAYERPLANE;
            if (playerPlane == null)
            {
                Log.WriteLine(nameof(playerPlane) + " was null!", LogLevel.CRITICAL);
                throw new InvalidOperationException(nameof(playerPlane) + " was null!");
            }

            if (!playerPlane.TeamMemberIdsWithSelectedPlanesByTheTeam.ContainsKey(PlayerIdCached))
            {
                //Log.WriteLine("Did not contain: " + PlayerIdCached, LogLevel.CRITICAL);
                continue;
            }

            playerPlane.TeamMemberIdsWithSelectedPlanesByTheTeam[PlayerIdCached] = UnitName.NOTSELECTED;
        }

        if (!_serialize) return;

        await SerializationManager.SerializeDB();
        Log.WriteLine("event: " + EventId + " after serialization");
    }

    public override void CheckTheScheduledEventStatus()
    {
        mcc = new MatchChannelComponents(MatchChannelIdCached);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            return;
        }

        try
        {
            DiscordBotDatabase.Instance.Categories.FindInterfaceCategoryWithCategoryId(
                LeagueCategoryIdCached).FindInterfaceChannelWithIdInTheCategory(
                    MatchChannelIdCached).FindInterfaceMessageWithNameInTheChannelAndUpdateItIfItExists(
                        MessageName.CONFIRMMATCHENTRYMESSAGE);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }
    }
}