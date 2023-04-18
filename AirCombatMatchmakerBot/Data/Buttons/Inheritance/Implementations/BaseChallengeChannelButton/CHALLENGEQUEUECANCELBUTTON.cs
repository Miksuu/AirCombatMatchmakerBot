using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CHALLENGEQUEUECANCELBUTTON : BaseChallengeChannelButton
{
    public CHALLENGEQUEUECANCELBUTTON()
    {
        buttonName = ButtonName.CHALLENGEQUEUECANCELBUTTON;
        buttonLabel = "Leave Queue";
        buttonStyle = ButtonStyle.Danger;
        ephemeralResponse = true;
    }

    public override async Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("Starting processing a challenge canel request by: " +
            _component.User.Id , LogLevel.VERBOSE);

        FindInterfaceLeagueAndCacheIt(_component.Channel.Id);
        if (interfaceLeagueCached == null)
        {
            string errorMsg = nameof(interfaceLeagueCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
        }

        string response = interfaceLeagueCached.LeagueData.ChallengeStatus.RemoveChallengeFromThisLeague(
            _component.User.Id, interfaceLeagueCached.LeaguePlayerCountPerTeam, interfaceLeagueCached);

        if (response == "notInTheQueue")
        {
            return ("You are not in the queue!!", false);
        }

        await _interfaceMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("After modifying message", LogLevel.VERBOSE);

        return ("", true);
    }
}