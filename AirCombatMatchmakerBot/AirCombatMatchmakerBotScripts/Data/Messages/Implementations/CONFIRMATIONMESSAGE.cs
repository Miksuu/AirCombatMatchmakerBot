using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;
using System.Collections.Concurrent;

[DataContract]
public class CONFIRMATIONMESSAGE : BaseMessage
{
    MatchChannelComponents mcc;
    public CONFIRMATIONMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.CONFIRMATIONMESSAGE;

        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.CONFIRMMATCHRESULTBUTTON, 1),
                new KeyValuePair<ButtonName, int>(ButtonName.DISPUTEMATCHRESULTBUTTON, 1),
            });

        thisInterfaceMessage.MessageEmbedTitle = "Match confirmation";
        thisInterfaceMessage.MessageDescription = "";
        mentionMatchPlayers = true;
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _channelCategoryId)
    {
        base.GenerateRegularButtons(_component, _channelCategoryId);
    }

    public override Task<string> GenerateMessage(ulong _channelCategoryId = 0)
    {
        Log.WriteLine("Starting to generate a message for the confirmation", LogLevel.DEBUG);

        mcc = new MatchChannelComponents(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.ERROR);
            return Task.FromResult(nameof(mcc) + " was null!");
        }

        string finalMessage = "Confirmed:\n";

        var matchReportData = mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData;

        int confirmedTeamsCounter = 0;
        foreach (var teamKvp in matchReportData)
        {
            string checkmark = EnumExtensions.GetEnumMemberAttrValue(EmojiName.REDSQUARE);

            if (teamKvp.Value.ConfirmedMatch)
            {
                checkmark = EnumExtensions.GetEnumMemberAttrValue(EmojiName.WHITECHECKMARK);
                confirmedTeamsCounter++;
            }

            finalMessage += checkmark + " " + teamKvp.Value.TeamName + "\n";
        }

        finalMessage += "You can either Confirm/Dispute the result below.";

        if (confirmedTeamsCounter > 1)
        {
            if (mcc.leagueMatchCached.MatchState != MatchState.MATCHDONE)
            {
                mcc.leagueMatchCached.FinishTheMatch(mcc.interfaceLeagueCached);
            }
            else
            {
                // Move this to method
                foreach (ScheduledEvent scheduledEvent in mcc.interfaceLeagueCached.LeagueEventManager.ClassScheduledEvents)
                {
                    if (scheduledEvent.GetType() == typeof(DeleteChannelEvent))
                    {
                        if (scheduledEvent.LeagueCategoryIdCached == mcc.interfaceLeagueCached.LeagueCategoryId &&
                            scheduledEvent.MatchChannelIdCached == mcc.leagueMatchCached.MatchChannelId)
                        {
                            var timeLeft = TimeService.CalculateTimeUntilWithUnixTime(scheduledEvent.TimeToExecuteTheEventOn);// - TimeService.GetCurrentUnixTime();

                            finalMessage += "\n\n Match is done. Deleting this channel in " + timeLeft + " seconds!";
                        }
                    }
                }
            }
        }

        Log.WriteLine("Generated: " + finalMessage, LogLevel.DEBUG);

        return Task.FromResult(finalMessage);
    }

    public override string GenerateMessageFooter()
    {
        return "";
        //return "Last updated at: " + DateTime.UtcNow.ToLongTimeString() + " " + DateTime.UtcNow.ToLongDateString() + " (GMT+0)";
    }
}