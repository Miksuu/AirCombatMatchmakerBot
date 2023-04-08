using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class MatchReporting
{
    public EloSystem EloSystem
    {
        get
        {
            Log.WriteLine("Getting " + nameof(eloSystem) + " to: " + eloSystem, LogLevel.VERBOSE);
            return eloSystem;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(eloSystem)
                + " to: " + value, LogLevel.VERBOSE);
            eloSystem = value;
        }
    }

    public ConcurrentDictionary<int, ReportData> TeamIdsWithReportData
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamIdsWithReportData) + " with count of: " +
                teamIdsWithReportData.Count, LogLevel.VERBOSE);
            return teamIdsWithReportData;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamIdsWithReportData)
                + " to: " + value, LogLevel.VERBOSE);
            teamIdsWithReportData = value;
        }
    }

    public bool ShowingConfirmationMessage
    {
        get
        {
            Log.WriteLine("Getting " + nameof(showingConfirmationMessage), LogLevel.VERBOSE);
            return showingConfirmationMessage;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(showingConfirmationMessage)
                + " to: " + value, LogLevel.VERBOSE);
            showingConfirmationMessage = value;
        }
    }

    public bool MatchDone
    {
        get
        {
            Log.WriteLine("Getting " + nameof(matchDone), LogLevel.VERBOSE);
            return matchDone;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(matchDone)
                + " to: " + value, LogLevel.VERBOSE);
            matchDone = value;
        }
    }

    public string? FinalResultForConfirmation
    {
        get
        {
            Log.WriteLine("Getting finalResultForConfirmation", LogLevel.VERBOSE);
            return finalResultForConfirmation;
        }
        set
        {
            Log.WriteLine("Setting finalResultForConfirmation"
                + " to: " + value, LogLevel.VERBOSE);
            finalResultForConfirmation = value;
        }
    }

    public string? FinalMessageForMatchReportingChannel
    {
        get
        {
            Log.WriteLine("Getting finalMessageForMatchReportingChannel", LogLevel.VERBOSE);
            return finalMessageForMatchReportingChannel;
        }
        set
        {
            Log.WriteLine("Setting finalMessageForMatchReportingChannel"
                + " to: " + value, LogLevel.VERBOSE);
            finalMessageForMatchReportingChannel = value;
        }
    }

    public string? FinalResultTitleForConfirmation
    {
        get
        {
            Log.WriteLine("Getting finalResultTitleForConfirmation", LogLevel.VERBOSE);
            return finalResultTitleForConfirmation;
        }
        set
        {
            Log.WriteLine("Setting finalResultTitleForConfirmation"
                + " to: " + value, LogLevel.VERBOSE);
            finalResultTitleForConfirmation = value;
        }
    }

    private EloSystem eloSystem { get; set; }
    [DataMember] private ConcurrentDictionary<int, ReportData> teamIdsWithReportData { get; set; }
    [DataMember] private bool showingConfirmationMessage { get; set; }
    [DataMember] private bool matchDone { get; set; }
    [DataMember] private string? finalResultForConfirmation { get; set; }
    [DataMember] private string? finalMessageForMatchReportingChannel { get; set; }
    [DataMember] private string? finalResultTitleForConfirmation { get; set; }

    public MatchReporting()
    {
        eloSystem = new EloSystem();
        teamIdsWithReportData = new ConcurrentDictionary<int, ReportData>();
    }

    public MatchReporting(ConcurrentDictionary<int, string> _teamsInTheMatch)
    {
        eloSystem = new EloSystem();
        teamIdsWithReportData = new ConcurrentDictionary<int, ReportData>();

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

    public async Task<(string, bool)> ProcessPlayersSentReportObject(
        InterfaceLeague _interfaceLeague, ulong _playerId, string _reportedObjectByThePlayer,
        TypeOfTheReportingObject _typeOfTheReportingObject, ulong _leagueCategoryId, ulong _messageChannelId)
    {
        string response = string.Empty;

        Log.WriteLine("Processing player's sent " + nameof(ReportObject) + " in league: " +
            _interfaceLeague.LeagueCategoryName + " by: " + _playerId + " with data: " +
            _reportedObjectByThePlayer + " of type: " + _typeOfTheReportingObject, LogLevel.DEBUG);

        if (matchDone)
        {
            Log.WriteLine(_playerId + " requested to report the match," +
                " when it was already over.", LogLevel.VERBOSE);
            return Task.FromResult(("Match is already done!", false)).Result;
        }

        // Can receive comments still even though the the confirmation is under way
        if (showingConfirmationMessage && _typeOfTheReportingObject != TypeOfTheReportingObject.COMMENTBYTHEUSER)
        {
            Log.WriteLine(_playerId + " requested to report the match," +
                " when it was already in confirmation.", LogLevel.VERBOSE);
            return Task.FromResult(("Match is in confirmation already! Finish that first, " +
                "or hit the MODIFY button if you need to change reporting result!", false)).Result;
        }

        Team? reportingTeam =
            _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(
                _playerId);
        if (reportingTeam == null)
        {
            Log.WriteLine(nameof(reportingTeam) + " was null! with playerId: " + _playerId, LogLevel.CRITICAL);
            return Task.FromResult((response, false)).Result;
        }

        // First time pressing the report button for the team
        if (!TeamIdsWithReportData.ContainsKey(reportingTeam.TeamId))
        {
            Log.WriteLine("Key wasn't found! by player:" + _playerId, LogLevel.WARNING);
            return ("", false);
        }
        // Replacing the result
        else
        {
            Log.WriteLine("Key was, the team is not their first time reporting.", LogLevel.VERBOSE);

            switch (_typeOfTheReportingObject)
            {
                case TypeOfTheReportingObject.REPORTEDSCORE:
                    TeamIdsWithReportData[reportingTeam.TeamId].ReportedScore.SetObjectValueAndFieldBool(
                        _reportedObjectByThePlayer, true);
                    response = "You reported score of: " + _reportedObjectByThePlayer;
                    break;
                case TypeOfTheReportingObject.TACVIEWLINK:
                    TeamIdsWithReportData[reportingTeam.TeamId].TacviewLink.SetObjectValueAndFieldBool(
                        _reportedObjectByThePlayer, true);
                    response = "You posted tacview link: " + _reportedObjectByThePlayer;
                    break;
                case TypeOfTheReportingObject.COMMENTBYTHEUSER:
                    TeamIdsWithReportData[reportingTeam.TeamId].CommentByTheUser.SetObjectValueAndFieldBool(
                        _reportedObjectByThePlayer, true);
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
        // edit that messageDescription instead of the reporting status messageDescription which would be null
        MessageName messageNameToEdit = MessageName.REPORTINGSTATUSMESSAGE;
        if (showingConfirmationMessage)
        {
            messageNameToEdit = MessageName.MATCHFINALRESULTMESSAGE;

            var interfaceMessage = interfaceChannel.FindInterfaceMessageWithNameInTheChannel(
                MessageName.MATCHFINALRESULTMESSAGE);
            if (interfaceMessage == null)
            {
                Log.WriteLine(nameof(interfaceMessage) + " was null!", LogLevel.CRITICAL);
                return (nameof(interfaceMessage) + " was null!", false);
            }
            finalResultForConfirmation = interfaceMessage.GenerateMessage();
            // Must be called after GenerateMessage() since it's defined there
            finalResultTitleForConfirmation = interfaceMessage.MessageEmbedTitle;
        }

        InterfaceMessage? messageToEdit = interfaceChannel.FindInterfaceMessageWithNameInTheChannel(
                        messageNameToEdit);
        if (messageToEdit == null)
        {
            string errorMsg = nameof(messageToEdit) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
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
            return Task.FromResult((response, false)).Result;
        }

        return Task.FromResult((response, true)).Result;
    }

    public async Task<(string, bool)> PrepareFinalMatchResult(
        InterfaceLeague _interfaceLeague, ulong _playerId,
         ulong _leagueCategoryId, ulong _messageChannelId)
    {
        string response = string.Empty;

        var responseTuple = CheckIfMatchCanBeSentToConfirmation(_leagueCategoryId, _messageChannelId).Result;
        if (responseTuple.Item3 == null)
        {
            Log.WriteLine(nameof(responseTuple.Item3) + " was null! with playerId: " + _playerId, LogLevel.CRITICAL);
            return (response, false);
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
                return (errorMsg, false);
            }

            MATCHFINALRESULTMESSAGE? finalResultMessage = interfaceMessage as MATCHFINALRESULTMESSAGE;
            finalResultForConfirmation = interfaceMessage.MessageDescription;
            finalMessageForMatchReportingChannel = finalResultMessage.AlternativeMessage;

            finalResultTitleForConfirmation = interfaceMessage.MessageEmbedTitle;

            await interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                MessageName.CONFIRMATIONMESSAGE);

            // Copypasted to MODIFYMATCHBUTTON.CS, maybe replace to method
            InterfaceChannel interfaceChannelToDeleteTheMessageIn = _interfaceLeague.FindLeaguesInterfaceCategory(
                ).FindInterfaceChannelWithIdInTheCategory(
                    _messageChannelId);
            if (interfaceChannelToDeleteTheMessageIn == null)
            {
                Log.WriteLine(nameof(interfaceChannelToDeleteTheMessageIn) + " was null, with: " +
                    _messageChannelId, LogLevel.CRITICAL);
                return (nameof(interfaceChannelToDeleteTheMessageIn) + " was null", false);
            }

            await interfaceChannelToDeleteTheMessageIn.DeleteMessagesInAChannelWithMessageName(
                MessageName.REPORTINGSTATUSMESSAGE);
        }

        Log.WriteLine("returning response: " + response, LogLevel.VERBOSE);

        return (response, true);
    }

    private Task<(string, bool, InterfaceChannel?)> CheckIfMatchCanBeSentToConfirmation(
        ulong _leagueCategoryId, ulong _messageChannelId)
    {
        bool confirmationMessageCanBeShown = CheckIfConfirmationMessageCanBeShown();

        InterfaceChannel? interfaceChannel = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            _leagueCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(_messageChannelId);
        if (interfaceChannel == null)
        {
            Log.WriteLine("channel was null!", LogLevel.CRITICAL);
            return Task.FromResult(("Channel doesn't exist!", false, interfaceChannel));
        }

        Log.WriteLine("Message can be shown: " + confirmationMessageCanBeShown +
            " showing: " + showingConfirmationMessage, LogLevel.DEBUG);

        if (confirmationMessageCanBeShown) //&& !showingConfirmationMessage)
        {
            showingConfirmationMessage = true;
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

                // Only process the ReportObject fields (ignore teamName)
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
                if (reportObject.DefaultStateEmoji == EmojiName.YELLOWSQUARE) continue;

                if (!reportObject.FieldFilled)
                {
                    Log.WriteLine("Team: " + teamKvp.Value.TeamName + "'s " + reportObject.FieldNameDisplay +
                        " was " + reportObject.FieldFilled + " with value: " + reportObject.ObjectValue, LogLevel.DEBUG);
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

        return eloSystem.CalculateAndSaveFinalEloDelta(
            FindTeamsInTheMatch(_interfaceLeague), teamIdsWithReportData);
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
                return new Team[0];
            }

            teamsInTheMatch[t] = foundTeam;
        }

        return teamsInTheMatch;
    }

    public (ConcurrentBag<ReportData>?, string) GetTeamReportDatasOfTheMatchWithPlayerId(
        InterfaceLeague _interfaceLeague, LeagueMatch _leagueMatch, ulong _playerId)
    {
        ConcurrentBag<Team> foundTeams = new ConcurrentBag<Team>();
        ConcurrentBag<ReportData> reportDatas = new ConcurrentBag<ReportData>();

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
            reportDatas.Add(teamIdsWithReportData.FirstOrDefault(
                t => t.Key == team.TeamId).Value);
        }

        return (reportDatas, "");
    }
}