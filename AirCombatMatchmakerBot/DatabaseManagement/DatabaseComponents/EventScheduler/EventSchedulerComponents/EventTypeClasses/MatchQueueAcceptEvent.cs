using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class MatchQueueAcceptEvent : ScheduledEvent, InterfaceEventType
{
    MatchChannelComponents mcc;

    public MatchQueueAcceptEvent() { }

    public MatchQueueAcceptEvent(
        ulong _timeFromNowToExecuteOn, ulong _leagueCategoryIdCached, ulong _matchChannelIdCached,
        ConcurrentBag<ScheduledEvent> _scheduledEvents)
    {
        Log.WriteLine("Creating event: " + nameof(MatchQueueAcceptEvent) + " with: " + _timeFromNowToExecuteOn + "|" +
            _leagueCategoryIdCached + "|" + _matchChannelIdCached);

        base.SetupScheduledEvent(_timeFromNowToExecuteOn, _scheduledEvents);
        LeagueCategoryIdCached = _leagueCategoryIdCached;
        MatchChannelIdCached = _matchChannelIdCached;

        Log.WriteLine("Done creating event: " + EventId + " type of: " + nameof(MatchQueueAcceptEvent) + " with: " +
            _timeFromNowToExecuteOn + "|" + _leagueCategoryIdCached + "|" + _matchChannelIdCached, LogLevel.DEBUG);
    }

    [DataMember] private bool removedFromTheQueues = false;

    public override async Task ExecuteTheScheduledEvent(bool _serialize = true)
    {
        mcc = new MatchChannelComponents(MatchChannelIdCached);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(mcc) + " was null!");
        }

        Log.WriteLine("event: " + EventId + " before setting matchChannelId");

        ulong matchChannelId = mcc.leagueMatchCached.MatchChannelId;

        Log.WriteLine("event: " + EventId + " after setting matchChannelId");

        await mcc.interfaceLeagueCached.LeagueData.Matches.FindMatchAndRemoveItFromConcurrentBag(matchChannelId);

        Log.WriteLine("event: " + EventId + " after removed from bag with: " + matchChannelId);

        // Loop through the ReportData's and put players back to queue who accepted
        // Later on, add restrictions to players who didn't accept
        var matchReportData = mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData;
        foreach (var teamKvp in matchReportData)
        {
            // Temporary solution, perhaps add enum when implementing penalties for not accepting the queue
            bool addTeamBackToTheQueue = false;
            ulong playerIdToAddBackInToTheQueue = 0;

            PLAYERPLANE? teamPlane = teamKvp.Value.FindBaseReportingObjectOfType(TypeOfTheReportingObject.PLAYERPLANE) as PLAYERPLANE;
            if (teamPlane == null)
            {
                Log.WriteLine(nameof(teamPlane) + " was null!", LogLevel.CRITICAL);
                throw new InvalidOperationException(nameof(teamPlane) + " was null!");
            }

            foreach (var teamMemberKvp in teamPlane.TeamMemberIdsWithSelectedPlanesByTheTeam)
            {
                // Add the player back to the queue
                if (teamMemberKvp.Value != UnitName.NOTSELECTED)
                {
                    addTeamBackToTheQueue = true;
                    playerIdToAddBackInToTheQueue = teamMemberKvp.Key;
                }
                // Add restrictions to the players who didn't accept the queue
                else
                {

                }
            }

            if (addTeamBackToTheQueue)
            {
                InterfaceMessage interfaceMessage = Database.Instance.Categories.FindInterfaceCategoryWithId(
                    mcc.interfaceLeagueCached.LeagueCategoryId).FindInterfaceChannelWithNameInTheCategory(
                        ChannelType.CHALLENGE).FindInterfaceMessageWithNameInTheChannel(MessageName.CHALLENGEMESSAGE);

                mcc.interfaceLeagueCached.LeagueData.ChallengeStatus.AddTeamFromPlayerIdToTheQueue(
                    playerIdToAddBackInToTheQueue, interfaceMessage);
            }
        }

        mcc.leagueMatchCached.AttemptToPutTheTeamsBackToTheQueueAfterTheMatch();

        // Create the event and execute it instantly
        var newEvent = new DeleteChannelEvent(mcc.interfaceLeagueCached.LeagueCategoryId, matchChannelId, "match");
        Log.WriteLine("event: " + EventId + " created newEvent");
        await newEvent.ExecuteTheScheduledEvent(false);
        Log.WriteLine("event: " + EventId + " after newEvent executed");

        if (!_serialize) return;

        await SerializationManager.SerializeDB();
        Log.WriteLine("event: " + EventId + " after serialization");
    }

    public override void CheckTheScheduledEventStatus()
    {
        try
        {
            mcc = new MatchChannelComponents(MatchChannelIdCached);
            if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
            {
                Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
                return;
            }

            if (!removedFromTheQueues && TimeService.CalculateTimeUntilWithUnixTime(TimeToExecuteTheEventOn) <= 1800)
            {
                removedFromTheQueues = true;

                Database.Instance.Leagues.RemovePlayersFromQueuesOnceMatchIsCloseEnough(
                    mcc.leagueMatchCached.GetIdsOfThePlayersInTheMatchAsArray().ToList());
            }

            Database.Instance.Categories.FindInterfaceCategoryWithId(
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