using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class LeagueMatchSchedulerEvent : ScheduledEvent
{
    LeagueCategoryComponents lcc;
    public LeagueMatchSchedulerEvent() { }

    public LeagueMatchSchedulerEvent(
        ulong _timeFromNowToExecuteOn, ulong _leagueCategoryId,
        ConcurrentBag<ScheduledEvent> _scheduledEvents)
    {
        Log.WriteLine("Creating event: " + nameof(DeleteChannelEvent) + " with: " + _timeFromNowToExecuteOn + "|" +
            _leagueCategoryId);

        base.SetupScheduledEvent(_timeFromNowToExecuteOn, _scheduledEvents);
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
    }

    public override async void CheckTheScheduledEventStatus()
    {
        try
        {
            lcc = new LeagueCategoryComponents(LeagueCategoryIdCached);
            if (lcc.interfaceLeagueCached == null)
            {
                Log.WriteLine(nameof(lcc) + " was null!", LogLevel.CRITICAL);
                throw new InvalidOperationException(nameof(lcc) + " was null!");
            }

            var channel = Database.Instance.Categories.FindInterfaceCategoryWithCategoryId(
                lcc.interfaceLeagueCached.LeagueCategoryId).FindInterfaceChannelWithNameInTheCategory(
                    ChannelType.CHALLENGE);

            if (!channel.InterfaceMessagesWithIds.Any(x => x.Value.MessageName == MessageName.CHALLENGEMESSAGE))
            {
                Log.WriteLine(nameof(MessageName.CHALLENGEMESSAGE) + " didn't exist yet.", LogLevel.DEBUG);
                return;
            }

            var challengeMessageAsInterface = channel.FindInterfaceMessageWithNameInTheChannel(MessageName.CHALLENGEMESSAGE);

            CHALLENGEMESSAGE challengeMessageClass = challengeMessageAsInterface as CHALLENGEMESSAGE;

            bool updateTheMessage = await challengeMessageClass.UpdateTeamsThatHaveMatchesClose();

            if (updateTheMessage)
            {
                challengeMessageAsInterface.GenerateAndModifyTheMessage();
            }

            lcc.interfaceLeagueCached.LeagueData.MatchScheduler.CheckCurrentStateOfTheMatchmakerAndAssignMatches();
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }
    }
}