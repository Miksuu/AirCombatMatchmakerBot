using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class MatchReporting : logClass<MatchReporting>, InterfaceLoggableClass
{
    public ConcurrentDictionary<int, ReportData> TeamIdsWithReportData
    {
        get => teamIdsWithReportData.GetValue();
        set => teamIdsWithReportData.SetValue(value);
    }

    public bool ShowingConfirmationMessage
    {
        get => showingConfirmationMessage.GetValue();
        set => showingConfirmationMessage.SetValue(value);
    }

    public bool MatchDone
    {
        get => matchDone.GetValue();
        set => matchDone.SetValue(value);
    }

    public string FinalResultForConfirmation
    {
        get => finalResultForConfirmation.GetValue();
        set => finalResultForConfirmation.SetValue(value);
    }

    public string FinalMessageForMatchReportingChannel
    {
        get => finalMessageForMatchReportingChannel.GetValue();
        set => finalMessageForMatchReportingChannel.SetValue(value);
    }

    public string FinalResultTitleForConfirmation
    {
        get => finalResultTitleForConfirmation.GetValue();
        set => finalResultTitleForConfirmation.SetValue(value);
    }

    [DataMember] private logConcurrentDictionary<int, ReportData> teamIdsWithReportData = new logConcurrentDictionary<int, ReportData>();
    [DataMember] private logClass<bool> showingConfirmationMessage = new logClass<bool>();
    [DataMember] private logClass<bool> matchDone = new logClass<bool>();
    [DataMember] private logString finalResultForConfirmation = new logString();
    [DataMember] private logString finalMessageForMatchReportingChannel = new logString();
    [DataMember] private logString finalResultTitleForConfirmation = new logString();

    public List<string> GetClassParameters()
    {
        return new List<string> { teamIdsWithReportData.GetLoggingClassParameters<int, ReportData>(),
        showingConfirmationMessage.GetParameter(), matchDone.GetParameter(), finalResultForConfirmation.GetValue(),
            finalMessageForMatchReportingChannel.GetValue(), finalResultForConfirmation.GetValue() };
    }

    public MatchReporting() { }

    public MatchReporting(ConcurrentDictionary<int, string> _teamsInTheMatch)
    {
        foreach (var teamKvp in _teamsInTheMatch)
        {
            if (TeamIdsWithReportData.ContainsKey(teamKvp.Key))
            {
                Log.WriteLine("Already contains the key: " + teamKvp.Key, LogLevel.DEBUG);
                continue;
            }

            Log.WriteLine("Adding team: " + teamKvp.Key + " | " + teamKvp.Value, LogLevel.VERBOSE);

            TeamIdsWithReportData.TryAdd(teamKvp.Key, new ReportData(teamKvp.Value));
        }
    }

    public async Task<Response> ProcessPlayersSentReportObject(
        InterfaceLeague _interfaceLeague, ulong _playerId, string _reportedObjectByThePlayer,
        TypeOfTheReportingObject _typeOfTheReportingObject, ulong _leagueCategoryId, ulong _messageChannelId)
    {
        string response = string.Empty;

        Log.WriteLine("Processing player's sent " + nameof(ReportObject) + " in league: " +
            _interfaceLeague.LeagueCategoryName + " by: " + _playerId + " with data: " +
            _reportedObjectByThePlayer + " of type: " + _typeOfTheReportingObject, LogLevel.DEBUG);

        if (MatchDone)
        {
            Log.WriteLine(_playerId + " requested to report the match," +
                " when it was already over.", LogLevel.VERBOSE);
            return Task.FromResult(new Response("Match is already done!", false)).Result;
        }

        // Can receive comments still even though the the confirmation is under way
        if (ShowingConfirmationMessage && _typeOfTheReportingObject == TypeOfTheReportingObject.REPORTEDSCORE)
        {
            Log.WriteLine(_playerId + " requested to report the match," +
                " when it was already in confirmation.", LogLevel.VERBOSE);
            return Task.FromResult(new Response("Match is in confirmation already! Finish that first, ", false)).Result;
        }

        Team? reportingTeam =
            _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(
                _playerId);
        if (reportingTeam == null)
        {
            Log.WriteLine(nameof(reportingTeam) + " was null! with playerId: " + _playerId, LogLevel.CRITICAL);
            return Task.FromResult(new Response(response, false)).Result;
        }

        // First time pressing the report button for the team
        if (!TeamIdsWithReportData.ContainsKey(reportingTeam.TeamId))
        {
            Log.WriteLine("Key wasn't found! by player:" + _playerId, LogLevel.WARNING);
            return new Response("", false);
        }
        // Replacing the result
        else
        {
            Log.WriteLine("Key was, the team is not their first time reporting.", LogLevel.VERBOSE);

            switch (_typeOfTheReportingObject)
            {
                case TypeOfTheReportingObject.REPORTEDSCORE:
                    TeamIdsWithReportData[reportingTeam.TeamId].ReportedScore.SetObjectValueAndFieldBool(
                        _reportedObjectByThePlayer, EmojiName.WHITECHECKMARK);
                    response = "You reported score of: " + _reportedObjectByThePlayer;
                    break;
                case TypeOfTheReportingObject.TACVIEWLINK:
                    TeamIdsWithReportData[reportingTeam.TeamId].TacviewLink.SetObjectValueAndFieldBool(
                        _reportedObjectByThePlayer, EmojiName.WHITECHECKMARK);

                    // Makes the other tacview submission to be optional
                    foreach (var item in TeamIdsWithReportData)
                    {
                        if (item.Key != reportingTeam.TeamId)
                        {
                            if (item.Value.TacviewLink.CurrentStatus == EmojiName.REDSQUARE)
                            {
                                item.Value.TacviewLink.SetObjectValueAndFieldBool(
                                    item.Value.TacviewLink.ObjectValue, EmojiName.YELLOWSQUARE);
                            }
                        } 
                    }

                    response = "You posted tacview link: " + _reportedObjectByThePlayer;
                    break;
                case TypeOfTheReportingObject.COMMENTBYTHEUSER:
                    TeamIdsWithReportData[reportingTeam.TeamId].CommentByTheUser.SetObjectValueAndFieldBool(
                        _reportedObjectByThePlayer, EmojiName.WHITECHECKMARK);
                    break;
                default:
                    Log.WriteLine("Unknown type! (not implemented?)", LogLevel.CRITICAL);
                    response = "Unknown type: " + _reportedObjectByThePlayer + "(not implemented?)";
                    break;
            }
        }

        InterfaceChannel interfaceChannel = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _leagueCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                    _messageChannelId);

        // If the match is on the confirmation phase,
        // edit that MessageDescription instead of the reporting status MessageDescription which would be null
        MessageName messageNameToEdit = MessageName.REPORTINGSTATUSMESSAGE;
        if (ShowingConfirmationMessage)
        {
            messageNameToEdit = MessageName.MATCHFINALRESULTMESSAGE;

            var interfaceMessage = interfaceChannel.FindInterfaceMessageWithNameInTheChannel(
                MessageName.MATCHFINALRESULTMESSAGE);
            if (interfaceMessage == null)
            {
                Log.WriteLine(nameof(interfaceMessage) + " was null!", LogLevel.CRITICAL);
                return new Response(nameof(interfaceMessage) + " was null!", false);
            }
            FinalResultForConfirmation = interfaceMessage.GenerateMessage();
            // Must be called after GenerateMessage() since it's defined there
            FinalResultTitleForConfirmation = interfaceMessage.MessageEmbedTitle;
        }

        InterfaceMessage? messageToEdit = interfaceChannel.FindInterfaceMessageWithNameInTheChannel(
                        messageNameToEdit);
        if (messageToEdit == null)
        {
            string errorMsg = nameof(messageToEdit) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return new Response(errorMsg, false);
        }
        await messageToEdit.GenerateAndModifyTheMessage();

        foreach (var reportedTeamKvp in TeamIdsWithReportData)
        {
            Log.WriteLine("Reported team: " + reportedTeamKvp.Key +
                " with value: " + reportedTeamKvp.Value, LogLevel.VERBOSE);
        }

        int reportedTeamsCount = TeamIdsWithReportData.Count;

        Log.WriteLine("Reported teams count: " + reportedTeamsCount, LogLevel.VERBOSE);

        if (reportedTeamsCount > 2)
        {
            Log.WriteLine("Count was: " + reportedTeamsCount + ", Error!", LogLevel.ERROR);

            // Maybe handle the error
            return Task.FromResult(new Response("Couldn't post comment", false)).Result;
        }

        return Task.FromResult(new Response(response, true)).Result;
    }

    public async Task<Response> PrepareFinalMatchResult(
        InterfaceLeague _interfaceLeague, ulong _playerId,
         ulong _leagueCategoryId, ulong _messageChannelId)
    {
        string response = string.Empty;

        (string, bool, InterfaceChannel) responseTuple = CheckIfMatchCanBeSentToConfirmation(_leagueCategoryId, _messageChannelId).Result;
        if (responseTuple.Item3 == null)
        {
            Log.WriteLine(nameof(responseTuple.Item3) + " was null! with playerId: " + _playerId, LogLevel.CRITICAL);
            return new Response(response, false);
        }

        response = responseTuple.Item1;
        InterfaceChannel interfaceChannel = responseTuple.Item3;

        if (responseTuple.Item2)
        {
            CalculateFinalMatchResult(_interfaceLeague);

            Log.WriteLine("Creating new messages from: " + _playerId, LogLevel.DEBUG);

            var interfaceMessage = await interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                MessageName.MATCHFINALRESULTMESSAGE);
            if (interfaceMessage == null)
            {
                string errorMsg = nameof(interfaceMessage) + " was null!";
                Log.WriteLine(errorMsg, LogLevel.CRITICAL);
                return new Response(errorMsg, false);
            }

            MATCHFINALRESULTMESSAGE? finalResultMessage = interfaceMessage as MATCHFINALRESULTMESSAGE;
            if (finalResultMessage == null)
            {
                string errorMsg = nameof(finalResultMessage) + " was null!";
                Log.WriteLine(errorMsg, LogLevel.CRITICAL);
                return new Response(errorMsg, false);
            }

            FinalResultForConfirmation = interfaceMessage.MessageDescription;
            FinalMessageForMatchReportingChannel = finalResultMessage.AlternativeMessage;
            FinalResultTitleForConfirmation = interfaceMessage.MessageEmbedTitle;

            await interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                MessageName.CONFIRMATIONMESSAGE);

            var interfaceCategory = _interfaceLeague.FindLeaguesInterfaceCategory();
            if (interfaceCategory == null)
            {
                string errorMsg = nameof(interfaceCategory) + " was null!";
                Log.WriteLine(errorMsg, LogLevel.CRITICAL);
                return new Response(errorMsg, false);
            }

            // Copypasted to MODIFYMATCHBUTTON.CS, maybe replace to method
            InterfaceChannel interfaceChannelToDeleteTheMessageIn =
                interfaceCategory.FindInterfaceChannelWithIdInTheCategory(
                    _messageChannelId);
            if (interfaceChannelToDeleteTheMessageIn == null)
            {
                Log.WriteLine(nameof(interfaceChannelToDeleteTheMessageIn) + " was null, with: " +
                    _messageChannelId, LogLevel.CRITICAL);
                return new Response(nameof(interfaceChannelToDeleteTheMessageIn) + " was null", false);
            }

            // Delete the messages which aren't useful for confirmation phase anymore
            await interfaceChannelToDeleteTheMessageIn.DeleteMessagesInAChannelWithMessageName(
                MessageName.REPORTINGMESSAGE);
            await interfaceChannelToDeleteTheMessageIn.DeleteMessagesInAChannelWithMessageName(
                MessageName.REPORTINGSTATUSMESSAGE);
        }

        Log.WriteLine("returning response: " + response, LogLevel.VERBOSE);

        return new Response(response, true);
    }

    private Task<(string, bool, InterfaceChannel)>? CheckIfMatchCanBeSentToConfirmation(
        ulong _leagueCategoryId, ulong _messageChannelId)
    {
        bool confirmationMessageCanBeShown = CheckIfConfirmationMessageCanBeShown();

        InterfaceChannel? interfaceChannel = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            _leagueCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(_messageChannelId);
        if (interfaceChannel == null)
        {
            Log.WriteLine(nameof(interfaceChannel) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Message can be shown: " + confirmationMessageCanBeShown +
            " showing: " + ShowingConfirmationMessage, LogLevel.DEBUG);

        if (confirmationMessageCanBeShown) //&& !showingConfirmationMessage)
        {
            ShowingConfirmationMessage = true;
        }

        return Task.FromResult(("", confirmationMessageCanBeShown, interfaceChannel));
    }

    private bool CheckIfConfirmationMessageCanBeShown()
    {
        Log.WriteLine("Starting to check if the confirmation message can be showed.", LogLevel.VERBOSE);

        foreach (var teamKvp in TeamIdsWithReportData)
        {
            FieldInfo[] fieldInfos = typeof(ReportData).GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

            Log.WriteLine("Got field infos, length: " + fieldInfos.Length + " for team: " +
                teamKvp.Value.TeamName, LogLevel.VERBOSE);

            foreach (FieldInfo field in fieldInfos)
            {
                Log.WriteLine("field type: " + field.FieldType, LogLevel.DEBUG);

                // Only process the ReportObject fields (ignore TeamName)
                if (field.FieldType != typeof(ReportObject)) continue;

                Log.WriteLine("This is " + nameof(ReportObject) + " field: " +
                    field.FieldType, LogLevel.VERBOSE);

                ReportObject? reportObject = (ReportObject?)field.GetValue(teamKvp.Value);
                if (reportObject == null)
                {
                    Log.WriteLine(nameof(reportObject) + " was null!", LogLevel.CRITICAL);
                    continue;
                }

                // Skips optional fields
                if (reportObject.CurrentStatus == EmojiName.YELLOWSQUARE) continue;

                if (reportObject.CurrentStatus != EmojiName.WHITECHECKMARK)
                {
                    Log.WriteLine("Team: " + teamKvp.Value.TeamName + "'s " + reportObject.FieldNameDisplay +
                        " was " + reportObject.CurrentStatus + " with value: " + reportObject.ObjectValue, LogLevel.DEBUG);
                    return false;
                }
            }
        }

        Log.WriteLine("All fields were true, proceeding to show the confirmation message", LogLevel.DEBUG);

        return true;
    }

    private string CalculateFinalMatchResult(InterfaceLeague _interfaceLeague)
    {
        Log.WriteLine("Starting to calculate the final match result with teams: " +
            TeamIdsWithReportData.ElementAt(0).Value.TeamName + " and: " +
            TeamIdsWithReportData.ElementAt(1).Value.TeamName, LogLevel.DEBUG);

        return EloSystem.CalculateAndSaveFinalEloDelta(
            FindTeamsInTheMatch(_interfaceLeague), TeamIdsWithReportData);
    }

    public Team[] FindTeamsInTheMatch(InterfaceLeague _interfaceLeague)
    {
        Team[] teamsInTheMatch = new Team[2];
        for (int t = 0; t < TeamIdsWithReportData.Count; t++)
        {
            var foundTeam = _interfaceLeague.LeagueData.FindActiveTeamWithTeamId(TeamIdsWithReportData.ElementAt(t).Key);

            if (foundTeam == null)
            {
                Log.WriteLine("Found team was null!", LogLevel.CRITICAL);
                return Array.Empty<Team>();
            }

            teamsInTheMatch[t] = foundTeam;
        }

        return teamsInTheMatch;
    }

    public (List<ReportData>?, string) GetTeamReportDatasOfTheMatchWithPlayerId(
        InterfaceLeague _interfaceLeague, LeagueMatch _leagueMatch, ulong _playerId)
    {
        List<Team> foundTeams = new List<Team>();
        List<ReportData> reportDatas = new List<ReportData>();

        Log.WriteLine("Getting ReportData on match: " + _leagueMatch.MatchId +
            " with: " + _playerId, LogLevel.DEBUG);

        if (!_leagueMatch.GetIdsOfThePlayersInTheMatchAsArray(
            _interfaceLeague).Contains(_playerId))
        {
            Log.WriteLine("Error, match: " + _leagueMatch.MatchId +
                " does not contain: " + _playerId, LogLevel.CRITICAL);
            return (null, "That's not your match to confirm! by " + _playerId);
        }

        Team? foundTeam =
            _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);

        if (foundTeam == null)
        {
            Log.WriteLine(nameof(foundTeam) + " was null!", LogLevel.CRITICAL);
            return (null, "Could not find: " + nameof(foundTeam));
        }

        foundTeams.Add(foundTeam);

        int otherTeamId = _leagueMatch.TeamsInTheMatch.FirstOrDefault(t => t.Key != foundTeam.TeamId).Key;
        Team? otherTeam = _interfaceLeague.LeagueData.FindActiveTeamWithTeamId(otherTeamId);

        if (otherTeam == null)
        {
            Log.WriteLine(nameof(otherTeam) + " was null!", LogLevel.CRITICAL);
            return (null, "Could not find: " + nameof(otherTeam));
        }

        foundTeams.Add(otherTeam);

        foreach (Team team in foundTeams)
        {
            reportDatas.Add(TeamIdsWithReportData.FirstOrDefault(
                t => t.Key == team.TeamId).Value);
        }

        return (reportDatas, "");
    }
}