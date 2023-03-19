using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.CompilerServices;

[DataContract]
public class MODIFYSCOREBUTTON : BaseButton
{
    public MODIFYSCOREBUTTON()
    {
        buttonName = ButtonName.MODIFYSCOREBUTTON;
        buttonLabel = "0";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = true;
    }

    public override Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        InterfaceMessage? matchFinalResultMessage =
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                    _interfaceMessage.MessageChannelId).FindInterfaceMessageWithNameInTheChannel(
                        MessageName.MATCHFINALRESULTMESSAGE);
        if (matchFinalResultMessage == null)
        {
            string errorMsg = nameof(matchFinalResultMessage) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return Task.FromResult((errorMsg, false));
        }

        string[] splitStrings = buttonCustomId.Split('_');

        ulong playerId = _component.User.Id;
        int playerReportedResult = int.Parse(splitStrings[1]);

        Log.WriteLine("Pressed by: " + playerId + " in: " + matchFinalResultMessage.MessageChannelId +
            " with label int: " + playerReportedResult + " in category: " +
            buttonCategoryId, LogLevel.DEBUG);

        InterfaceChannel interfaceChannel = 
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                    _interfaceMessage.MessageChannelId);

        //Find the channel of the message and cast the interface to to the MATCHCHANNEL class       
        MATCHCHANNEL? matchChannel = (MATCHCHANNEL)interfaceChannel;
        if (matchChannel == null)
        {
            string errorMsg = nameof(matchChannel) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return Task.FromResult((errorMsg, false));
        }

        var leagueMatchTuple = 
            matchChannel.FindInterfaceLeagueAndLeagueMatchOnThePressedButtonsChannel(
                buttonCategoryId, matchFinalResultMessage.MessageChannelId);
        if (leagueMatchTuple.Item1 == null || leagueMatchTuple.Item2 == null)
        {
            string errorMsg = nameof(leagueMatchTuple) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return Task.FromResult((errorMsg, false));
        }

        var finalResponseTuple = leagueMatchTuple.Item2.MatchReporting.ProcessPlayersSentReportObject(
            leagueMatchTuple.Item1, playerId, playerReportedResult.ToString(),
            TypeOfTheReportingObject.REPORTEDSCORE).Result;

        Log.WriteLine("Reached end before the return with player id: " +
            playerId + " with response:" + finalResponseTuple.Item1, LogLevel.DEBUG);

        return Task.FromResult((finalResponseTuple.Item1, true));
    }
}