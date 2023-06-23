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

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override Task<string> GenerateMessage()
    {
        Log.WriteLine("Starting to generate a message for the confirmation", LogLevel.DEBUG);

        mcc = new MatchChannelComponents(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
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
            if (mcc.leagueMatchCached.MatchReporting.MatchState != MatchState.MATCHDONE)
            {
                mcc.leagueMatchCached.FinishTheMatch();
            }
            else
            {
                // Move this to method
                foreach (ScheduledEvent scheduledEvent in mcc.leagueMatchCached.MatchEventManager.ClassScheduledEvents)
                {
                    if (scheduledEvent.GetType() == typeof(DeleteChannelEvent))
                    {
                        if (scheduledEvent.LeagueCategoryIdCached == mcc.interfaceLeagueCached.LeagueCategoryId &&
                            scheduledEvent.MatchChannelIdCached == mcc.leagueMatchCached.MatchChannelId)
                        {
                            var timeLeft = scheduledEvent.TimeToExecuteTheEventOn - (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();

                            finalMessage += "\n\n Match is done. Deleting this channel in " + timeLeft + " seconds!";
                        }
                    }
                }
            }
        }

        Log.WriteLine("Generated: " + finalMessage, LogLevel.DEBUG);

        return Task.FromResult(finalMessage);
    }
}