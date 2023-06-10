using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

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

    [DataMember] private logConcurrentDictionary<int, string> teamsInTheMatch = new logConcurrentDictionary<int, string>();
    [DataMember] private logClass<int> matchId = new logClass<int>();
    [DataMember] private logClass<ulong> matchChannelId = new logClass<ulong>();
    [DataMember] private logClass<MatchReporting> matchReporting = new logClass<MatchReporting>(new MatchReporting());
    [DataMember] private logClass<LeagueName> matchLeague = new logClass<LeagueName>(new LeagueName());
    [DataMember] private logClass<ScheduleObject> scheduleObject = new logClass<ScheduleObject>(new ScheduleObject());

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
    public LeagueMatch(InterfaceLeague _interfaceLeague, int[] _teamsToFormMatchOn, MatchState _matchState)
    {
        SetInterfaceLeagueReferencesForTheMatch(_interfaceLeague);

        int leagueTeamSize = _interfaceLeague.LeaguePlayerCountPerTeam;
        MatchLeague = _interfaceLeague.LeagueCategoryName;

        Log.WriteLine("Teams to from the match on: " + _teamsToFormMatchOn[0] +
            ", " + _teamsToFormMatchOn[1], LogLevel.DEBUG);

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
            catch(Exception ex)
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
            DateTime currentTime = await TimeService.GetCurrentTime();

            Log.WriteLine("Date suggested: " + _dateAndTime + " by: " + _playerId + " on: " + currentTime.ToString(), LogLevel.VERBOSE);
            // Convert the input date and time string to a DateTime object
            if (!DateTime.TryParse(_dateAndTime, out DateTime suggestedScheduleDate))
            {
                Log.WriteLine("Invalid date suggested: " + _dateAndTime + " by: " + _playerId, LogLevel.DEBUG);
                return new Response("Invalid date and time format. Please provide a valid date and time.", false);
            }

            Log.WriteLine("Valid Datetime: " + suggestedScheduleDate.ToLongTimeString() + " by: " + _playerId, LogLevel.VERBOSE);

            int timeUntil = TimeService.CalculateTimeUntil(currentTime, suggestedScheduleDate);

            Log.WriteLine("Time until: " + timeUntil, LogLevel.VERBOSE);

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

            scheduleObject = new logClass<ScheduleObject>(new ScheduleObject(suggestedScheduleDate, playerTeamId)).GetValue();

            StartMatchAfterScheduling(_interfaceChannel, timeUntil);

            return new Response("Scheduled match to: " + suggestedScheduleDate, true);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return new Response(ex.Message, false);
        }
    }

    public async void StartMatchAfterScheduling(InterfaceChannel _interfaceChannel, int _timeUntil)
    {
        Log.WriteLine("Starting the match on second thread on channel after scheduling: " + matchChannelId +
            " with timeUntil: " + _timeUntil, LogLevel.VERBOSE);

        try
        {
            // Delete the scheduling messages here
            //await _interfaceChannel.DeleteMessagesInAChannelWithMessageName(MessageName.CONFIRMMATCHENTRYMESSAGE);

            MatchReporting.MatchState = MatchState.PLAYERREADYCONFIRMATIONPHASE;

            new MatchQueueAcceptEvent(_timeUntil, interfaceLeagueRef.LeagueCategoryId, _interfaceChannel.ChannelId);

            await _interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                MessageName.CONFIRMMATCHENTRYMESSAGE, true);
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

            int matchChannelDeleteDelay = 45;

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
}