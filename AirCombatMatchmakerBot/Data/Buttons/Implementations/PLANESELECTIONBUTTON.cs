﻿using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.CompilerServices;

[DataContract]
public class PLANESELECTIONBUTTON : BaseButton
{
    MatchChannelComponents mcc = new MatchChannelComponents();
    public PLANESELECTIONBUTTON()
    {
        buttonName = ButtonName.PLANESELECTIONBUTTON;
        thisInterfaceButton.ButtonLabel = "Plane";
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
        mcc.FindMatchAndItsLeagueAndInsertItToTheCache(_interfaceMessage);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            return Task.FromResult(new Response(nameof(mcc) + " was null!", false));
        }

        var playerId = _component.User.Id;
        string[] splitStrings = thisInterfaceButton.ButtonCustomId.Split('_');
        string playerSelectedPlane = splitStrings[0];

        Log.WriteLine("Starting to find team with playerId: " + playerId +
            " with selected plane: " + playerSelectedPlane, LogLevel.VERBOSE);

        Team? playerTeam = mcc.interfaceLeagueCached.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(playerId);
        if (playerTeam == null)
        {
            Log.WriteLine(nameof(playerTeam) + " was null!", LogLevel.CRITICAL);
            return Task.FromResult(new Response(nameof(playerTeam) + " was null!", false));
        }

        Log.WriteLine("Finding with " + nameof(playerTeam) + ": " + playerTeam.TeamName + " with id: " + playerTeam.TeamId +
            " on league: " + mcc.interfaceLeagueCached.LeagueCategoryName, LogLevel.VERBOSE);

        foreach (var teamKvp in mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData)
        {
            Log.WriteLine("Loop on: " + teamKvp.Key + " with " + teamKvp.Value, LogLevel.VERBOSE);
            if (teamKvp.Key == playerTeam.TeamId)
            {
                Log.WriteLine("Found team: " + teamKvp.Key + " with name: " + teamKvp.Value.TeamName +
                    " with playerId: " + playerId, LogLevel.VERBOSE);

                if (!teamKvp.Value.TeamMemberIdsWithSelectedPlanesByTheTeam.ContainsKey(playerId))
                {
                    Log.WriteLine("Does not contain: " + playerId, LogLevel.CRITICAL);
                    continue;
                }
                else
                {
                    Log.WriteLine("Contains: " + playerId, LogLevel.VERBOSE);
                    var playerIdSelectedPlane = teamKvp.Value.TeamMemberIdsWithSelectedPlanesByTheTeam.FirstOrDefault(x => x.Key == playerId);
                    if (playerIdSelectedPlane.Value == null)
                    {
                        Log.WriteLine(nameof(playerIdSelectedPlane.Value) + " was null!", LogLevel.CRITICAL);
                        return Task.FromResult(new Response(nameof(playerIdSelectedPlane.Value) + " was null!", false));
                    }

                    playerIdSelectedPlane.Value.SetObjectValueAndFieldBool(playerSelectedPlane, EmojiName.WHITECHECKMARK);

                    Log.WriteLine("Done modifying: " + playerId + " with plane: " + playerSelectedPlane, LogLevel.VERBOSE);
                }

                _interfaceMessage.GenerateAndModifyTheMessage();

                return Task.FromResult(new Response("", true));
            }
        }

        return Task.FromResult(new Response("Could not find: " + playerId, false));
    }
}