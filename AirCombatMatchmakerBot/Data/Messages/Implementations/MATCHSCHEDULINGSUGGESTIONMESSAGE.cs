using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;
using System.Threading.Channels;

[DataContract]
public class MATCHSCHEDULINGSUGGESTIONMESSAGE : BaseMessage
{
    MatchChannelComponents mcc { get; set; }
    public MATCHSCHEDULINGSUGGESTIONMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.MATCHSCHEDULINGSUGGESTIONMESSAGE;

        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.ACCEPTSCHEDULEDTIME, 1),
            });

        thisInterfaceMessage.MessageEmbedTitle = "";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public async override string GenerateMessage()
    {
        try
        {
            mcc = new MatchChannelComponents(this);
            if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
            {
                string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                    nameof(mcc.leagueMatchCached) + " was null!";
                Log.WriteLine(errorMsg, LogLevel.CRITICAL);
                return errorMsg;
            }

            var scheduleObject = mcc.leagueMatchCached.ScheduleObject;
            var teamsInTheMatch = mcc.leagueMatchCached.TeamsInTheMatch;
            if (!teamsInTheMatch.ContainsKey(scheduleObject.TeamIdThatRequestedScheduling))
            {
                return thisInterfaceMessage.MessageDescription;
            }

            var requestedTime = TimeService.ConvertToDateTimeFromUnixTime(scheduleObject.RequestedSchedulingTimeInUnixTime);

            var teamNameThatScheduled = teamsInTheMatch.First(
                t => t.Key == scheduleObject.TeamIdThatRequestedScheduling).Value;

            // Refactor this?
            var messageToFind = Database.Instance.Categories.FindInterfaceCategoryWithId(
                mcc.interfaceLeagueCached.LeagueCategoryId).FindInterfaceChannelWithIdInTheCategory(
                    mcc.leagueMatchCached.MatchChannelId).FindInterfaceMessageWithNameInTheChannel(
                        MessageName.MATCHSCHEDULINGMESSAGE);
            var client = BotReference.GetClientRef();
            var channel = client.GetChannel(mcc.leagueMatchCached.MatchChannelId) as IMessageChannel;
            var message = await channel.GetMessageAsync(messageToFind.MessageId);

            thisInterfaceMessage.MessageDescription +=
                "**" + requestedTime + " requested by team: " + teamNameThatScheduled + "**\n" +
                "Instructions: " + message.GetJumpUrl();

            return Task.FromResult(thisInterfaceMessage.MessageDescription);
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return Task.FromResult(ex.Message);
        }
    }
}