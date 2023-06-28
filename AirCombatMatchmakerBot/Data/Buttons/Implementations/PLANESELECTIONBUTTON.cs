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
        try
        {
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
                " with selected plane: " + playerSelectedPlane);

            Team playerTeam = mcc.interfaceLeagueCached.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(playerId);

            Log.WriteLine("Finding with " + nameof(playerTeam) + ": " + playerTeam.TeamName + " with id: " + playerTeam.TeamId +
                " on league: " + mcc.interfaceLeagueCached.LeagueCategoryName);

            foreach (var teamKvp in mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData)
            {
                try
                {
                    Log.WriteLine("Loop on: " + teamKvp.Key + " with " + teamKvp.Value);
                    if (teamKvp.Key != playerTeam.TeamId)
                    {
                        continue;
                    }

                    Log.WriteLine("Found team: " + teamKvp.Key + " with name: " + teamKvp.Value.TeamName +
                        " with playerId: " + playerId);

                    var planeReportObject =
                        mcc.leagueMatchCached.MatchReporting.GetInterfaceReportingObjectWithTypeOfTheReportingObject(
                            TypeOfTheReportingObject.PLAYERPLANE, playerTeam.TeamId) as PLAYERPLANE;
                    if (planeReportObject == null)
                    {
                        Log.WriteLine(nameof(planeReportObject) + " was null!", LogLevel.CRITICAL);
                        continue;
                    }

                    Log.WriteLine(planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.Count.ToString(), LogLevel.DEBUG);

                    if (planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam[playerId] != UnitName.NOTSELECTED)
                    {
                        return Task.FromResult(new Response("Already accepted: " + playerId, false));
                    }

                    if (!planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.ContainsKey(playerId))
                    {
                        Log.WriteLine("Does not contain: " + playerId, LogLevel.CRITICAL);
                        continue;
                    }

                    Log.WriteLine("Contains: " + playerId);

                    var playerIdSelectedPlane = planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.FirstOrDefault(
                        x => x.Key == playerId);

                    var unitNameInstance = (InterfaceUnit)EnumExtensions.GetInstance(playerSelectedPlane);

                    Log.WriteLine("unitNameInstance:" + unitNameInstance);

                    if (!planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.ContainsKey(playerId))
                    {
                        Log.WriteLine("does not contain", LogLevel.ERROR);
                        continue;
                    }

                    planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam[playerId] = unitNameInstance.UnitName;

                    Log.WriteLine("Done modifying: " + playerId + " with plane: " +
                        planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam[playerId], LogLevel.DEBUG);

                    // Check here that if everyone else is ready, skip the rest (unnecessary processing otherwise)
                    bool skipCheck = false;
                    foreach (var teamMember in mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData)
                    {
                        if (teamMember.Key != playerTeam.TeamId)
                        {
                            var otherTeamPlaneReportObject =
                                mcc.leagueMatchCached.MatchReporting.GetInterfaceReportingObjectWithTypeOfTheReportingObject(
                                    TypeOfTheReportingObject.PLAYERPLANE, teamMember.Key) as PLAYERPLANE;

                            var status = otherTeamPlaneReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.First();

                            if (status.Value != UnitName.NOTSELECTED)
                            {
                                skipCheck = true;
                                break;
                            }
                        }
                    }
                    if (skipCheck)
                    {
                        return Task.FromResult(new Response("", true));
                    }

                    // If timeUntil is more than 20minutes, add a temp event to cancel the queue after 5min
                    var matchQueueEvent = mcc.leagueMatchCached.MatchEventManager.GetEventByType(typeof(MatchQueueAcceptEvent));
                    var timeUntil = TimeService.CalculateTimeUntilWithUnixTime(matchQueueEvent.TimeToExecuteTheEventOn);
                    if (timeUntil > 1200)
                    {
                        new TempQueueEvent(
                            mcc.interfaceLeagueCached.LeagueCategoryId, mcc.leagueMatchCached.MatchChannelId,
                            playerId, mcc.leagueMatchCached.MatchEventManager.ClassScheduledEvents);
                    }

                    _interfaceMessage.GenerateAndModifyTheMessage();

                    return Task.FromResult(new Response("", true));
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                    continue;
                }
            }
            return Task.FromResult(new Response("Could not find: " + playerId, false));
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return Task.FromResult(new Response(ex.Message, false));
        }
    }
}