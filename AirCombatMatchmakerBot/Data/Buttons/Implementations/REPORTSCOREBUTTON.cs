using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.CompilerServices;

[DataContract]
public class REPORTSCOREBUTTON : BaseButton
{
    public REPORTSCOREBUTTON()
    {
        buttonName = ButtonName.REPORTSCOREBUTTON;
        buttonLabel = "0";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = true;
    }

    public async override Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        InterfaceMessage? reportingStatusMessage =
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                    _interfaceMessage.MessageChannelId).FindInterfaceMessageWithNameInTheChannel(
                        MessageName.REPORTINGSTATUSMESSAGE);
        if (reportingStatusMessage == null)
        {
            string errorMsg = nameof(reportingStatusMessage) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
        }

        string[] splitStrings = buttonCustomId.Split('_');
        ulong playerId = _component.User.Id;
        int playerReportedResult = int.Parse(splitStrings[1]);

        Log.WriteLine("Pressed by: " + playerId + " in: " + reportingStatusMessage.MessageChannelId + 
            " with label int: " + playerReportedResult + " in category: " +
            buttonCategoryId, LogLevel.DEBUG);

        InterfaceChannel interfaceChannel = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
            _interfaceMessage.MessageChannelId);

        //Find the channel of the message and cast the interface to to the MATCHCHANNEL class       
        MATCHCHANNEL? matchChannel = (MATCHCHANNEL)interfaceChannel;
        if (matchChannel == null)
        {
            string errorMsg = nameof(matchChannel) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
        }

        var leagueMatchTuple = 
            matchChannel.FindInterfaceLeagueAndLeagueMatchOnThePressedButtonsChannel(
                buttonCategoryId, reportingStatusMessage.MessageChannelId);
        if (leagueMatchTuple.Item1 == null || leagueMatchTuple.Item2 == null)
        {
            string errorMsg = nameof(leagueMatchTuple) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
        }

        var finalResponseTuple = leagueMatchTuple.Item2.MatchReporting.ProcessPlayersSentReportObject(
            leagueMatchTuple.Item1, playerId, playerReportedResult.ToString(),
            TypeOfTheReportingObject.REPORTEDSCORE, 
            _interfaceMessage.MessageCategoryId, _interfaceMessage.MessageChannelId).Result;

        if (!finalResponseTuple.Item2)
        {
            return (finalResponseTuple.Item1, false);
        }

        finalResponseTuple = await leagueMatchTuple.Item2.MatchReporting.PrepareFinalMatchResult(
            leagueMatchTuple.Item1, playerId,
            _interfaceMessage.MessageCategoryId, _interfaceMessage.MessageChannelId);

        if (!finalResponseTuple.Item2)
        {
            return (finalResponseTuple.Item1, false);
        }

        Log.WriteLine("Reached end before the return with player id: " +
            playerId + " with response:" + finalResponseTuple.Item1, LogLevel.DEBUG);

        return (finalResponseTuple.Item1, true);
    }
}