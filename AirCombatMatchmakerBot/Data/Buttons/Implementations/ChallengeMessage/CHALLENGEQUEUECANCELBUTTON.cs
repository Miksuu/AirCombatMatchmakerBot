using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CHALLENGEQUEUECANCELBUTTON : BaseButton
{
    MatchChannelComponents mc = new MatchChannelComponents();
    public CHALLENGEQUEUECANCELBUTTON()
    {
        buttonName = ButtonName.CHALLENGEQUEUECANCELBUTTON;
        buttonLabel = "Leave Queue";
        buttonStyle = ButtonStyle.Danger;
        ephemeralResponse = true;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId)
    {
        return "";
    }

    public override async Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("Starting processing a challenge canel request by: " +
            _component.User.Id , LogLevel.VERBOSE);

        mc.FindMatchAndItsLeagueAndInsertItToTheCache(_interfaceMessage);

        if (mc.interfaceLeagueCached == null || mc.leagueMatchCached == null)
        {
            string errorMsg = nameof(mc.interfaceLeagueCached) + " or " +
                nameof(mc.leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }

        string response = mc.interfaceLeagueCached.LeagueData.ChallengeStatus.RemoveChallengeFromThisLeague(
            _component.User.Id, mc.interfaceLeagueCached.LeaguePlayerCountPerTeam, mc.interfaceLeagueCached);

        if (response == "notInTheQueue")
        {
            return new Response("You are not in the queue!!", false);
        }

        await _interfaceMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("After modifying message", LogLevel.VERBOSE);

        return new Response("", true);
    }
}