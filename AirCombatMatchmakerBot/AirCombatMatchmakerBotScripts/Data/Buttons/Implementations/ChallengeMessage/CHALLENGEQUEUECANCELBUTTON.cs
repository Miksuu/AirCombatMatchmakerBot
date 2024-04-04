﻿using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CHALLENGEQUEUECANCELBUTTON : BaseButton
{
    LeagueCategoryComponents lcc;
    public CHALLENGEQUEUECANCELBUTTON()
    {
        buttonName = ButtonName.CHALLENGEQUEUECANCELBUTTON;
        thisInterfaceButton.ButtonLabel = "Leave Queue";
        buttonStyle = ButtonStyle.Danger;
        ephemeralResponse = true;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _channelCategoryId)
    {
        return "";
    }

    public override async Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("Starting processing a challenge canel request by: " +
            _component.User.Id );

        lcc = new LeagueCategoryComponents(_interfaceMessage.MessageCategoryId);

        if (lcc.interfaceLeagueCached == null)
        {
            string errorMsg = nameof(lcc.interfaceLeagueCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.ERROR);
            return new Response(errorMsg, false);
        }

        string response =
            lcc.interfaceLeagueCached.LeagueData.ChallengeStatus.RemoveChallengeFromThisLeague(
                _component.User.Id, lcc.interfaceLeagueCached);

        if (response == "notInTheQueue")
        {
            return new Response("You are not in the queue!!", false);
        }

        _interfaceMessage.GenerateAndModifyTheMessageAsync();

        Log.WriteLine("After modifying message");

        return new Response("", true);
    }
}