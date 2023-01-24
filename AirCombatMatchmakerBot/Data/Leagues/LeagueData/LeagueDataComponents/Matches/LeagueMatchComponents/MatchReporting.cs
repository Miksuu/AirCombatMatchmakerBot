using Discord;
using Discord.WebSocket;
using System;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Channels;

[DataContract]
public class MatchReporting
{
    public Dictionary<int, ReportData> TeamIdsWithReportData
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


    [DataMember] private Dictionary<int, ReportData> teamIdsWithReportData { get; set; }
    [DataMember] private bool showingConfirmationMessage { get; set; }
    [DataMember] private bool matchDone { get; set; }

    public MatchReporting()
    {
        teamIdsWithReportData = new Dictionary<int, ReportData>();
    }

    public MatchReporting(Dictionary<int, string> _teamsInTheMatch)
    {
        teamIdsWithReportData = new Dictionary<int, ReportData>();

        foreach (var teamKvp in _teamsInTheMatch)
        {
            if (TeamIdsWithReportData.ContainsKey(teamKvp.Key))
            {
                Log.WriteLine("Already contains the key: " + teamKvp.Key, LogLevel.DEBUG);
                continue;
            }

            Log.WriteLine("Adding team: " + teamKvp.Key + " | " + teamKvp.Value, LogLevel.VERBOSE);

            TeamIdsWithReportData.Add(teamKvp.Key, new ReportData(teamKvp.Value));
        }
    }

    public async Task<string> ReportMatchResult(
        InterfaceLeague _interfaceLeague, ulong _playerId,
        int _playerReportedResult, InterfaceMessage _interfaceMessage)
    {
        string response = string.Empty;

        if (matchDone)
        {
            Log.WriteLine(_playerId + " requested to report the match," +
                " when it was already over.", LogLevel.VERBOSE);
            return Task.FromResult("Match is already done!").Result;
        }

        Team? reportingTeam =
            _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(
                _playerId);
        if (reportingTeam == null)
        {
            Log.WriteLine(nameof(reportingTeam) + " was null! with playerId: " + _playerId, LogLevel.CRITICAL);
            return Task.FromResult(response).Result;
        }

        // First time pressing the report button for the team
        if (!TeamIdsWithReportData.ContainsKey(reportingTeam.TeamId))
        {
            Log.WriteLine("Key wasn't found! by player:" + _playerId, LogLevel.WARNING);

            return "";
        }
        // Replacing the result
        else
        {
            Log.WriteLine("Key was, the team is not their first time reporting.", LogLevel.VERBOSE);
            TeamIdsWithReportData[reportingTeam.TeamId].ReportedScore.SetObjectValueAndFieldBool(
                _playerReportedResult.ToString(), true);
            response = "You reported score of: " + _playerReportedResult;
        }

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
            return Task.FromResult(response).Result;
        }

        response = CheckIfMatchCanBeSentToConfirmation(_interfaceMessage).Result;

        /*
        // Fix this
        if (false)
        {
            //MatchDone = true;
            response = CalculateFinalMatchResult(_interfaceLeague, reportingTeam);
        }*/

        await _interfaceMessage.ModifyMessage(_interfaceMessage.GenerateMessage());

        await SerializationManager.SerializeDB();

