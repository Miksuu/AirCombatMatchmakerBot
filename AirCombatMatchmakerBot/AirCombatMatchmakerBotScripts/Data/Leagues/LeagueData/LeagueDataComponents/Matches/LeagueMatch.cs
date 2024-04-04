using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using System;
using System.Globalization;
using System.Security;
using Microsoft.VisualBasic;
using Discord;
using System.Threading.Channels;
using System.Text.RegularExpressions;

[DataContract]
public class LeagueMatch
{
    [IgnoreDataMember]
    public ConcurrentDictionary<int, string> TeamsInTheMatch
    {
        get => teamsInTheMatch.GetValue();
        set => teamsInTheMatch.SetValue(value);
    }

    public int MatchId
    {
        get => matchId.GetValue();
        set => matchId.SetValue(value);
    }

    public ulong MatchChannelId
    {
        get => matchChannelId.GetValue();
        set => matchChannelId.SetValue(value);
    }

    public LeagueName MatchLeague
    {
        get => matchLeague.GetValue();
        set => matchLeague.SetValue(value);
    }

    public bool IsAScheduledMatch
    {
        get => isAScheduledMatch.GetValue();
        set => isAScheduledMatch.SetValue(value);
    }

    public MatchState MatchState
    {
        get => matchState.GetValue();
        set => matchState.SetValue(value);
    }

    public ConcurrentBag<ulong> AlreadySuggestedTimes
    {
        get => alreadySuggestedTimes.GetValue();
        set => alreadySuggestedTimes.SetValue(value);
    }

    [DataMember] private logConcurrentDictionary<int, string> teamsInTheMatch = new logConcurrentDictionary<int, string>();
    [DataMember] private logVar<int> matchId = new logVar<int>();
    [DataMember] private logVar<ulong> matchChannelId = new logVar<ulong>();
    [DataMember] public MatchReporting MatchReporting = new MatchReporting();
    [DataMember] private logEnum<LeagueName> matchLeague = new logEnum<LeagueName>();
    [DataMember] public ScheduleObject ScheduleObject = new ScheduleObject();
    [DataMember] private logVar<bool> isAScheduledMatch = new logVar<bool>();
    [DataMember] public EventManager MatchEventManager = new EventManager();
    [DataMember] private logEnum<MatchState> matchState = new logEnum<MatchState>();

    [DataMember] private logConcurrentBag<ulong> alreadySuggestedTimes = new logConcurrentBag<ulong>();

    public LeagueMatch() { }

    // TODO: Add interfaceLeague ref on constructor as a reference
    public LeagueMatch(InterfaceLeague _interfaceLeague, int[] _teamsToFormMatchOn,
        MatchState _matchState, bool _attemptToPutTeamsBackToQueueAfterTheMatch = false)
    {
        try
        {
            Log.WriteLine("Starting of LeagueMatch ctor: " + _teamsToFormMatchOn.Length + " state: " +
                _matchState + " with bool:" + IsAScheduledMatch);

            IsAScheduledMatch = _attemptToPutTeamsBackToQueueAfterTheMatch;
            //SetInterfaceLeagueReferencesForTheMatch(_interfaceLeague);

            int leagueTeamSize = _interfaceLeague.LeaguePlayerCountPerTeam;
            MatchLeague = _interfaceLeague.LeagueCategoryName;

            Log.WriteLine("Teams to from the match on: " + _teamsToFormMatchOn[0] +
                ", " + _teamsToFormMatchOn[1] + " scheduled match: " + _attemptToPutTeamsBackToQueueAfterTheMatch, LogLevel.VERBOSE);

            // Add the team's name to the ConcurrentDictionary as a value
            foreach (int teamId in _teamsToFormMatchOn)
            {
                Log.WriteLine("Starting to loop: " + teamId);
                Team foundTeam =
                    _interfaceLeague.LeagueData.Teams.FindTeamById(teamId);

                Log.WriteLine("Found team: " + foundTeam.TeamId, LogLevel.DEBUG);

                TeamsInTheMatch.TryAdd(teamId, foundTeam.GetTeamInAString(false, leagueTeamSize));

                Log.WriteLine("Count is now: " + TeamsInTheMatch.Count, LogLevel.DEBUG);
            }

            foreach (var item in TeamsInTheMatch)
            {
                Log.WriteLine("final teamsInTheMatch: " + item.Key + " with count: " +TeamsInTheMatch.Count, LogLevel.VERBOSE);
            }

            Log.WriteLine("before incrementing to: " + Database.GetInstance<ApplicationDatabase>().Leagues.LeaguesMatchCounter + " with: " + TeamsInTheMatch.Count);

            MatchId = Database.GetInstance<ApplicationDatabase>().Leagues.LeaguesMatchCounter;
            Database.GetInstance<ApplicationDatabase>().Leagues.LeaguesMatchCounter++;

            Log.WriteLine("incremented to: " + Database.GetInstance<ApplicationDatabase>().Leagues.LeaguesMatchCounter + " with: " + TeamsInTheMatch.Count);

            MatchState = _matchState;

            Log.WriteLine("before MatchReporting ctor" + TeamsInTheMatch.Count);

            MatchReporting = new MatchReporting(TeamsInTheMatch, _interfaceLeague);

            Log.WriteLine("after MatchReporting ctor" + TeamsInTheMatch.Count);

            Log.WriteLine("Constructed a new match with teams ids: " + TeamsInTheMatch.ElementAt(0) +
                TeamsInTheMatch.ElementAt(1) + " with matchId of: " + MatchId, LogLevel.DEBUG);
        }
        catch(Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            throw;
        }
    }

