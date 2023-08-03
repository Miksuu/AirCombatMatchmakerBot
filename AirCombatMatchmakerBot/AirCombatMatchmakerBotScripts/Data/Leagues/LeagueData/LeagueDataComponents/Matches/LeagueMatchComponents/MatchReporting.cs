using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class MatchReporting
{
    [IgnoreDataMember]
    public ConcurrentDictionary<int, ReportData> TeamIdsWithReportData
    {
        get => teamIdsWithReportData.GetValue();
        set => teamIdsWithReportData.SetValue(value);
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

    [DataMember] private logConcurrentDictionary<int, ReportData> teamIdsWithReportData =
        new logConcurrentDictionary<int, ReportData>();
    [DataMember] private logString finalResultForConfirmation = new logString();
    [DataMember] private logString finalMessageForMatchReportingChannel = new logString();
    [DataMember] private logString finalResultTitleForConfirmation = new logString();

    public InterfaceLeague interfaceLeagueRef;

    public MatchReporting() { }

    public MatchReporting(
        ConcurrentDictionary<int, string> _teamsInTheMatch, InterfaceLeague _interfaceLeague)
    {
        interfaceLeagueRef = _interfaceLeague;

        foreach (var teamKvp in _teamsInTheMatch)
        {
            if (TeamIdsWithReportData.ContainsKey(teamKvp.Key))
            {
                Log.WriteLine("Already contains the key: " + teamKvp.Key, LogLevel.DEBUG);
                continue;
            }

            Log.WriteLine("Adding team: " + teamKvp.Key + " | " + teamKvp.Value);

            try
            {
                Log.WriteLine("before toAdd", LogLevel.DEBUG);

                var toAdd = (teamKvp.Key, new ReportData(teamKvp.Value,
                    interfaceLeagueRef.LeagueData.Teams.FindTeamById(teamKvp.Key).Players));

                Log.WriteLine("after toAdd", LogLevel.DEBUG);

                TeamIdsWithReportData.TryAdd(toAdd.Key, toAdd.Item2);

                Log.WriteLine("after adding with toAdd", LogLevel.DEBUG);
            }

            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.ERROR);
                continue;
            }
        }
    }

    public BaseReportingObject GetInterfaceReportingObjectWithTypeOfTheReportingObject(
        TypeOfTheReportingObject _typeOfTheReportingObject, int _reportingTeamTeamId)
    {
        var reportingObject = TeamIdsWithReportData[_reportingTeamTeamId].ReportingObjects.FirstOrDefault(x
            => x.GetTypeOfTheReportingObject() == _typeOfTheReportingObject);
        if (reportingObject == null)
        {
            Log.WriteLine(nameof(reportingObject) + " was null!", LogLevel.ERROR);
            throw new InvalidOperationException(nameof(reportingObject) + " was null!");
        }

        return reportingObject;
    }

    public async Task<Response> ProcessPlayersSentReportObject(
        ulong _playerId, string _reportedObjectByThePlayer, TypeOfTheReportingObject _typeOfTheReportingObject,
        ulong _channelCategoryId, ulong _messageChannelId)
    {
        try
        {
            string response = string.Empty;

            MatchState matchState = Database.Instance.Leagues.GetILeagueByCategoryId(
                _channelCategoryId).LeagueData.Matches.FindLeagueMatchByTheChannelId(_messageChannelId).MatchState;

            Log.WriteLine("Processing player's sent " + nameof(BaseReportingObject) + " in league: " +
                interfaceLeagueRef.LeagueCategoryName + " by: " + _playerId + " with data: " +
                _reportedObjectByThePlayer + " of type: " + _typeOfTheReportingObject, LogLevel.DEBUG);

            Team reportingTeam = interfaceLeagueRef.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(
                _playerId);

            // First time pressing the report button for the team
            if (!TeamIdsWithReportData.ContainsKey(reportingTeam.TeamId))
            {
                Log.WriteLine("Key wasn't found! by player:" + _playerId, LogLevel.WARNING);
                return new Response("", false);
            }
            // Replacing the result
            else
            {
                Log.WriteLine("Key was, the team is not their first time reporting.");

                var interfaceReportingObject =
                    GetInterfaceReportingObjectWithTypeOfTheReportingObject(_typeOfTheReportingObject, reportingTeam.TeamId).thisReportingObject;

                if (!interfaceReportingObject.AllowedMatchStatesToProcessOn.Contains(matchState))
                {
                    return new Response("That's not allowed at this stage of the reporting!", false);
                }

                if (_reportedObjectByThePlayer == "-")
                {
                    interfaceReportingObject.CancelTheReportingObjectAction();
                }
                else
                {
                    interfaceReportingObject.ProcessTheReportingObjectAction(
                        _reportedObjectByThePlayer, TeamIdsWithReportData, reportingTeam.TeamId);
                }
            }
            InterfaceChannel interfaceChannel =
                DiscordBotDatabase.Instance.Categories.FindInterfaceCategoryWithCategoryId(
                    _channelCategoryId).FindInterfaceChannelWithIdInTheCategory(
                        _messageChannelId);

            // If the match is on the confirmation phase,
            // edit that MessageDescription instead of the reporting status MessageDescription which would be null
            if (matchState == MatchState.CONFIRMATIONPHASE)
            {
                var interfaceMessage = interfaceChannel.FindInterfaceMessageWithNameInTheChannel(
                    MessageName.MATCHFINALRESULTMESSAGE);

                FinalResultForConfirmation = interfaceMessage.GenerateMessage().Result;
                // Must be called after GenerateMessage() since it's defined there
                FinalResultTitleForConfirmation = interfaceMessage.MessageEmbedTitle;
                interfaceMessage.GenerateAndModifyTheMessage();
            }
            else if (matchState == MatchState.REPORTINGPHASE)
            {
                InterfaceMessage messageToEdit = interfaceChannel.FindInterfaceMessageWithNameInTheChannel(
                MessageName.REPORTINGSTATUSMESSAGE);
                messageToEdit.GenerateAndModifyTheMessage();
            }

            foreach (var reportedTeamKvp in TeamIdsWithReportData)
            {
                Log.WriteLine("Reported team: " + reportedTeamKvp.Key +
                    " with value: " + reportedTeamKvp.Value);
            }

            int reportedTeamsCount = TeamIdsWithReportData.Count;

            Log.WriteLine("Reported teams count: " + reportedTeamsCount);

            if (reportedTeamsCount > 2)
            {
                Log.WriteLine("Count was: " + reportedTeamsCount + ", Error!", LogLevel.ERROR);

                // Maybe handle the error
                return Task.FromResult(new Response("Couldn't post comment", false)).Result;
            }

            return Task.FromResult(new Response(response, true)).Result;
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return new Response(ex.Message, false);
        }
    }

    public async Task<Response> PrepareFinalMatchResult(ulong _playerId, ulong _messageChannelId)
    {
        try
        {
            string response = string.Empty;

            (string, bool, InterfaceChannel?) responseTuple =
                CheckIfMatchCanBeSentToConfirmation(interfaceLeagueRef.LeagueCategoryId, _messageChannelId);
            if (responseTuple.Item3 == null)
            {
                Log.WriteLine(nameof(responseTuple.Item3) + " was null! with playerId: " + _playerId, LogLevel.ERROR);
                return new Response(response, false);
            }

            response = responseTuple.Item1;
            InterfaceChannel interfaceChannel = responseTuple.Item3;

            if (responseTuple.Item2)
            {
                CalculateFinalMatchResult();

                Log.WriteLine("Creating new messages from: " + _playerId, LogLevel.DEBUG);

                InterfaceMessage interfaceMessage = interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                    MessageName.MATCHFINALRESULTMESSAGE).Result;

                MATCHFINALRESULTMESSAGE? finalResultMessage = interfaceMessage as MATCHFINALRESULTMESSAGE;
                if (finalResultMessage == null)
                {
                    string errorMsg = nameof(finalResultMessage) + " was null!";
                    Log.WriteLine(errorMsg, LogLevel.ERROR);
                    return new Response(errorMsg, false);
                }

                FinalResultForConfirmation = interfaceMessage.MessageDescription;
                FinalMessageForMatchReportingChannel = finalResultMessage.AlternativeMessage;
                FinalResultTitleForConfirmation = interfaceMessage.MessageEmbedTitle;

                InterfaceCategory interfaceCategory;
                InterfaceChannel interfaceChannelToDeleteTheMessageIn;

                await interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                    MessageName.CONFIRMATIONMESSAGE);
                interfaceCategory = interfaceLeagueRef.FindLeaguesInterfaceCategory();

                // Copypasted to MODIFYMATCHBUTTON.CS, maybe replace to method
                interfaceChannelToDeleteTheMessageIn =
                    interfaceCategory.FindInterfaceChannelWithIdInTheCategory(
                        _messageChannelId);

                // Delete the messages which aren't useful for confirmation phase anymore
                await interfaceChannelToDeleteTheMessageIn.DeleteMessagesInAChannelWithMessageName(
                    MessageName.REPORTINGMESSAGE);
                await interfaceChannelToDeleteTheMessageIn.DeleteMessagesInAChannelWithMessageName(
                    MessageName.REPORTINGSTATUSMESSAGE);
            }

            Log.WriteLine("returning response: " + response);

            return new Response(response?.ToString() ?? "[null]", true);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return new Response(ex.Message, false);
        }
    }

    private (string, bool, InterfaceChannel?) CheckIfMatchCanBeSentToConfirmation(
        ulong _channelCategoryId, ulong _messageChannelId)
    {
        try
        {
            InterfaceChannel interfaceChannel = DiscordBotDatabase.Instance.Categories.FindInterfaceCategoryWithCategoryId(
                _channelCategoryId).FindInterfaceChannelWithIdInTheCategory(_messageChannelId);

            bool confirmationMessageCanBeShown = CheckIfConfirmationMessageCanBeShown(interfaceChannel);

            Log.WriteLine("Message can be shown: " + confirmationMessageCanBeShown, LogLevel.DEBUG);

            if (confirmationMessageCanBeShown)
            {
                Database.Instance.Leagues.GetILeagueByCategoryId(
               _channelCategoryId).LeagueData.Matches.FindLeagueMatchByTheChannelId(_messageChannelId).MatchState = MatchState.CONFIRMATIONPHASE;
            }

            return ("", confirmationMessageCanBeShown, interfaceChannel);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return new(ex.Message, false, null);
        }
    }

    private bool CheckIfConfirmationMessageCanBeShown(InterfaceChannel _interfaceChannel)
    {
        Log.WriteLine("Starting to check if the confirmation message can be showed.");

        foreach (var teamKvp in TeamIdsWithReportData)
        {
            FieldInfo[] fieldInfos = typeof(ReportData).GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

            Log.WriteLine("Got field infos, length: " + fieldInfos.Length + " for team: " +
                teamKvp.Value.TeamName);

            foreach (var item in teamKvp.Value.ReportingObjects)
            {
                var interfaceItem = (InterfaceReportingObject)item;

                // Skips optional fields
                if (interfaceItem.CurrentStatus == EmojiName.YELLOWSQUARE) continue;

                if (interfaceItem.CurrentStatus != EmojiName.WHITECHECKMARK)
                {
                    Log.WriteLine("Team: " + teamKvp.Value.TeamName + "'s " + interfaceItem.TypeOfTheReportingObject +
                        " was " + interfaceItem.CurrentStatus + " with value: " + interfaceItem.ObjectValue, LogLevel.DEBUG);
                    return false;
                }
            }
        }

        int[] reportedScores = new int[TeamIdsWithReportData.Count];
        bool[] scoreFound = new bool[TeamIdsWithReportData.Count];
        for (int i = 0; i < TeamIdsWithReportData.Count; i++)
        {
            bool found = int.TryParse(
                TeamIdsWithReportData.ElementAt(i).Value.FindBaseReportingObjectOfType(
                    TypeOfTheReportingObject.REPORTEDSCORE).thisReportingObject.ObjectValue, out int _foundScore);
            if (found)
            {
                scoreFound[i] = true;
                reportedScores[i] = _foundScore;
            }
        }
        if (reportedScores[0] == reportedScores[1] && scoreFound[0] && scoreFound[1])
        {
            var reportingStatusMessage = _interfaceChannel.FindInterfaceMessageWithNameInTheChannel(
                MessageName.REPORTINGSTATUSMESSAGE);

            reportingStatusMessage.AddContentToTheEndOfTheMessage("The match can not be a draw!");
            return false;
        }

        Log.WriteLine("All fields were true and it's not a draw, proceeding to show the confirmation message", LogLevel.DEBUG);

        return true;
    }

    private string CalculateFinalMatchResult()
    {
        Log.WriteLine("Starting to calculate the final match result with teams: " +
            TeamIdsWithReportData.ElementAt(0).Value.TeamName + " and: " +
            TeamIdsWithReportData.ElementAt(1).Value.TeamName, LogLevel.DEBUG);

        return EloSystem.CalculateAndSaveFinalEloDelta(
            FindTeamsInTheMatch(), TeamIdsWithReportData.ToDictionary(x => x.Key, x => x.Value));
    }

    public Team[] FindTeamsInTheMatch()
    {
        Team[] teamsInTheMatch = new Team[2];
        for (int t = 0; t < TeamIdsWithReportData.Count; t++)
        {
            try
            {
                var foundTeam = interfaceLeagueRef.LeagueData.FindActiveTeamWithTeamId(TeamIdsWithReportData.ElementAt(t).Key);
                teamsInTheMatch[t] = foundTeam;
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.ERROR);
                continue;
            }
        }

        return teamsInTheMatch;
    }

    public List<ReportData> GetTeamReportDatasOfTheMatchWithPlayerId(LeagueMatch _leagueMatch, ulong _playerId)
    {
        Team foundTeam;
        List<Team> foundTeams = new List<Team>();
        List<ReportData> reportDatas = new List<ReportData>();

        Log.WriteLine("Getting ReportData on match: " + _leagueMatch.MatchId +
            " with: " + _playerId, LogLevel.DEBUG);

        if (!_leagueMatch.GetIdsOfThePlayersInTheMatchAsArray().Contains(_playerId))
        {
            Log.WriteLine("Error, match: " + _leagueMatch.MatchId +
                " does not contain: " + _playerId, LogLevel.ERROR);
            throw new InvalidOperationException("That's not your match to confirm! by " + _playerId);
        }

        try
        {
            foundTeam = interfaceLeagueRef.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);
            foundTeams.Add(foundTeam);

            int otherTeamId = _leagueMatch.TeamsInTheMatch.FirstOrDefault(t => t.Key != foundTeam.TeamId).Key;
            Team otherTeam = interfaceLeagueRef.LeagueData.FindActiveTeamWithTeamId(otherTeamId);

            foundTeams.Add(otherTeam);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            throw new InvalidOperationException(ex.Message);
        }

        foreach (Team team in foundTeams)
        {
            reportDatas.Add(TeamIdsWithReportData.FirstOrDefault(
                t => t.Key == team.TeamId).Value);
        }

        return reportDatas;
    }
}