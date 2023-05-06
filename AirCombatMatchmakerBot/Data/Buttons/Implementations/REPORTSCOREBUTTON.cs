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
    MatchChannelComponents mmc = new MatchChannelComponents();
    public REPORTSCOREBUTTON()
    {
        buttonName = ButtonName.REPORTSCOREBUTTON;
        buttonLabel = "0";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = true;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId)
    {
        buttonLabel = _buttonIndex.ToString();
        Log.WriteLine("is: " + nameof(buttonName) +
            " set label to: " + buttonLabel, LogLevel.VERBOSE);

        return "";
    }

    public async override Task<Response> ActivateButtonFunction(
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
            return new Response (errorMsg, false);
        }

        string[] splitStrings = buttonCustomId.Split('_');
        ulong playerId = _component.User.Id;
        int playerReportedResult = int.Parse(splitStrings[1]);

        Log.WriteLine("Pressed by: " + playerId + " in: " + reportingStatusMessage.MessageChannelId + 
            " with label int: " + playerReportedResult + " in category: " +
            buttonCategoryId, LogLevel.DEBUG);

        mmc.FindMatchAndItsLeagueAndInsertItToTheCache(_interfaceMessage);

        if (mmc.interfaceLeagueCached == null || mmc.leagueMatchCached == null)
        {
            string errorMsg = nameof(mmc.interfaceLeagueCached) + " or " +
                nameof(mmc.leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }

        Response response = mmc.leagueMatchCached.MatchReporting.ProcessPlayersSentReportObject(
            mmc.interfaceLeagueCached, playerId, playerReportedResult.ToString(),
            TypeOfTheReportingObject.REPORTEDSCORE, 
            _interfaceMessage.MessageCategoryId, _interfaceMessage.MessageChannelId).Result;

        if (!response.serialize)
        {
            return response;
        }

        response = await mmc.leagueMatchCached.MatchReporting.PrepareFinalMatchResult(
            mmc.interfaceLeagueCached, playerId,
            _interfaceMessage.MessageCategoryId, _interfaceMessage.MessageChannelId);

        if (!response.serialize)
        {
            return response;
        }

        Log.WriteLine("Reached end before the return with player id: " +
            playerId + " with response:" + response.responseString, LogLevel.DEBUG);

        return response;
    }
}