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

    public async override Task<string> GenerateMessage(ulong _leagueCategoryId = 0)
    {
        try
        {
            mcc = new MatchChannelComponents(this);
            if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
            {
                string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                    nameof(mcc.leagueMatchCached) + " was null!";
                Log.WriteLine(errorMsg, LogLevel.ERROR);
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

            thisInterfaceMessage.MessageDescription +=
                "**" + requestedTime + " requested by team: " + teamNameThatScheduled + "**\n\n" +
                "If the time above is fine, use ``/schedule accept`` command, or click the ACCEPT button below. " +
                "Otherwise refer to the instructions: " + await DiscordBotDatabase.Instance.Categories.GetMessageJumpUrl(
                    mcc.interfaceLeagueCached.LeagueCategoryId, mcc.leagueMatchCached.MatchChannelId,
                    MessageName.MATCHSCHEDULINGMESSAGE) + " to propose a new time!";

            Log.WriteLine(thisInterfaceMessage.MessageDescription);

            return thisInterfaceMessage.MessageDescription;
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return Task.FromResult(ex.Message).Result;
        }
    }

    public override string GenerateMessageFooter()
    {
        return "";
        //return "Last updated at: " + DateTime.UtcNow.ToLongTimeString() + " " + DateTime.UtcNow.ToLongDateString() + " (GMT+0)";
    }
}