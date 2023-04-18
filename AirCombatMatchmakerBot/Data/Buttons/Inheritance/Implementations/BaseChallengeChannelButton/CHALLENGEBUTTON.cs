using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CHALLENGEBUTTON : BaseChallengeChannelButton
{
    public CHALLENGEBUTTON()
    {
        buttonName = ButtonName.CHALLENGEBUTTON;
        buttonLabel = "Enter Queue";
        buttonStyle = ButtonStyle.Success;
        ephemeralResponse = true;
    }

    public override async Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("Starting processing a challenge by: " +
            _component.User.Id , LogLevel.VERBOSE);
        
        FindInterfaceLeagueAndCacheIt(_component.Channel.Id);
        if (interfaceLeagueCached == null)
        {
            string errorMsg = nameof(interfaceLeagueCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
        }

        string response = interfaceLeagueCached.LeagueData.ChallengeStatus.PostChallengeToThisLeague(
            _component.User.Id, interfaceLeagueCached.LeaguePlayerCountPerTeam,
            interfaceLeagueCached);

        if (response == "alreadyInQueue")
        {
            return ("You are already in the queue!", false);
        }

        //string newMessage = _interfaceMessage.MessageDescription + postedChallengeMessage;

        await _interfaceMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("After modifying message", LogLevel.VERBOSE);

        return ("", true);
    }
}