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
        string finalMessage = string.Empty;

        /*
        Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            messageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                messageChannelId).FindInterfaceMessageWithNameInTheChannel(messageName);

        var matchTuple =
            matchChannel.FindInterfaceLeagueAndLeagueMatchOnThePressedButtonsChannel(
        buttonCategoryId, _interfaceMessage.MessageChannelId);

        if (matchTuple.Item1 == null || matchTuple.Item2 == null)
        {
            Log.WriteLine(nameof(matchTuple) + " was null!", LogLevel.CRITICAL);
            return Task.FromResult(matchTuple.Item3);
        }

        var reportDataTupleWithString = matchTuple.Item2.MatchReporting.GetTeamReportDataWithPlayerId(
            matchTuple.Item1, matchTuple.Item2, componentPlayerId);

        finalMessage += "You can either Confirm, Modify or Dispute the result below.";
        */
        return message;
    }
}