    public ulong[] GetIdsOfThePlayersInTheMatchAsArray(InterfaceLeague _interfaceLeague)
    {
        try
        {
            int playerCounter = 0;

            // Calculate how many users need to be granted roles
            int userAmountToGrantRolesTo = _interfaceLeague.LeaguePlayerCountPerTeam * 2;
            ulong[] allowedUserIds = new ulong[userAmountToGrantRolesTo];

            Log.WriteLine(nameof(allowedUserIds) + " length: " +
                allowedUserIds.Length);

            foreach (var teamKvp in TeamsInTheMatch)
            {
                Log.WriteLine("Looping on team id: " + teamKvp.Key);

                try
                {
                    Team foundTeam = _interfaceLeague.LeagueData.Teams.FindTeamById(teamKvp.Key);

                    foreach (Player player in foundTeam.Players)
                    {
                        allowedUserIds[playerCounter] = player.PlayerDiscordId;
                        Log.WriteLine("Added " + allowedUserIds[playerCounter] + " to: " +
                            nameof(allowedUserIds) + ". " + nameof(playerCounter) + " is now: " +
                            playerCounter + 1 + " out of: " + (allowedUserIds.Length - 1).ToString());

                        playerCounter++;
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.Message, LogLevel.ERROR);
                    continue;
                }
            }

            return allowedUserIds;
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<Response> CreateScheduleSuggestion(ulong _playerId, string _dateAndTime, InterfaceLeague _interfaceLeague)
    {
        try
        {
            Log.WriteLine("Date suggested: " + _dateAndTime + " by: " + _playerId + " with towards id: " +
                ScheduleObject.TeamIdThatRequestedScheduling);

            var defaultEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var playerTeamId = _interfaceLeague.LeagueData.Teams.CheckIfPlayersTeamIsActiveByIdAndReturnThatTeam(_playerId).TeamId;

            DateTime? suggestedScheduleDate = TimeService.GetDateTimeFromUserInput(_dateAndTime);
            if (suggestedScheduleDate == null || !suggestedScheduleDate.HasValue)
            {
                if (_dateAndTime.ToLower() == "accept")
                {
                    return AcceptMatchScheduling(_playerId, playerTeamId, _interfaceLeague);
                }
                else
                {
                    Log.WriteLine("Invalid date suggested: " + _dateAndTime + " by: " + _playerId, LogLevel.DEBUG);
                    return new Response("Invalid date and time format. Please provide a valid date and time.", false);
                }
            }

            TimeSpan timeDifference = suggestedScheduleDate.Value - defaultEpoch;
            ulong scheduledTime = (ulong)timeDifference.TotalSeconds;
            ulong currentTime = TimeService.GetCurrentUnixTime();

            if (currentTime >= scheduledTime)
            {
                return new Response("That time is already past! Please provide a valid date and time.", false);
            }

            ulong timeUntil = scheduledTime - currentTime;
            if (timeUntil <= 0)
            {
                Log.WriteLine("Invalid date suggested: " + _dateAndTime + " by: " + _playerId +
                    " because timeUntil was: " + timeUntil, LogLevel.DEBUG);
                return new Response("The date you tried to suggest was too early!", false);
            }

            if (playerTeamId == ScheduleObject.TeamIdThatRequestedScheduling)
            {
                return new Response("You have already suggested a date for the match!", false);
            }

            InterfaceChannel interfaceChannel = Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryWithCategoryId(
                Database.GetInstance<ApplicationDatabase>().MatchChannelsIdWithCategoryId[MatchChannelId]).FindInterfaceChannelWithIdInTheCategory(
                    MatchChannelId);

            if (scheduledTime == ScheduleObject.RequestedSchedulingTimeInUnixTime)
            {
                await interfaceChannel.DeleteMessagesInAChannelWithMessageName(MessageName.MATCHSCHEDULINGSUGGESTIONMESSAGE);

                //StartMatchAfterSchedulingOnAnotherThread(interfaceChannel, timeUntil);
                return AcceptMatchScheduling(_playerId, playerTeamId, _interfaceLeague);
            }

            var suggestedScheduleDateInUnixTime = TimeService.ConvertDateTimeToUnixTime(suggestedScheduleDate.Value);
            if (AlreadySuggestedTimes.Contains(suggestedScheduleDateInUnixTime))
            {
                return new Response("That time, " + TimeService.ConvertToZuluTimeFromUnixTime(suggestedScheduleDateInUnixTime) +
                    " was already suggested before! Please suggest a new time.", false);
            }
            AlreadySuggestedTimes.Add(suggestedScheduleDateInUnixTime);

            var response = Database.GetInstance<ApplicationDatabase>().Leagues.CheckIfListOfPlayersCanJoinOrSuggestATimeForTheMatchWithTime(
                GetIdsOfThePlayersInTheMatchAsArray(_interfaceLeague).ToList(), suggestedScheduleDateInUnixTime, _playerId, _interfaceLeague).Result;
            if (!response.serialize)
            {
                return response;
            }

            ScheduleObject = new ScheduleObject(suggestedScheduleDateInUnixTime, playerTeamId);

            // Must delete before showing the new message, without the ID being saved in a variable
            await interfaceChannel.DeleteMessagesInAChannelWithMessageName(MessageName.MATCHSCHEDULINGSUGGESTIONMESSAGE);

            var newMessage =
                interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                    MessageName.MATCHSCHEDULINGSUGGESTIONMESSAGE).Result;

            Log.WriteLine("timeUntil: " + timeUntil);

            return new Response("", true);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return new Response(ex.Message, false);
        }
    }

    // Used by /schedule accept command, and the ACCEPTSCHEDULEDTIME-button
    public Response AcceptMatchScheduling(ulong _playerId, int _playerTeamId, InterfaceLeague _interfaceLeague)
    {
        if (ScheduleObject.TeamIdThatRequestedScheduling == 0)
        {
            return new Response("You can't accept a match that hasn't been scheduled!", false);
        }

        if (ScheduleObject.TeamIdThatRequestedScheduling == _playerTeamId)
        {
            return new Response("You have already suggested a date!", false);
        }

        Log.WriteLine("player: " + _playerId + " on team: " + _playerTeamId + " accepted the match.", LogLevel.DEBUG);

        InterfaceChannel _interfaceChannelTemp = Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryWithCategoryId(
            Database.GetInstance<ApplicationDatabase>().MatchChannelsIdWithCategoryId[MatchChannelId]).FindInterfaceChannelWithIdInTheCategory(
                MatchChannelId);

        ulong timeUntilTemp = TimeService.CalculateTimeUntilWithUnixTime(ScheduleObject.RequestedSchedulingTimeInUnixTime);

        new MatchQueueAcceptEvent(
            timeUntilTemp + 900, _interfaceLeague.LeagueCategoryId,
            _interfaceChannelTemp.ChannelId, MatchEventManager.ClassScheduledEvents);

        // Improved response time
        Thread secondThread = new Thread(() => StartMatchAfterSchedulingOnAnotherThread(_interfaceChannelTemp, timeUntilTemp));
        secondThread.Start();

        return new Response("", true);
    }

    public async void StartMatchAfterSchedulingOnAnotherThread(InterfaceChannel _interfaceChannel, ulong _timeUntil)
    {
        try
        {
            Log.WriteLine("Starting the match on second thread on channel after scheduling: " + matchChannelId +
                " with timeUntil: " + _timeUntil);

            var client = BotReference.GetClientRef();

            // Loop through scheduling messages and delete them
            //foreach (ulong messageId in StoredScheduleMessageIds)
            //{
            //    var iMessageChannel = await _interfaceChannel.GetMessageChannelById(client);

            //    var messageToDelete = await iMessageChannel.GetMessageAsync(messageId);
            //    if (messageToDelete == null)
            //    {
            //        Log.WriteLine(nameof(messageToDelete) + " was null!", LogLevel.ERROR);
            //        continue;
            //    }
            //    await messageToDelete.DeleteAsync();
            //}

            MatchState = MatchState.PLAYERREADYCONFIRMATIONPHASE;

            await _interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                MessageName.CONFIRMMATCHENTRYMESSAGE, true);

        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return;
        }
    }

