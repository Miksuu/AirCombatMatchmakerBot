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

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _channelCategoryId)
    {
        return "";
    }

    public override async Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        try
        {
            InitializeMatchComponents(_interfaceMessage);
            if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
            {
                Log.WriteLine(nameof(mcc) + " was null!", LogLevel.ERROR);
                return new Response(nameof(mcc) + " was null!", false);
            }

            var playerId = _component.User.Id;
            string playerSelectedPlane = GetPlayerSelectedPlane();

            Log.WriteLine($"Starting to find team with playerId: {playerId} with selected plane: {playerSelectedPlane}");

            Team playerTeam = FindActiveTeamByPlayerId(playerId);

            Log.WriteLine($"Finding with {nameof(playerTeam)}: {playerTeam.TeamName} with id: {playerTeam.TeamId} on league: {mcc.interfaceLeagueCached.LeagueCategoryName}");

            return await ProcessTeams(playerId, playerSelectedPlane, playerTeam, _interfaceMessage);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return new Response(ex.Message, false);
        }
    }

    private void InitializeMatchComponents(InterfaceMessage _interfaceMessage)
    {
        mcc = new MatchChannelComponents(_interfaceMessage);
    }

    private string GetPlayerSelectedPlane()
    {
        string[] splitStrings = thisInterfaceButton.ButtonCustomId.Split('_');
        return splitStrings[0];
    }

    private Team FindActiveTeamByPlayerId(ulong _playerId)
    {
        return mcc.interfaceLeagueCached.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);
    }

    private async Task<Response> ProcessTeams(
        ulong _playerId, string _playerSelectedPlane, Team _playerTeam, InterfaceMessage _interfaceMessage)
    {
        foreach (var teamKvp in mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData)
        {
            try
            {
                Log.WriteLine($"Loop on: {teamKvp.Key} with {teamKvp.Value}");
                if (teamKvp.Key != _playerTeam.TeamId)
                {
                    continue;
                }

                Log.WriteLine($"Found team: {teamKvp.Key} with name: {teamKvp.Value.TeamName} with playerId: {_playerId}");

                var planeReportObject = GetPlaneReportObject(_playerTeam.TeamId);
                if (planeReportObject == null)
                {
                    continue;
                }

                Log.WriteLine(planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.Count.ToString(), LogLevel.DEBUG);

                if (planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam[_playerId] != UnitName.NOTSELECTED)
                {
                    return new Response($"Already accepted: {_playerId}", false);
                }

                if (!planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.ContainsKey(_playerId))
                {
                    Log.WriteLine($"Does not contain: {_playerId}", LogLevel.ERROR);
                    continue;
                }

                Log.WriteLine($"Contains: {_playerId}");

                var unitNameInstance = GetUnitNameInstance(_playerSelectedPlane);

                Log.WriteLine($"unitNameInstance: {unitNameInstance}");

                if (!planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.ContainsKey(_playerId))
                {
                    Log.WriteLine("does not contain", LogLevel.ERROR);
                    continue;
                }

                planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam[_playerId] = unitNameInstance.UnitName;

                Log.WriteLine($"Done modifying: {_playerId} with plane: {planeReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam[_playerId]}", LogLevel.DEBUG);

                bool everyoneIsReady = CheckIfEveryoneIsReady(_playerTeam.TeamId);

                if (!everyoneIsReady)
                {
                    _interfaceMessage.GenerateAndModifyTheMessage();
                    var timeUntil = GetTimeUntil();
                    if (timeUntil > 1200)
                    {
                        CreateTempQueueEvent(_playerId);
                    }

                    return new Response("", true);
                }

                mcc.leagueMatchCached.MatchEventManager.ClearCertainTypeOfEventsFromTheList(typeof(MatchQueueAcceptEvent));
                mcc.leagueMatchCached.MatchEventManager.ClearCertainTypeOfEventsFromTheList(typeof(TempQueueEvent));

                mcc.leagueMatchCached.MatchState = MatchState.REPORTINGPHASE;

                InterfaceChannel interfaceChannel = Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryWithCategoryId(
                        _interfaceMessage.MessageCategoryId).FindInterfaceChannelWithIdInTheCategory(
                            _interfaceMessage.MessageChannelId);

                await SerializationManager.SerializeDB();

                new Thread(() => mcc.leagueMatchCached.StartTheMatchOnSecondThread(interfaceChannel)).Start();
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.ERROR);
                continue;
            }
        }
        return new Response($"Could not find: {_playerId}", false);
    }

    private PLAYERPLANE GetPlaneReportObject(int _teamId)
    {
        return mcc.leagueMatchCached.MatchReporting.GetInterfaceReportingObjectWithTypeOfTheReportingObject(
            TypeOfTheReportingObject.PLAYERPLANE, _teamId) as PLAYERPLANE;
    }

    private InterfaceUnit GetUnitNameInstance(string _playerSelectedPlane)
    {
        return (InterfaceUnit)EnumExtensions.GetInstance(_playerSelectedPlane);
    }

    private bool CheckIfEveryoneIsReady(int _teamId)
    {
        foreach (var teamMember in mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData)
        {
            if (teamMember.Key == _teamId)
            {
                continue;
            }

            var otherTeamPlaneReportObject = GetPlaneReportObject(teamMember.Key);

            var status = otherTeamPlaneReportObject.TeamMemberIdsWithSelectedPlanesByTheTeam.First();

            if (status.Value != UnitName.NOTSELECTED)
            {
                return true;
            }
        }

        return false;
    }

    private ulong GetTimeUntil()
    {
        return TimeService.CalculateTimeUntilWithUnixTime(
            mcc.leagueMatchCached.MatchEventManager.GetTimeUntilEventOfType(typeof(MatchQueueAcceptEvent)));
    }

    private void CreateTempQueueEvent(ulong _playerId)
    {
        new TempQueueEvent(
            mcc.interfaceLeagueCached.LeagueCategoryId, mcc.leagueMatchCached.MatchChannelId,
            _playerId, mcc.leagueMatchCached.MatchEventManager.ClassScheduledEvents);
    }
}