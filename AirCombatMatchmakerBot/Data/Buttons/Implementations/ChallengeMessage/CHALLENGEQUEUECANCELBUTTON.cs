using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CHALLENGEQUEUECANCELBUTTON : BaseButton
{
    MatchChannelComponents mcc = new MatchChannelComponents();
    public CHALLENGEQUEUECANCELBUTTON()
    {
        buttonName = ButtonName.CHALLENGEQUEUECANCELBUTTON;
        thisInterfaceButton.ButtonLabel = "Leave Queue";
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

        mcc.FindMatchAndItsLeagueAndInsertItToTheCache(_interfaceMessage);

        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                nameof(mcc.leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }

        string response = mcc.interfaceLeagueCached.LeagueData.ChallengeStatus.RemoveChallengeFromThisLeague(
            _component.User.Id, mcc.interfaceLeagueCached.LeaguePlayerCountPerTeam, mcc.interfaceLeagueCached);

        if (response == "notInTheQueue")
        {
            return new Response("You are not in the queue!!", false);
        }

        await _interfaceMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("After modifying message", LogLevel.VERBOSE);

        return new Response("", true);
    }
}