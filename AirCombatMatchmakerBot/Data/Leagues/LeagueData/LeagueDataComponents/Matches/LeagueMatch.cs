using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using System;
using System.Globalization;

[DataContract]
public class LeagueMatch : logClass<LeagueMatch>
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

    public MatchReporting MatchReporting
    {
        get => matchReporting.GetValue();
        set => matchReporting.SetValue(value);
    }

    public LeagueName MatchLeague
    {
        get => matchLeague.GetValue();
        set => matchLeague.SetValue(value);
    }

    public ScheduleObject ScheduleObject
    {
        get => scheduleObject.GetValue();
        set => scheduleObject.SetValue(value);
    }

    public bool IsAScheduledMatch
    {
        get => isAScheduledMatch.GetValue();
        set => isAScheduledMatch.SetValue(value);
    }

    [DataMember] private logConcurrentDictionary<int, string> teamsInTheMatch = new logConcurrentDictionary<int, string>();
    [DataMember] private logClass<int> matchId = new logClass<int>();
    [DataMember] private logClass<ulong> matchChannelId = new logClass<ulong>();
    [DataMember] private logClass<MatchReporting> matchReporting = new logClass<MatchReporting>(new MatchReporting());
    [DataMember] private logClass<LeagueName> matchLeague = new logClass<LeagueName>(new LeagueName());
    [DataMember] private logClass<ScheduleObject> scheduleObject = new logClass<ScheduleObject>(new ScheduleObject());
    [DataMember] private logClass<bool> isAScheduledMatch = new logClass<bool>();

    private InterfaceLeague interfaceLeagueRef;

    public LeagueMatch() { }

    public void SetInterfaceLeagueReferencesForTheMatch(InterfaceLeague _interfaceLeagueRef)
    {
        if (_interfaceLeagueRef == null)
        {
            Log.WriteLine(_interfaceLeagueRef.ToString(), LogLevel.WARNING);
        }

        interfaceLeagueRef = _interfaceLeagueRef;
        MatchReporting.interfaceLeagueRef = _interfaceLeagueRef;

        Log.WriteLine(MatchReporting.interfaceLeagueRef.ToString(), LogLevel.VERBOSE);

        Log.WriteLine("Set:", LogLevel.VERBOSE);
    }

    // TODO: Add interfaceLeague ref on constructor as a reference
    public LeagueMatch(InterfaceLeague _interfaceLeague, int[] _teamsToFormMatchOn,
        MatchState _matchState, bool _attemptToPutTeamsBackToQueueAfterTheMatch = false)
    {
        IsAScheduledMatch = _attemptToPutTeamsBackToQueueAfterTheMatch;
        SetInterfaceLeagueReferencesForTheMatch(_interfaceLeague);

        int leagueTeamSize = _interfaceLeague.LeaguePlayerCountPerTeam;
        MatchLeague = _interfaceLeague.LeagueCategoryName;

        Log.WriteLine("Teams to from the match on: " + _teamsToFormMatchOn[0] +
            ", " + _teamsToFormMatchOn[1] + " scheduled match: " + _attemptToPutTeamsBackToQueueAfterTheMatch, LogLevel.DEBUG);

        // Add the team's name to the ConcurrentDictionary as a value
        foreach (int teamId in _teamsToFormMatchOn)
        {
            Team foundTeam =
                _interfaceLeague.LeagueData.Teams.FindTeamById(teamId);

            Log.WriteLine("Found team: " + foundTeam.TeamId, LogLevel.DEBUG);

            TeamsInTheMatch.TryAdd(teamId, foundTeam.GetTeamInAString(false, leagueTeamSize));

            Log.WriteLine("Count is now: " + TeamsInTheMatch.Count, LogLevel.DEBUG);
        }

        foreach (var item in TeamsInTheMatch)
        {
            Log.WriteLine("final teamsInTheMatch: " + item.Key, LogLevel.DEBUG);
        }

        MatchId = Database.Instance.Leagues.LeaguesMatchCounter;
        Database.Instance.Leagues.LeaguesMatchCounter++;

        MatchReporting = new MatchReporting(TeamsInTheMatch, _interfaceLeague, _matchState);

        Log.WriteLine("Constructed a new match with teams ids: " + TeamsInTheMatch.ElementAt(0) +
            TeamsInTheMatch.ElementAt(1) + " with matchId of: " + MatchId, LogLevel.DEBUG);
    }

    public ulong[] GetIdsOfThePlayersInTheMatchAsArray()
    {
        int playerCounter = 0;

        // Calculate how many users need to be granted roles
        int userAmountToGrantRolesTo = interfaceLeagueRef.LeaguePlayerCountPerTeam * 2;
        ulong[] allowedUserIds = new ulong[userAmountToGrantRolesTo];

        Log.WriteLine(nameof(allowedUserIds) + " length: " +
            allowedUserIds.Length, LogLevel.VERBOSE);

        foreach (var teamKvp in TeamsInTheMatch)
        {
            Log.WriteLine("Looping on team id: " + teamKvp.Key, LogLevel.VERBOSE);

            try
            {
                Team foundTeam = interfaceLeagueRef.LeagueData.Teams.FindTeamById(teamKvp.Key);

                foreach (Player player in foundTeam.Players)
                {
                    allowedUserIds[playerCounter] = player.PlayerDiscordId;
                    Log.WriteLine("Added " + allowedUserIds[playerCounter] + " to: " +
                        nameof(allowedUserIds) + ". " + nameof(playerCounter) + " is now: " +
                        playerCounter + 1 + " out of: " + (allowedUserIds.Length - 1).ToString(), LogLevel.VERBOSE);

                    playerCounter++;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                continue;
            }
        }

        return allowedUserIds;
    }

    public async Task<Response> CreateScheduleSuggestion(ulong _playerId, string _dateAndTime)
    {
        try
        {
            Log.WriteLine("Date suggested: " + _dateAndTime + " by: " + _playerId + " with towards id: " +
                ScheduleObject.TeamIdThatRequestedScheduling, LogLevel.VERBOSE);

            DateTime suggestedScheduleDate;

            // Parse the input date and time string
            if (_dateAndTime.ToLower().StartsWith("today "))
            {
                string timeString = _dateAndTime.Substring(6);
                DateTime currentDate = DateTime.UtcNow.Date;
                if (!TimeSpan.TryParseExact(timeString, new[] {
                    @"hh\:mm\:ss'z'", @"hh\:mm'z'", @"hh'z'",
                    @"hhmmss'z'", @"hhmm'z'"

                }, CultureInfo.InvariantCulture, out TimeSpan timeComponent))
                {
                    return new Response("Invalid time format. Please provide a valid time.", false);
                }
                suggestedScheduleDate = currentDate.Add(timeComponent);
            }
            else if (_dateAndTime.ToLower().StartsWith("tomorrow "))
            {
                string timeString = _dateAndTime.Substring(9);
                DateTime tomorrowDate = DateTime.UtcNow.Date.AddDays(1);
                if (!TimeSpan.TryParseExact(timeString, new[] { 
                    @"hh\:mm\:ss'z'", @"hh\:mm'z'", @"hh'z'", 
                    @"hhmmss'z'", @"hhmm'z'"
                }, CultureInfo.InvariantCulture, out TimeSpan timeComponent))
                {
                    return new Response("Invalid time format. Please provide a valid time.", false);
                }
                suggestedScheduleDate = tomorrowDate.Add(timeComponent);
            }
            else
            {
                if (!DateTime.TryParseExact(_dateAndTime, new[] {
                    "dd/MM/yyyy HH:mm:ss'z'", "dd/MM/yyyy HH:mm'z'", "dd/MM/yyyy HH'z'",
                    "dd/MM/yyyy HHmmss'z'", "dd/MM/yyyy HHmm'z'",
                    "dd.MM.yyyy HH:mm:ss'z'", "dd.MM.yyyy HH:mm'z'", "dd.MM.yyyy HH'z'",
                    "dd.MM.yyyy HHmmss'z'", "dd.MM.yyyy HHmm'z'",
                },
                CultureInfo.InvariantCulture, DateTimeStyles.None, out suggestedScheduleDate))
                {
                    return new Response("Invalid date and time format. Please provide a valid date and time.", false);
                }
            }

            // Assume the year is the current year if not provided
            if (suggestedScheduleDate.Year == 1)
            {
                suggestedScheduleDate = suggestedScheduleDate.AddYears(DateTime.UtcNow.Year - 1);
            }

            Log.WriteLine("Valid DateTime: " + suggestedScheduleDate.ToLongTimeString() + " by: " + _playerId, LogLevel.VERBOSE);

            bool isValidDateAndTime = true;
            ulong scheduledTime = (ulong)(suggestedScheduleDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            ulong currentTime = (ulong)(
                TimeService.GetCurrentTime().Result -
                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            ulong timeUntil = scheduledTime - currentTime;

            if (!isValidDateAndTime && _dateAndTime.ToLower() != "accept")
            {
                Log.WriteLine("Invalid date suggested: " + _dateAndTime + " by: " + _playerId, LogLevel.DEBUG);
                return new Response("Invalid date and time format. Please provide a valid date and time.", false);
            }
            else if (!isValidDateAndTime && _dateAndTime.ToLower() == "accept" && ScheduleObject.TeamIdThatRequestedScheduling != 0)
            {
                var playerTeamIdTemp = interfaceLeagueRef.LeagueData.Teams.CheckIfPlayersTeamIsActiveByIdAndReturnThatTeam(
                    _playerId).TeamId;

                if (ScheduleObject.TeamIdThatRequestedScheduling == playerTeamIdTemp)
                {
                    return new Response("You have already suggested a date which is: " +
                        ScheduleObject.GetValue().RequestedSchedulingTimeInUnixTime, false);
                }

                Log.WriteLine("player: " + playerTeamIdTemp + " on team: " + playerTeamIdTemp + " accepted the match.", LogLevel.DEBUG);

                InterfaceChannel _interfaceChannelTemp = Database.Instance.Categories.FindInterfaceCategoryWithId(
                    Database.Instance.Categories.MatchChannelsIdWithCategoryId[MatchChannelId]).FindInterfaceChannelWithIdInTheCategory(
                        MatchChannelId);

                ulong timeUntilTemp = TimeService.CalculateTimeUntilWithUnixTime(ScheduleObject.RequestedSchedulingTimeInUnixTime);

                await StartMatchAfterScheduling(_interfaceChannelTemp, timeUntilTemp);

                return new Response("Scheduled match to: " + ScheduleObject.RequestedSchedulingTimeInUnixTime, true);
            }
            else if (!isValidDateAndTime && _dateAndTime.ToLower() == "accept" && ScheduleObject.TeamIdThatRequestedScheduling == 0)
            {
                return new Response("You can't accept a match that hasn't been scheduled!", false);
            }

            if (timeUntil <= 0)
            {
                Log.WriteLine("Invalid date suggested: " + _dateAndTime + " by: " + _playerId +
                    " because timeUntil was: " + timeUntil, LogLevel.DEBUG);
                return new Response("The date you tried to suggest was too early!", false);
            }

            InterfaceChannel _interfaceChannel = Database.Instance.Categories.FindInterfaceCategoryWithId(
                Database.Instance.Categories.MatchChannelsIdWithCategoryId[MatchChannelId]).FindInterfaceChannelWithIdInTheCategory(
                    MatchChannelId);

            var playerTeamId = interfaceLeagueRef.LeagueData.Teams.CheckIfPlayersTeamIsActiveByIdAndReturnThatTeam(_playerId).TeamId;

            if (playerTeamId == ScheduleObject.TeamIdThatRequestedScheduling)
            {
                return new Response("You have already suggested a date!", false);
            }

            if (scheduledTime == ScheduleObject.RequestedSchedulingTimeInUnixTime)
            {
                await StartMatchAfterScheduling(_interfaceChannel, timeUntil);
                return new Response("Accepted scheduled match to: " + suggestedScheduleDate, true);
            }

            ScheduleObject = new logClass<ScheduleObject>(new ScheduleObject(suggestedScheduleDate, playerTeamId)).GetValue();

            return new Response("Scheduled match to: " + suggestedScheduleDate, true);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return new Response(ex.Message, false);
        }
    }


    public async Task StartMatchAfterScheduling(InterfaceChannel _interfaceChannel, ulong _timeUntil)
    {
        Log.WriteLine("Starting the match on second thread on channel after scheduling: " + matchChannelId +
            " with timeUntil: " + _timeUntil, LogLevel.VERBOSE);

        try
        {
            // Delete the scheduling messages here
            //await _interfaceChannel.DeleteMessagesInAChannelWithMessageName(MessageName.CONFIRMMATCHENTRYMESSAGE);

            MatchReporting.MatchState = MatchState.PLAYERREADYCONFIRMATIONPHASE;

            new MatchQueueAcceptEvent(_timeUntil + 900, interfaceLeagueRef.LeagueCategoryId, _interfaceChannel.ChannelId);

            await _interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                MessageName.CONFIRMMATCHENTRYMESSAGE, true);

            return;
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }
    }

    public async void StartTheMatchOnSecondThread(InterfaceChannel _interfaceChannel)
    {
        Log.WriteLine("Starting the match on second thread on channel: " + _interfaceChannel.ChannelId, LogLevel.VERBOSE);

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
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }

        await SerializationManager.SerializeDB();
    }

    public async void FinishTheMatch()
    {
        MatchReporting.MatchState = MatchState.MATCHDONE;

        AttachmentData[] attachmentDatas;
        InterfaceMessage interfaceMessage;

        Log.WriteLine("Finishing match: " + MatchId, LogLevel.DEBUG);
        EloSystem.CalculateFinalStatsAndEloForBothTeams(
            interfaceLeagueRef, MatchReporting.FindTeamsInTheMatch(),
            MatchReporting.TeamIdsWithReportData.ToDictionary(x => x.Key, x => x.Value));

        Log.WriteLine("Final result for the confirmation was null, but during player removal", LogLevel.DEBUG);

        InterfaceChannel interfaceChannel = Database.Instance.Categories.FindInterfaceCategoryWithId(
            interfaceLeagueRef.LeagueCategoryId).FindInterfaceChannelWithIdInTheCategory(
                MatchChannelId);

        try
        {
            interfaceMessage = await interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                MessageName.MATCHFINALRESULTMESSAGE, false);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }

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

        try
        {
            attachmentDatas = TacviewManager.FindTacviewAttachmentsForACertainMatch(
               MatchId, interfaceLeagueRef).Result;

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

            await interfaceLeagueRef.PostMatchReport(
                MatchReporting.FinalMessageForMatchReportingChannel, MatchReporting.FinalResultTitleForConfirmation, attachmentDatas);

            AttemptToPutTheTeamsBackToTheQueueAfterTheMatch();

            ulong matchChannelDeleteDelay = 45;

            // Schedule an event to delete the channel later
            new DeleteChannelEvent(matchChannelDeleteDelay, interfaceLeagueRef.LeagueCategoryId, MatchChannelId, "match");

            var messageToModify = interfaceChannel.FindInterfaceMessageWithNameInTheChannel(MessageName.CONFIRMATIONMESSAGE);
            await messageToModify.GenerateAndModifyTheMessage();

            //await interfaceChannel.DeleteThisChannel(_interfaceLeague.LeagueCategoryId, "match", matchChannelDeleteDelay);

            int matchIdTemp = MatchId;

            Database.Instance.ArchivedLeagueMatches.Add(
                await interfaceLeagueRef.LeagueData.Matches.FindMatchAndRemoveItFromConcurrentBag(MatchChannelId));
            Log.WriteLine("Added " + matchIdTemp + " to the archive, count is now: " +
                Database.Instance.ArchivedLeagueMatches.Count, LogLevel.DEBUG);

            // When removing the player from the database, no need for this because it's done after he is gone from the league
            //if (!_removingPlayerFromDatabase)
            //{
            interfaceLeagueRef.UpdateLeagueLeaderboard();
            //}

            await SerializationManager.SerializeDB();
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }
    }

    public void AttemptToPutTheTeamsBackToTheQueueAfterTheMatch()
    {
        // Defined when the match is created
        if (IsAScheduledMatch)
        {
            // Place the teams back in to the queue
            foreach (var teamId in MatchReporting.TeamIdsWithReportData)
            {
                if (!interfaceLeagueRef.LeagueData.MatchScheduler.TeamsInTheMatchmaker.ContainsKey(teamId.Key))
                {
                    Log.WriteLine("Does not contain key: " + teamId + ", left from the matchmaker?", LogLevel.DEBUG);
                    continue;
                }

                interfaceLeagueRef.LeagueData.MatchScheduler.TeamsInTheMatchmaker[teamId.Key].TeamMatchmakingState = TeamMatchmakingState.INQUEUE;

                Log.WriteLine("Set team's " + teamId + " to in queue.", LogLevel.DEBUG);
            }
        }
    }
}