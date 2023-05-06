using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CHALLENGEQUEUECANCELBUTTON : BaseButton
{
    MatchChannelComponents mmc = new MatchChannelComponents();
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

        mmc.FindMatchAndItsLeagueAndInsertItToTheCache(_interfaceMessage);

        if (mmc.interfaceLeagueCached == null || mmc.leagueMatchCached == null)
        {
            string errorMsg = nameof(mmc.interfaceLeagueCached) + " or " +
                nameof(mmc.leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }

        string response = mmc.interfaceLeagueCached.LeagueData.ChallengeStatus.RemoveChallengeFromThisLeague(
            _component.User.Id, mmc.interfaceLeagueCached.LeaguePlayerCountPerTeam, mmc.interfaceLeagueCached);

        if (response == "notInTheQueue")
        {
            return new Response("You are not in the queue!!", false);
        }

        await _interfaceMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("After modifying message", LogLevel.VERBOSE);

        return new Response("", true);
    }
}