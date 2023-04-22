using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.CompilerServices;

[DataContract]
public class PLANESELECTIONBUTTON : BaseMatchButton
{
    public PLANESELECTIONBUTTON()
    {
        buttonName = ButtonName.PLANESELECTIONBUTTON;
        buttonLabel = "Plane";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = false;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId)
    {
        return "";
    }

    public override Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        string finalResponse = string.Empty;
        ulong componentPlayerId = _component.User.Id;

        FindMatchTupleAndInsertItToTheCache(_interfaceMessage);
        if (interfaceLeagueCached == null || leagueMatchCached == null)
        {
            string errorMsg = nameof(interfaceLeagueCached) + " or " +
                nameof(leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return Task.FromResult((errorMsg, false));
        }



        //var playerTeamReportData = leagueMatchCached.MatchReporting.Set

        return Task.FromResult(("", true));
    }
}