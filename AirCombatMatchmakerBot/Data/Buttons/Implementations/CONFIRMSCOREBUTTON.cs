using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.CompilerServices;

[DataContract]
public class CONFIRMSCOREBUTTON : BaseButton
{
    MatchChannelComponents mcc;
    public CONFIRMSCOREBUTTON()
    {
        buttonName = ButtonName.CONFIRMSCOREBUTTON;
        thisInterfaceButton.ButtonLabel = "CONFIRM";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = false;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId)
    {
        return "";
    }

    public override Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        string[] splitStrings = thisInterfaceButton.ButtonCustomId.Split('_');

        ulong playerId = _component.User.Id;
        int playerReportedResult = int.Parse(splitStrings[1]);

        Log.WriteLine("Pressed by: " + playerId + " in: " + _interfaceMessage.MessageChannelId +
            " with label int: " + playerReportedResult + " in category: " +
            thisInterfaceButton.ButtonCategoryId, LogLevel.DEBUG);

        mcc = new MatchChannelComponents(_interfaceMessage);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                nameof(mcc.leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return Task.FromResult(new Response(errorMsg, false));
        }

        Log.WriteLine("Setting ConfirmedMatch false");

        foreach (var item in mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData)
        {
            item.Value.ConfirmedMatch = false;
        }

        Log.WriteLine("Done setting ConfirmedMatch false");

        var response = mcc.leagueMatchCached.MatchReporting.ProcessPlayersSentReportObject(
            playerId, playerReportedResult.ToString(),
            TypeOfTheReportingObject.REPORTEDSCORE,
            _interfaceMessage.MessageCategoryId, _interfaceMessage.MessageChannelId).Result;

        Log.WriteLine("Reached end before the return with player id: " +
            playerId + " with response:" + response.responseString, LogLevel.DEBUG);

        return Task.FromResult(response);
    }
}