using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.CompilerServices;

[DataContract]
public class PLANESELECTIONBUTTON : BaseButton
{
    MatchChannelComponents mcc;
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
        Team playerTeam;

        mcc = new MatchChannelComponents(_interfaceMessage);
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

        try
        {
            playerTeam = mcc.interfaceLeagueCached.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(playerId);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return Task.FromResult(new Response(ex.Message, false));
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

                try
                {
                    var planeReportObject =
                        mcc.leagueMatchCached.MatchReporting.GetInterfaceReportingObjectWithTypeOfTheReportingObject(
                            TypeOfTheReportingObject.PLAYERPLANE, playerTeam.TeamId) as PLAYERPLANE;

                    if (planeReportObject == null)
                    {
                        Log.WriteLine(nameof(planeReportObject) + " was null!", LogLevel.CRITICAL);
                        continue;
                    }

                    Log.WriteLine(planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.Count.ToString(), LogLevel.DEBUG);

                    if (!planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.ContainsKey(playerId))
                    {
                        Log.WriteLine("Does not contain: " + playerId, LogLevel.CRITICAL);
                        continue;
                    }
                    else
                    {
                        Log.WriteLine("Contains: " + playerId, LogLevel.VERBOSE);

                        try
                        {
                            var playerIdSelectedPlane = planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.FirstOrDefault(
                                x => x.Key == playerId);

                            var unitNameInstance = (InterfaceUnit)EnumExtensions.GetInstance(playerSelectedPlane);

                            Log.WriteLine("unitNameInstance:" + unitNameInstance, LogLevel.VERBOSE);

                            if (!planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.ContainsKey(playerId))
                            {
                                Log.WriteLine("does not contain", LogLevel.ERROR);
                                continue;
                            }

                            planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam[playerId] = unitNameInstance.UnitName;

                            Log.WriteLine("Done modifying: " + playerId + " with plane: " +
                                planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam[playerId], LogLevel.DEBUG);
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                            continue;
                        }

                    }

                    _interfaceMessage.GenerateAndModifyTheMessage();

                    return Task.FromResult(new Response("", true));
                }
                catch(Exception ex)
                {
                    Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                    continue;
                }
            }
        }

        return Task.FromResult(new Response("Could not find: " + playerId, false));
    }
}