        return Task.FromResult(response).Result;
    }

    private async Task<string> CheckIfMatchCanBeSentToConfirmation(InterfaceMessage _interfaceMessage)
    {
        bool confirmationMessageCanBeShown = CheckIfConfirmationMessageCanBeShown();

        Log.WriteLine("Message can be shown: " + confirmationMessageCanBeShown +
            " showing: " + showingConfirmationMessage, LogLevel.DEBUG);

        if (confirmationMessageCanBeShown && !showingConfirmationMessage)
        {
            showingConfirmationMessage = true;

            var interfaceChannel = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                    _interfaceMessage.MessageChannelId);
            if (interfaceChannel == null)
            {
                Log.WriteLine("channel was null!", LogLevel.CRITICAL);
                return "Channel doesn't exist!";
            }

            await interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                interfaceChannel, MessageName.CONFIRMATIONMESSAGE);
        }

        return "";
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

                ReportObject reportObject = (ReportObject)field.GetValue(teamKvp.Value);
                if (reportObject == null)
                {
                    Log.WriteLine(nameof(reportObject) + " was null!", LogLevel.CRITICAL);
                    continue;
                }

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

    private string CalculateFinalMatchResult(InterfaceLeague _interfaceLeague, Team _reportingTeam)
    {
        Log.WriteLine("Starting to calculate the final match result with teams: " +
            TeamIdsWithReportData.ElementAt(0) + " and: " +
            TeamIdsWithReportData.ElementAt(1), LogLevel.DEBUG);   

        Team[] teamsInTheMatch = new Team[2];
        teamsInTheMatch[0] = _reportingTeam;

        Team? otherTeam = FindTheOtherTeamThatIsActive(_interfaceLeague, _reportingTeam.TeamId);
        if (otherTeam == null)
        {
            Log.WriteLine(nameof(otherTeam) + " was null!", LogLevel.CRITICAL);
            return "";
        }
        teamsInTheMatch[1] = otherTeam;

        return CalculateAndChangeFinalEloPoints(_interfaceLeague, teamsInTheMatch);
    }

    private Team? FindTheOtherTeamThatIsActive(InterfaceLeague _interfaceLeague, int _excludedTeamId)
    {
        Log.WriteLine("Finding the other team excluding: " + _excludedTeamId, LogLevel.VERBOSE);

        if (_interfaceLeague == null)
        {
            Log.WriteLine(nameof(_interfaceLeague) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        int otherTeamId = TeamIdsWithReportData.FirstOrDefault(t => t.Key != _excludedTeamId).Key;
        Log.WriteLine("Found other team id: " + otherTeamId, LogLevel.VERBOSE);
        return _interfaceLeague.LeagueData.FindTeamWithTeamId(otherTeamId);
    }

    private int DecideWinnerIndex()
    {
        int winnerIndex = 0;

        string teamOneObjectValue = TeamIdsWithReportData.ElementAt(0).Value.ReportedScore.ObjectValue;
        string teamTwoObjectValue = TeamIdsWithReportData.ElementAt(1).Value.ReportedScore.ObjectValue;

        int teamOneOutput = 0;
        if (int.TryParse(teamOneObjectValue, out int output))
        {
            teamOneOutput = output;
        }
        else
        {
            Log.WriteLine("Parse failed for value (team one): " + teamOneObjectValue, LogLevel.CRITICAL);
            return 3;
        }

        int teamTwoOutput = 0;
        if (int.TryParse(teamOneObjectValue, out int outputTwo))
        {
            teamTwoOutput = outputTwo;
        }
        else
        {
            Log.WriteLine("Parse failed for value (team two): " + teamTwoObjectValue, LogLevel.CRITICAL);
            return 3;
        }

        if (teamTwoOutput > teamOneOutput)
        {
            winnerIndex = 1;
        }
        else if (teamTwoOutput == teamOneOutput)
        {
            winnerIndex = 2;
        }
            
        Log.WriteLine("winnerIndex is = " + winnerIndex, LogLevel.VERBOSE);

        return winnerIndex;
    }

    private string CalculateAndChangeFinalEloPoints(
        InterfaceLeague _interfaceLeague, Team[] _teamsInTheMatch)
    {
        float firstTeamSkillRating = _teamsInTheMatch[0].SkillRating;
        float secondTeamSkillRating = _teamsInTheMatch[1].SkillRating;

        Log.WriteLine("Calculating final elo points for: " + firstTeamSkillRating +
            " | " + secondTeamSkillRating, LogLevel.VERBOSE);

        int winnerIndex = DecideWinnerIndex();

        if (winnerIndex == 2)
        {
            return "The match cannot be a draw!";
        }

        int eloDelta = (int)(32 * (1 - winnerIndex - ExpectationToWin(
            _teamsInTheMatch[0].SkillRating, _teamsInTheMatch[1].SkillRating)));

        Log.WriteLine("EloDelta: " + eloDelta, LogLevel.VERBOSE);

        if (_teamsInTheMatch[0] == null)
        {
            Log.WriteLine(nameof(_teamsInTheMatch) + " was null!", LogLevel.CRITICAL);
            return "";
        }

        Team? databaseTeamOne = _interfaceLeague.LeagueData.FindTeamWithTeamId(_teamsInTheMatch[0].TeamId);

        if (databaseTeamOne == null)
        {
            Log.WriteLine(nameof(databaseTeamOne) + " was null!", LogLevel.CRITICAL);
            return "";
        }

        Team? databaseTeamTwo = _interfaceLeague.LeagueData.FindTeamWithTeamId(_teamsInTheMatch[1].TeamId);

        if (databaseTeamTwo == null)
        {
            Log.WriteLine(nameof(databaseTeamTwo) + " was null!", LogLevel.CRITICAL);
            return "";
        }

        // Make the change in the player's ratings
        databaseTeamOne.SkillRating += eloDelta;
        databaseTeamTwo.SkillRating -= eloDelta;

        Log.WriteLine("Done calculating and changing elo points for: " + databaseTeamOne.SkillRating +
            " | " + databaseTeamTwo.SkillRating, LogLevel.DEBUG);

        return "";
    }

    private double ExpectationToWin(float _playerOneRating, float _playerTwoRating)
    {
        return 1 / (1 + Math.Pow(10, (_playerTwoRating - _playerOneRating) / 400.0));
    }

    public async Task ProcessTacviewSentByTheUser(
        InterfaceLeague _interfaceLeague, SocketMessage _socketMessage, string _attachmentUrl)
    {
        Log.WriteLine("Starting to process tacview send request by: " + _socketMessage.Author.Id +
            " in league: " + _interfaceLeague.LeagueCategoryName + " with url: " +
            _attachmentUrl, LogLevel.VERBOSE);

        Team? reportingTeam =
            _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(
                _socketMessage.Author.Id);
        if (reportingTeam == null)
        {
            Log.WriteLine(nameof(reportingTeam) + " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Reporting team id: " + reportingTeam.TeamId, LogLevel.VERBOSE);

        if (!TeamIdsWithReportData.ContainsKey(reportingTeam.TeamId))
        {
            Log.WriteLine("Did not contain key: " + reportingTeam.TeamId + " admin?: " +
                _socketMessage.Author.Id + " trying to post a tacview to a match channel" +
                " that he's not part of?", LogLevel.WARNING);
            return;
        }

        TeamIdsWithReportData[reportingTeam.TeamId].TacviewLink.SetObjectValueAndFieldBool(
            _attachmentUrl, true);

        InterfaceMessage reportingStatusMessage =
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _interfaceLeague.DiscordLeagueReferences.LeagueCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                    _socketMessage.Channel.Id).FindInterfaceMessageWithNameInTheChannel(
                        MessageName.REPORTINGSTATUSMESSAGE);

        if (reportingStatusMessage == null)
        {
            Log.WriteLine(nameof(reportingStatusMessage) + " was null!", LogLevel.CRITICAL);
            return;
        }

        await CheckIfMatchCanBeSentToConfirmation(reportingStatusMessage);

        await reportingStatusMessage.ModifyMessage(reportingStatusMessage.GenerateMessage());

        await SerializationManager.SerializeDB();
    }
}