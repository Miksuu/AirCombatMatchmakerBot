﻿using System.Runtime.Serialization;
using System.Collections.Concurrent;

public static class EloSystem 
{ 
    public static string CalculateAndSaveFinalEloDelta(
        Team[] _teamsInTheMatch, Dictionary<int, ReportData> _teamIdsWithReportData)
    {
        float firstTeamSkillRating = _teamsInTheMatch[0].SkillRating;
        float secondTeamSkillRating = _teamsInTheMatch[1].SkillRating;

        Log.WriteLine("Calculating final elo points for: " + firstTeamSkillRating +
            " | " + secondTeamSkillRating, LogLevel.DEBUG);

        int winnerIndex = DecideWinnerIndex(_teamIdsWithReportData);

        if (winnerIndex == 2)
        {
            // Handle this
            return "The match cannot be a draw!";
        }

        Log.WriteLine("Before calculating elo delta", LogLevel.DEBUG);

        float eloDelta = (int)(32 * (1 - winnerIndex - ExpectationToWin(
            firstTeamSkillRating, secondTeamSkillRating)));

        Log.WriteLine("calculated EloDelta: " + eloDelta, LogLevel.DEBUG);


        if (_teamsInTheMatch[0] == null)
        {
            Log.WriteLine(nameof(_teamsInTheMatch) + " was null!", LogLevel.ERROR);
            return "Error while calculating and saving the final elo delta";
        }

        // Make the change in the player's ratings
        _teamIdsWithReportData.ElementAt(0).Value.FinalEloDelta = eloDelta;
        _teamIdsWithReportData.ElementAt(1).Value.FinalEloDelta = -eloDelta;

        Log.WriteLine("Done calculating " +
            _teamIdsWithReportData.ElementAt(0).Value.FinalEloDelta +
            " | " + _teamIdsWithReportData.ElementAt(1).Value.FinalEloDelta, LogLevel.DEBUG);

        return "";
    }

    public static void CalculateAndSaveFinalEloDeltaForMatchForfeit(
        Team[] _teamsInTheMatch, Dictionary<int, ReportData> _teamIdsWithReportData,
        int _losingTeamId)
    {
        float firstTeamSkillRating = _teamsInTheMatch[0].SkillRating;
        float secondTeamSkillRating = _teamsInTheMatch[1].SkillRating;

        Log.WriteLine("Calculating final elo points for: " + firstTeamSkillRating +
            " | " + secondTeamSkillRating, LogLevel.DEBUG);

        int winningTeamIndex = 0;

        if (_teamsInTheMatch[0].TeamId == _losingTeamId) winningTeamIndex++;

        // Duplicate code to the above method perhaps refactor
        float eloDelta = (int)(32 * (1 - winningTeamIndex - ExpectationToWin(
            firstTeamSkillRating, secondTeamSkillRating)));

        Log.WriteLine("calculated EloDelta: " + eloDelta, LogLevel.DEBUG);

        if (_teamsInTheMatch[0] == null)
        {
            Log.WriteLine(nameof(_teamsInTheMatch) + " was null!", LogLevel.ERROR);
            return; //"Error while calculating and saving the final elo delta";
        }

        // Make the change in the player's ratings
        _teamIdsWithReportData.ElementAt(0).Value.FinalEloDelta = eloDelta;
        _teamIdsWithReportData.ElementAt(1).Value.FinalEloDelta = -eloDelta;

        Log.WriteLine("Done calculating " +
            _teamIdsWithReportData.ElementAt(0).Value.FinalEloDelta +
            " | " + _teamIdsWithReportData.ElementAt(1).Value.FinalEloDelta, LogLevel.DEBUG);
    }

    public static void CalculateFinalStatsAndEloForBothTeams(
        InterfaceLeague _interfaceLeague, Team[] _teamsInTheMatch,
        Dictionary<int, ReportData> _teamIdsWithReportData)
    {
        for (int t = 0; t < _teamsInTheMatch.Length; ++t)
        {
            try
            {
                Team databaseTeam = _interfaceLeague.LeagueData.FindActiveTeamWithTeamId(_teamsInTheMatch[t].TeamId);
                ReportData reportData = _teamIdsWithReportData.ElementAt(t).Value;
                Log.WriteLine(databaseTeam.TeamId + " SR before: " + databaseTeam.SkillRating);
                databaseTeam.SkillRating += reportData.FinalEloDelta;
                Log.WriteLine(databaseTeam.TeamId + " SR after: " + databaseTeam.SkillRating);

                databaseTeam.CalculateTeamStatsAfterAMatch(reportData, _teamIdsWithReportData.ElementAt(1-t).Value);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.ERROR);
                continue;
            }
        }
    }

    private static double ExpectationToWin(float _playerOneRating, float _playerTwoRating)
    {
        return 1 / (1 + Math.Pow(10, (_playerTwoRating - _playerOneRating) / 400.0));
    }

    private static InterfaceReportingObject GetInterfaceReportingObjectByIndex(Dictionary<int, ReportData> _teamIdsWithReportData, int _index)
    {
        var baseReportingObject = _teamIdsWithReportData.ElementAt(_index).Value.ReportingObjects.FirstOrDefault(
            x => x.GetTypeOfTheReportingObject() == TypeOfTheReportingObject.REPORTEDSCORE);
        if (baseReportingObject == null)
        {
            Log.WriteLine(nameof(baseReportingObject) + " was null!", LogLevel.ERROR);
            throw new InvalidOperationException(nameof(baseReportingObject) + " was null!");
        }

        return (InterfaceReportingObject)baseReportingObject;
    }

    public static int DecideWinnerIndex(Dictionary<int, ReportData> _teamIdsWithReportData)
    {
        int winnerIndex = 0;
        try
        {
            string teamOneObjectValue = GetInterfaceReportingObjectByIndex(_teamIdsWithReportData, 0).ObjectValue;
            string teamTwoObjectValue = GetInterfaceReportingObjectByIndex(_teamIdsWithReportData, 1).ObjectValue;
            Log.WriteLine("object values: " + teamOneObjectValue + " | " + teamTwoObjectValue, LogLevel.DEBUG);

            int teamOneOutput = 0;
            if (int.TryParse(teamOneObjectValue, out int output))
            {
                teamOneOutput = output;
            }
            else
            {
                Log.WriteLine("Parse failed for value (team one): " + teamOneObjectValue, LogLevel.ERROR);
                return 3;
            }

            int teamTwoOutput = 0;
            if (int.TryParse(teamTwoObjectValue, out int outputTwo))
            {
                teamTwoOutput = outputTwo;
            }
            else
            {
                Log.WriteLine("Parse failed for value (team two): " + teamTwoObjectValue, LogLevel.ERROR);
                return 3;
            }

            Log.WriteLine("outputs: " + teamOneOutput + " | " + teamTwoOutput, LogLevel.DEBUG);

            if (teamTwoOutput > teamOneOutput)
            {
                winnerIndex = 1;
            }
            else if (teamTwoOutput == teamOneOutput)
            {
                winnerIndex = 2;
            }

            Log.WriteLine("winnerIndex is = " + winnerIndex);

            return winnerIndex;
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            throw new InvalidOperationException(nameof(InterfaceReportingObject) + " was null!");
        }
    }
}