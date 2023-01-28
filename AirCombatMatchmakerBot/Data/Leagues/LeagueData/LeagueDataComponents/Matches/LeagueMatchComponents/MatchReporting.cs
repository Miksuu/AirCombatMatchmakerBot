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

    private EloSystem eloSystem { get; set; }

    [DataMember] private Dictionary<int, ReportData> teamIdsWithReportData { get; set; }
    [DataMember] private bool showingConfirmationMessage { get; set; }
    [DataMember] private bool matchDone { get; set; }
    [DataMember] private string? finalResultForConfirmation { get; set; }

    public MatchReporting()
    {
        eloSystem = new EloSystem();
        teamIdsWithReportData = new Dictionary<int, ReportData>();
    }

    public MatchReporting(Dictionary<int, string> _teamsInTheMatch)
    {
        eloSystem = new EloSystem();
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

    public async Task<string> ProcessPlayersSentReportObject(
        InterfaceLeague _interfaceLeague, ulong _playerId, InterfaceMessage _finalMatchResultMessage,
        string _reportedObjectByThePlayer, TypeOfTheReportingObject _typeOfTheReportingObject)
    {
        string response = string.Empty;

        Log.WriteLine("Processing player's sent " + nameof(ReportObject) + " in league: " +
            _interfaceLeague.LeagueCategoryName + " by: " + _playerId + " with data: " +
            _reportedObjectByThePlayer + " of type: " + _typeOfTheReportingObject, LogLevel.DEBUG);

        if (showingConfirmationMessage)
        {
            Log.WriteLine(_playerId + " requested to report the match," +
                " when it was already in confirmation.", LogLevel.VERBOSE);
            return Task.FromResult("Match is in confirmation already! Finish that first, " +
                "or hit the MODIFY button if you need to change reporting result!").Result;
        }

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
                    response = "Unknown type: " + _reportedObjectByThePlayer + "(not implemented ?)";
                    break;
            }
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

        var responseTuple = CheckIfMatchCanBeSentToConfirmation(_finalMatchResultMessage).Result;
        response = responseTuple.Item1;

        await _finalMatchResultMessage.GenerateAndModifyTheMessage();

        await SerializationManager.SerializeDB();

        return Task.FromResult(response).Result;
    }

    private async Task<(string, bool)> CheckIfMatchCanBeSentToConfirmation(InterfaceMessage _interfaceMessage)
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
                return ("Channel doesn't exist!", false);
            }

            await interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                    interfaceChannel, MessageName.MATCHFINALRESULTMESSAGE);

            await interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                interfaceChannel, MessageName.CONFIRMATIONMESSAGE);
        }

        return ("", confirmationMessageCanBeShown);
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

        return eloSystem.CalculateAndChangeFinalEloPoints(
            _interfaceLeague, teamsInTheMatch, teamIdsWithReportData);
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
}