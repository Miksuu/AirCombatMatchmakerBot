using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;

[DataContract]
public class CONFIRMATIONMESSAGE : BaseMessage
{
    public CONFIRMATIONMESSAGE()
    {
        messageName = MessageName.CONFIRMATIONMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>
        {
            { ButtonName.CONFIRMMATCHRESULTBUTTON, 1 },
            { ButtonName.MODIFYMATCHRESULTBUTTON, 1 },
            { ButtonName.DISPUTEMATCHRESULTBUTTON, 1 }
        };
        message = "You can either Confirm, Modify or Dispute the result below.";
    }

    public override string GenerateMessage()
    {
        Log.WriteLine("Starting to generate a message for the confirmation", LogLevel.DEBUG);

        string finalMessage = "Confirmed:\n";

        InterfaceChannel interfaceChannel = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            messageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
            messageChannelId);
        if (interfaceChannel == null)
        {
            Log.WriteLine(nameof(interfaceChannel) + " was null!", LogLevel.CRITICAL);
            return "InterfaceChannel was null!";
        }

        Log.WriteLine("Found interfaceChannel:" + interfaceChannel.ChannelId, LogLevel.VERBOSE);

        //Find the channel of the message and cast the interface to to the MATCHCHANNEL class       
        MATCHCHANNEL? matchChannel = (MATCHCHANNEL)interfaceChannel;
        if (matchChannel == null)
        {
            Log.WriteLine(nameof(matchChannel) + " was null!", LogLevel.CRITICAL);
            return nameof(matchChannel) + " was null!";
        }

        Log.WriteLine(nameof(matchChannel) + ": " + matchChannel, LogLevel.VERBOSE);

        var matchTuple =
            matchChannel.FindInterfaceLeagueAndLeagueMatchOnThePressedButtonsChannel(
                messageCategoryId, messageChannelId);

        if (matchTuple.Item1 == null || matchTuple.Item2 == null)
        {
            Log.WriteLine(nameof(matchTuple) + " was null!", LogLevel.CRITICAL);
            return matchTuple.Item3;
        }

        Log.WriteLine("Found match tuple: " + matchTuple.Item2.MatchChannelId, LogLevel.VERBOSE);

        var matchReportData = matchTuple.Item2.MatchReporting.TeamIdsWithReportData;

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
            InterfaceLeague? interfaceLeague = Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(messageCategoryId);
            if (interfaceLeague == null)
            {
                Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.CRITICAL);
                return nameof(interfaceLeague) + " was null!";
            }

            matchTuple.Item2.FinishTheMatch(interfaceLeague);
        }

        finalMessage += "You can either Confirm, Modify or Dispute the result below.";

        Log.WriteLine("Generated: " + finalMessage, LogLevel.DEBUG);

        return finalMessage;
    }
}