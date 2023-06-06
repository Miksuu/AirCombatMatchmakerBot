﻿using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class JOINMATCHSCHEDULER : BaseButton
{
    LeagueCategoryComponents lcc = new LeagueCategoryComponents();
    public JOINMATCHSCHEDULER()
    {
        buttonName = ButtonName.JOINMATCHSCHEDULER;
        thisInterfaceButton.ButtonLabel = "JOIN";
        buttonStyle = ButtonStyle.Success;
        ephemeralResponse = true;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId)
    {
        return "";
    }

    public override async Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        ulong playerId = _component.User.Id;
        ulong channelId = _component.Channel.Id;

        Log.WriteLine("Starting processing a join by: " +
            playerId + " in channel: " + channelId, LogLevel.VERBOSE);

        lcc.FindCategorysLeagueAndInsertItToTheCache(_interfaceMessage.MessageCategoryId);
        if (lcc.interfaceLeagueCached == null)
        {
            string errorMsg = nameof(lcc.interfaceLeagueCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }

        var matchScheduler = lcc.interfaceLeagueCached.LeagueData.MatchScheduler;
        Log.WriteLine(nameof(matchScheduler) + matchScheduler, LogLevel.DEBUG);

        var response = matchScheduler.AddTeamToTheMatchSchedulerWithPlayerId(playerId);

        //_interfaceMessage.GenerateAndModifyTheMessage();
        //Log.WriteLine("After modifying message", LogLevel.VERBOSE);

        return response;
    }
}