    public async void StartTheMatchOnSecondThread(InterfaceChannel _interfaceChannel)
    {
        Log.WriteLine("Starting the match on second thread on channel: " + _interfaceChannel.ChannelId);

        try
        {
            await _interfaceChannel.DeleteMessagesInAChannelWithMessageName(MessageName.CONFIRMMATCHENTRYMESSAGE);

            await _interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                MessageName.REPORTINGSTATUSMESSAGE, true);
            await _interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                MessageName.REPORTINGMESSAGE, true);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return;
        }

        await SerializationManager.SerializeDB();
    }

    public async void FinishTheMatch(InterfaceLeague _interfaceLeague)
    {
        try
        {
            MatchState = MatchState.MATCHDONE;

            AttachmentData[] attachmentDatas;
            InterfaceMessage interfaceMessage;

            Log.WriteLine("Finishing match: " + MatchId, LogLevel.DEBUG);

            // Refactor to the teamsearcht o inside the method?
            EloSystem.CalculateFinalStatsAndEloForBothTeams(
                _interfaceLeague, MatchReporting.FindTeamsInTheMatch(_interfaceLeague),
                MatchReporting.TeamIdsWithReportData.ToDictionary(x => x.Key, x => x.Value));

            Log.WriteLine("Final result for the confirmation was null, but during player removal", LogLevel.DEBUG);

            InterfaceChannel interfaceChannel = Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryWithCategoryId(
                _interfaceLeague.LeagueCategoryId).FindInterfaceChannelWithIdInTheCategory(
                    MatchChannelId);

            interfaceMessage = await interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                    MessageName.MATCHFINALRESULTMESSAGE, false);

            var matchFinalResultMessage = interfaceMessage as MATCHFINALRESULTMESSAGE;
            if (matchFinalResultMessage == null)
            {
                Log.WriteLine(nameof(matchFinalResultMessage) + " was null!", LogLevel.ERROR);
                return;
            }

            Log.WriteLine("altMsg: " + matchFinalResultMessage.AlternativeMessage, LogLevel.DEBUG);

            MatchReporting.FinalResultForConfirmation = interfaceMessage.MessageDescription;
            MatchReporting.FinalMessageForMatchReportingChannel = matchFinalResultMessage.AlternativeMessage;
            MatchReporting.FinalResultTitleForConfirmation = interfaceMessage.MessageEmbedTitle;

            attachmentDatas = TacviewManager.FindTacviewAttachmentsForACertainMatch(
               MatchId, _interfaceLeague).Result;

            if (MatchReporting.FinalMessageForMatchReportingChannel == null)
            {
                Log.WriteLine(nameof(MatchReporting) + " FinalMessageForMatchReportingChannel was null!", LogLevel.ERROR);
                return;
            }

            if (MatchReporting.FinalResultTitleForConfirmation == null)
            {
                Log.WriteLine(nameof(MatchReporting) + " matchReporting.FinalResultTitleForConfirmation was null!", LogLevel.ERROR);
                return;
            }

            Log.WriteLine("finalMsg: " + MatchReporting.FinalMessageForMatchReportingChannel, LogLevel.DEBUG);

            await _interfaceLeague.PostMatchReport(
                MatchReporting.FinalMessageForMatchReportingChannel, MatchReporting.FinalResultTitleForConfirmation, attachmentDatas);

            AttemptToPutTheTeamsBackToTheQueueAfterTheMatch(_interfaceLeague);

            ulong matchChannelDeleteDelay = 45;

            // Schedule an event to delete the channel later
            new DeleteChannelEvent(
                matchChannelDeleteDelay, _interfaceLeague.LeagueCategoryId,
                MatchChannelId, "match", _interfaceLeague.LeagueEventManager.ClassScheduledEvents);

            var messageToModify = interfaceChannel.FindInterfaceMessageWithNameInTheChannel(MessageName.CONFIRMATIONMESSAGE);
            messageToModify.GenerateAndModifyTheMessageAsync();

            //await interfaceChannel.DeleteThisChannel(_interfaceLeague.LeagueCategoryId, "match", matchChannelDeleteDelay);

            int matchIdTemp = MatchId;

            Database.GetInstance<ApplicationDatabase>().ArchivedLeagueMatches.Add(
                await _interfaceLeague.LeagueData.Matches.FindMatchAndRemoveItFromConcurrentBag(MatchChannelId, _interfaceLeague));
            Log.WriteLine("Added " + matchIdTemp + " to the archive, count is now: " +
                Database.GetInstance<ApplicationDatabase>().ArchivedLeagueMatches.Count, LogLevel.DEBUG);

            // When removing the player from the database, no need for this because it's done after he is gone from the league
            //if (!_removingPlayerFromDatabase)
            //{
            _interfaceLeague.UpdateLeagueLeaderboard();
            //}

            await SerializationManager.SerializeDB();
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return;
        }
    }

    public void AttemptToPutTheTeamsBackToTheQueueAfterTheMatch(InterfaceLeague _interfaceLeague)
    {
        // Defined when the match is created
        if (IsAScheduledMatch)
        {
            // Place the teams back in to the queue
            foreach (var teamId in MatchReporting.TeamIdsWithReportData)
            {
                if (!_interfaceLeague.LeagueData.MatchScheduler.TeamsInTheMatchmaker.ContainsKey(teamId.Key))
                {
                    Log.WriteLine("Does not contain key: " + teamId + ", left from the matchmaker?", LogLevel.DEBUG);
                    continue;
                }

                _interfaceLeague.LeagueData.MatchScheduler.TeamsInTheMatchmaker[teamId.Key].TeamMatchmakingState = TeamMatchmakingState.INQUEUE;

                Log.WriteLine("Set team's " + teamId + " to in queue.", LogLevel.DEBUG);
            }
        }
    }

    public string GenerateFinalMentionMessage(InterfaceLeague _interfaceLeague, bool _mentionOtherTeamsPlayers)
    {
        string finalMentionMessage = "";
        ulong[] playerIdsInTheMatch = GetIdsOfThePlayersInTheMatchAsArray(_interfaceLeague);
        foreach (ulong playerId in playerIdsInTheMatch)
        {
            // Skip pinging the team that doesn't need to be pinged (such as when received Schedule request)
            if (_mentionOtherTeamsPlayers &&
                _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(playerId).TeamId ==
                ScheduleObject.TeamIdThatRequestedScheduling)
            {
                continue;
            }

            finalMentionMessage += "<@" + playerId.ToString() + "> ";
        }
        return finalMentionMessage;
    }
}