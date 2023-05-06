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
    MatchChannelComponents mc = new MatchChannelComponents();
    public CONFIRMSCOREBUTTON()
    {
        buttonName = ButtonName.CONFIRMSCOREBUTTON;
        buttonLabel = "CONFIRM";
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
        string[] splitStrings = buttonCustomId.Split('_');

        ulong playerId = _component.User.Id;
        int playerReportedResult = int.Parse(splitStrings[1]);

        Log.WriteLine("Pressed by: " + playerId + " in: " + _interfaceMessage.MessageChannelId +
            " with label int: " + playerReportedResult + " in category: " +
            buttonCategoryId, LogLevel.DEBUG);

        mc.FindMatchAndItsLeagueAndInsertItToTheCache(_interfaceMessage);

        if (mc.interfaceLeagueCached == null || mc.leagueMatchCached == null)
        {
            string errorMsg = nameof(mc.interfaceLeagueCached) + " or " +
                nameof(mc.leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return Task.FromResult(new Response(errorMsg, false));
        }

        Log.WriteLine("Setting ConfirmedMatch false", LogLevel.VERBOSE);

        foreach (var item in mc.leagueMatchCached.MatchReporting.TeamIdsWithReportData)
        {
            item.Value.ConfirmedMatch = false;
        }

        Log.WriteLine("Done setting ConfirmedMatch false", LogLevel.VERBOSE);

        var response = mc.leagueMatchCached.MatchReporting.ProcessPlayersSentReportObject(
            mc.interfaceLeagueCached, playerId, playerReportedResult.ToString(),
            TypeOfTheReportingObject.REPORTEDSCORE,
            _interfaceMessage.MessageCategoryId, _interfaceMessage.MessageChannelId).Result;

        Log.WriteLine("Reached end before the return with player id: " +
            playerId + " with response:" + response.responseString, LogLevel.DEBUG);

        return Task.FromResult(response);
    }
}