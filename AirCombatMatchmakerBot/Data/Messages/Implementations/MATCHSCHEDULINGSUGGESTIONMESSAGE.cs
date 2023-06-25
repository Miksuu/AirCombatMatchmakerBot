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
        mentionOtherTeamsPlayers = true;
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public async override Task<string> GenerateMessage()
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

            var requestedTime = TimeService.ConvertToZuluTimeFromUnixTime(scheduleObject.RequestedSchedulingTimeInUnixTime);

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
                "**" + requestedTime + " requested by team: " + teamNameThatScheduled + "**\n\n" +
                "If the time above is fine, use ``/schedule accept`` command, or click the ACCEPT button below. " +
                "Otherwise refer to the instructions: " + message.GetJumpUrl() + " to propose a new time!";

            Log.WriteLine(thisInterfaceMessage.MessageDescription);

            return thisInterfaceMessage.MessageDescription;
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return Task.FromResult(ex.Message).Result;
        }
    }
}