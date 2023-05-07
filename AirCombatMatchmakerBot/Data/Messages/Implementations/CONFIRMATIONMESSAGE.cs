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
    MatchChannelComponents mcc = new MatchChannelComponents();
    public CONFIRMATIONMESSAGE()
    {
        messageName = MessageName.CONFIRMATIONMESSAGE;

        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.CONFIRMMATCHRESULTBUTTON, 1),
                new KeyValuePair<ButtonName, int>(ButtonName.DISPUTEMATCHRESULTBUTTON, 1),
            });

        messageEmbedTitle = "Match confirmation";
        messageDescription = "";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override string GenerateMessage()
    {
        Log.WriteLine("Starting to generate a message for the confirmation", LogLevel.DEBUG);

        mcc.FindMatchAndItsLeagueAndInsertItToTheCache(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            return nameof(mcc) + " was null!";
        }

        string finalMessage = "Confirmed:\n";

        InterfaceChannel interfaceChannel = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            thisInterfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                thisInterfaceMessage.MessageChannelId);
        if (interfaceChannel == null)
        {
            Log.WriteLine(nameof(interfaceChannel) + " was null!", LogLevel.CRITICAL);
            return "InterfaceChannel was null!";
        }

        Log.WriteLine("Found interfaceChannel:" + interfaceChannel.ChannelId, LogLevel.VERBOSE);

        //Log.WriteLine("Found match tuple: " + mcc.leagueMatchCached.MatchChannelId, LogLevel.VERBOSE);

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

        if (confirmedTeamsCounter > 1) 
        {
            InterfaceLeague? interfaceLeague =
                Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(thisInterfaceMessage.MessageCategoryId);
            if (interfaceLeague == null)
            {
                Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.CRITICAL);
                return nameof(interfaceLeague) + " was null!";
            }

            mcc.leagueMatchCached.FinishTheMatch(interfaceLeague);
        }

        finalMessage += "You can either Confirm/Dispute the result below.";

        Log.WriteLine("Generated: " + finalMessage, LogLevel.DEBUG);

        return finalMessage;
    }
}