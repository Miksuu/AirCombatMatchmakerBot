using System.Runtime.Serialization;

[DataContract]
public class MatchQueueAcceptEvent : ScheduledEvent, InterfaceEventType
{
    MatchChannelComponents mcc;

    public MatchQueueAcceptEvent() { }

    public MatchQueueAcceptEvent(
        ulong _timeFromNowToExecuteOn, ulong _leagueCategoryIdCached, ulong _matchChannelIdCached)
    {
        Log.WriteLine("Creating event: " + nameof(DeleteChannelEvent) + " with: " + _timeFromNowToExecuteOn + "|" +
            _leagueCategoryIdCached + "|" + _matchChannelIdCached, LogLevel.VERBOSE);

        base.SetupScheduledEvent(_timeFromNowToExecuteOn);
        LeagueCategoryIdCached = _leagueCategoryIdCached;
        MatchChannelIdCached = _matchChannelIdCached;

        Log.WriteLine("Done creating event: " + EventId + " type of: " + nameof(DeleteChannelEvent) + " with: " + 
            _timeFromNowToExecuteOn + "|" + _leagueCategoryIdCached + "|" + _matchChannelIdCached, LogLevel.DEBUG);
    }

    public override async Task ExecuteTheScheduledEvent(bool _serialize = true)
    {
        mcc = new MatchChannelComponents(MatchChannelIdCached);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(mcc) + " was null!");
        }

        Log.WriteLine("event: " + EventId + " before setting matchChannelId", LogLevel.VERBOSE);

        ulong matchChannelId = mcc.leagueMatchCached.MatchChannelId;

        Log.WriteLine("event: " + EventId + " after setting matchChannelId", LogLevel.VERBOSE);

        await mcc.interfaceLeagueCached.LeagueData.Matches.FindMatchAndRemoveItFromConcurrentBag(matchChannelId);

        Log.WriteLine("event: " + EventId + " after removed from bag with: " + matchChannelId, LogLevel.VERBOSE);

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
        Log.WriteLine("event: " + EventId + " created newEvent", LogLevel.VERBOSE);
        await newEvent.ExecuteTheScheduledEvent(false);
        Log.WriteLine("event: " + EventId + " after newEvent executed", LogLevel.VERBOSE);

        if (!_serialize) return;

        await SerializationManager.SerializeDB();
        Log.WriteLine("event: " + EventId + " after serialization", LogLevel.VERBOSE);
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
            InterfaceMessage confirmMatchEntryMessage =
                Database.Instance.Categories.FindInterfaceCategoryWithId(
                    LeagueCategoryIdCached).FindInterfaceChannelWithIdInTheCategory(
                        MatchChannelIdCached).FindInterfaceMessageWithNameInTheChannel(
                            MessageName.CONFIRMMATCHENTRYMESSAGE);

            Log.WriteLine("Found: " + confirmMatchEntryMessage.MessageId + " with content: " +
                confirmMatchEntryMessage.MessageDescription, LogLevel.DEBUG);

            confirmMatchEntryMessage.GenerateAndModifyTheMessage();
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }
    }
}