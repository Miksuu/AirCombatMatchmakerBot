﻿using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.CompilerServices;

[DataContract]
public class CONFIRMSCOREBUTTON : BaseMatchButton
{
    public CONFIRMSCOREBUTTON()
    {
        buttonName = ButtonName.CONFIRMSCOREBUTTON;
        buttonLabel = "CONFIRM";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = false;
    }

    public override Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        string[] splitStrings = buttonCustomId.Split('_');

        ulong playerId = _component.User.Id;
        int playerReportedResult = int.Parse(splitStrings[1]);

        Log.WriteLine("Pressed by: " + playerId + " in: " + _interfaceMessage.MessageChannelId +
            " with label int: " + playerReportedResult + " in category: " +
            buttonCategoryId, LogLevel.DEBUG);

        InterfaceChannel interfaceChannel = 
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                    _interfaceMessage.MessageChannelId);

        FindMatchTupleAndInsertItToTheCache(_interfaceMessage);
        if (interfaceLeagueCached == null || leagueMatchCached == null)
        {
            string errorMsg = nameof(interfaceLeagueCached) + " or " +
                nameof(leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return Task.FromResult((errorMsg, false));
        }

        Log.WriteLine("Setting ConfirmedMatch false", LogLevel.VERBOSE);

        foreach (var item in leagueMatchCached.MatchReporting.TeamIdsWithReportData)
        {
            item.Value.ConfirmedMatch = false;
        }

        Log.WriteLine("Done setting ConfirmedMatch false", LogLevel.VERBOSE);

        var finalResponseTuple = leagueMatchCached.MatchReporting.ProcessPlayersSentReportObject(
            interfaceLeagueCached, playerId, playerReportedResult.ToString(),
            TypeOfTheReportingObject.REPORTEDSCORE,
            _interfaceMessage.MessageCategoryId, _interfaceMessage.MessageChannelId).Result;

        Log.WriteLine("Reached end before the return with player id: " +
            playerId + " with response:" + finalResponseTuple.Item1, LogLevel.DEBUG);

        return Task.FromResult((finalResponseTuple.Item1, true));
    }
}