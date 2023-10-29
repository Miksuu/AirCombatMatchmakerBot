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

    protected override void GenerateButtons(ComponentBuilder _component, ulong _channelCategoryId)
    {
        base.GenerateRegularButtons(_component, _channelCategoryId);
    }

    public async override Task<MessageComponents> GenerateMessage(ulong _channelCategoryId = 0)
    {
        try
        {
            mcc = new MatchChannelComponents(this);
            if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
            {
                string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                    nameof(mcc.leagueMatchCached) + " was null!";
                Log.WriteLine(errorMsg, LogLevel.ERROR);
                return new MessageComponents(errorMsg, "");
            }

            string playersToMention = mcc.leagueMatchCached.GenerateFinalMentionMessage(mcc.interfaceLeagueCached, true);

            var scheduleObject = mcc.leagueMatchCached.ScheduleObject;
            var teamsInTheMatch = mcc.leagueMatchCached.TeamsInTheMatch;
            if (!teamsInTheMatch.ContainsKey(scheduleObject.TeamIdThatRequestedScheduling))
            {
                return new MessageComponents(thisInterfaceMessage.MessageDescription, playersToMention);
            }

            var requestedTime = TimeService.ConvertToZuluTimeFromUnixTime(scheduleObject.RequestedSchedulingTimeInUnixTime);

            var teamNameThatScheduled = teamsInTheMatch.First(
                t => t.Key == scheduleObject.TeamIdThatRequestedScheduling).Value;

            thisInterfaceMessage.MessageDescription +=
                "**" + requestedTime + " requested by team: " + teamNameThatScheduled + "**\n\n" +
                "If the time above is fine, use ``/schedule accept`` command, or click the ACCEPT button below. " +
                "Otherwise refer to the instructions: " + await Database.GetInstance<DiscordBotDatabase>().Categories.GetMessageJumpUrl(
                    mcc.interfaceLeagueCached.LeagueCategoryId, mcc.leagueMatchCached.MatchChannelId,
                    MessageName.MATCHSCHEDULINGMESSAGE) + " to propose a new time!";

            return new MessageComponents(thisInterfaceMessage.MessageDescription, playersToMention);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return Task.FromResult(new MessageComponents(ex.Message, "")).Result;
        }
    }

    public override string GenerateMessageFooter()
    {
        return "";
        //return "Last updated at: " + DateTime.UtcNow.ToLongTimeString() + " " + DateTime.UtcNow.ToLongDateString() + " (GMT+0)";
    }
}