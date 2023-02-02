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
    }

    public void CreateTheButton(){}

    public override Task<string> ActivateButtonFunction(
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
            Log.WriteLine(nameof(interfaceChannel) + " was null!", LogLevel.CRITICAL);
            return Task.FromResult("Could not find: " + nameof(interfaceChannel));
        }


        MATCHCHANNEL? matchChannel = (MATCHCHANNEL)interfaceChannel;
        if (matchChannel == null)
        {
            Log.WriteLine(nameof(matchChannel) + " was null!", LogLevel.CRITICAL);
            return Task.FromResult("Could not find: " + nameof(matchChannel));
        }

        var matchTuple =
            matchChannel.FindInterfaceLeagueAndLeagueMatchOnThePressedButtonsChannel(
        buttonCategoryId, _interfaceMessage.MessageChannelId);

        if (matchTuple.Item1 == null || matchTuple.Item2 == null)
        {
            Log.WriteLine(nameof(matchTuple) + " was null!", LogLevel.CRITICAL);
            return Task.FromResult(matchTuple.Item3);
        }

        var reportDataTupleWithString = matchTuple.Item2.MatchReporting.GetTeamReportDatasOfTheMatchWithPlayerId(
            matchTuple.Item1, matchTuple.Item2, componentPlayerId);

        if (reportDataTupleWithString.Item1 == null)
        {
            Log.WriteLine(nameof(reportDataTupleWithString) + " was null!", LogLevel.CRITICAL);
            return Task.FromResult(reportDataTupleWithString.Item2);
        }

        if (reportDataTupleWithString.Item2 != "")
        {
            Log.WriteLine("User: " + componentPlayerId + " confirm a match on channel: " +
                _component.Channel.Id + "!", LogLevel.WARNING);
            return Task.FromResult(reportDataTupleWithString.Item2);
        }

        if (reportDataTupleWithString.Item1.ElementAt(0).ConfirmedMatch)
        {
            return Task.FromResult("You have already confirmed the match!");
        }

        reportDataTupleWithString.Item1.ElementAt(0).ConfirmedMatch = true;

        if (reportDataTupleWithString.Item1.ElementAt(1).ConfirmedMatch == true)
        {
            matchTuple.Item2.MatchReporting.MatchDone = true;
            Log.WriteLine("Both teams are done with the reporting on match: " + matchTuple.Item2.MatchId, LogLevel.DEBUG);
        }

        InterfaceMessage confirmationMessage =
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                    _interfaceMessage.MessageChannelId).FindInterfaceMessageWithNameInTheChannel(
                        MessageName.CONFIRMATIONMESSAGE);

        confirmationMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("Reached end before the return with player id: " + componentPlayerId, LogLevel.DEBUG);

        return Task.FromResult(finalResponse);
    }
}