using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class CONFIRMMATCHRESULTBUTTON : BaseButton
{
    public CONFIRMMATCHRESULTBUTTON()
    {
        buttonName = ButtonName.CONFIRMMATCHRESULTBUTTON;
        buttonLabel = "CONFIRM";
        buttonStyle = ButtonStyle.Success;
        ephemeralResponse = true;
    }

    public override async Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        string finalResponse = string.Empty;

        ulong componentPlayerId = _component.User.Id;

        Log.WriteLine("Activating button function: " + buttonName.ToString() + " by: " +
            componentPlayerId + " in msg: " + _interfaceMessage.MessageId, LogLevel.VERBOSE);

        InterfaceChannel interfaceChannel = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
            _interfaceMessage.MessageChannelId);
        if (interfaceChannel == null)
        {
            string errorMsg = nameof(interfaceChannel) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
        }

        MATCHCHANNEL? matchChannel = (MATCHCHANNEL)interfaceChannel;
        if (matchChannel == null)
        {
            string errorMsg = nameof(matchChannel) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
        }

        var matchTuple =
            matchChannel.FindInterfaceLeagueAndLeagueMatchOnThePressedButtonsChannel(
                buttonCategoryId, _interfaceMessage.MessageChannelId);
        if (matchTuple.Item1 == null || matchTuple.Item2 == null)
        {
            Log.WriteLine(matchTuple.Item3, LogLevel.CRITICAL);
            return (matchTuple.Item3, false);
        }

        var reportDataTupleWithString = matchTuple.Item2.MatchReporting.GetTeamReportDatasOfTheMatchWithPlayerId(
            matchTuple.Item1, matchTuple.Item2, componentPlayerId);
        if (reportDataTupleWithString.Item1 == null)
        {
            Log.WriteLine(nameof(reportDataTupleWithString) + " was null!", LogLevel.CRITICAL);
            return (reportDataTupleWithString.Item2, false);
        }
        if (reportDataTupleWithString.Item2 != "")
        {
            Log.WriteLine("User: " + componentPlayerId + " confirm a match on channel: " +
                _component.Channel.Id + "!", LogLevel.WARNING);
            return (reportDataTupleWithString.Item2, false);
        }

        if (reportDataTupleWithString.Item1.ElementAt(0).ConfirmedMatch)
        {
            return ("You have already confirmed the match!", false);
        }

        reportDataTupleWithString.Item1.ElementAt(0).ConfirmedMatch = true;

        if (reportDataTupleWithString.Item1.ElementAt(1).ConfirmedMatch == true)
        {
            matchTuple.Item2.MatchReporting.MatchDone = true;
            Log.WriteLine("Both teams are done with the reporting on match: " + matchTuple.Item2.MatchId, LogLevel.DEBUG);
        }

        InterfaceMessage? confirmationMessage =
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                    _interfaceMessage.MessageChannelId).FindInterfaceMessageWithNameInTheChannel(
                        MessageName.CONFIRMATIONMESSAGE);

        if (confirmationMessage == null)
        {
            string errorMsg = nameof(confirmationMessage) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
        }

        Log.WriteLine("Found: " + confirmationMessage.MessageId + " with content: " + confirmationMessage.Message, LogLevel.DEBUG);

        await confirmationMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("Reached end before the return with player id: " + componentPlayerId +
            " with finalResposne: " + finalResponse, LogLevel.DEBUG);

        return (finalResponse, true);
    }
}