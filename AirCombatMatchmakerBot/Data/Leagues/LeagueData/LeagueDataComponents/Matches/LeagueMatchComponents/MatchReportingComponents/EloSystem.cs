using System.Runtime.Serialization;

public class EloSystem
{
    public string CalculateAndSaveFinalEloDelta(
    InterfaceLeague _interfaceLeague, Team[] _teamsInTheMatch, Dictionary<int, ReportData> _teamIdsWithReportData)
    {
        float firstTeamSkillRating = _teamsInTheMatch[0].SkillRating;
        float secondTeamSkillRating = _teamsInTheMatch[1].SkillRating;

        /*
        _teamIdsWithReportData.ElementAt(0).Value.CachedSkillRating = firstTeamSkillRating;
        _teamIdsWithReportData.ElementAt(1).Value.CachedSkillRating = firstTeamSkillRating;
        */

        Log.WriteLine("Calculating final elo points for: " + firstTeamSkillRating +
            " | " + secondTeamSkillRating, LogLevel.DEBUG);

        int winnerIndex = DecideWinnerIndex(_teamIdsWithReportData);

        if (winnerIndex == 2)
        {
            // Handle this ?
            return "The match cannot be a draw!";
        }

        Log.WriteLine("Before calculating elo delta", LogLevel.DEBUG);

        float eloDelta = (int)(32 * (1 - winnerIndex - ExpectationToWin(
            firstTeamSkillRating, secondTeamSkillRating)));

        Log.WriteLine("calculated EloDelta: " + eloDelta, LogLevel.DEBUG);


        if (_teamsInTheMatch[0] == null)
        {
            Log.WriteLine(nameof(_teamsInTheMatch) + " was null!", LogLevel.CRITICAL);
            return "Error while calculating and saving the final elo delta";
        }

        /*
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
        }*/

        // Make the change in the player's ratings
        _teamIdsWithReportData.ElementAt(0).Value.FinalEloDelta = eloDelta;
        _teamIdsWithReportData.ElementAt(1).Value.FinalEloDelta = -eloDelta;

        Log.WriteLine("Done calculating and changing elo points for: " +
            _teamIdsWithReportData.ElementAt(0).Value.FinalEloDelta +
            " | " + _teamIdsWithReportData.ElementAt(1).Value.FinalEloDelta, LogLevel.DEBUG);

        return "";
    }

    private double ExpectationToWin(float _playerOneRating, float _playerTwoRating)
    {
        return 1 / (1 + Math.Pow(10, (_playerTwoRating - _playerOneRating) / 400.0));
    }

    public int DecideWinnerIndex(Dictionary<int, ReportData> _teamIdsWithReportData)
    {
        int winnerIndex = 0;

        string? teamOneObjectValue = _teamIdsWithReportData.ElementAt(0).Value.ReportedScore.ObjectValue;
        string? teamTwoObjectValue = _teamIdsWithReportData.ElementAt(1).Value.ReportedScore.ObjectValue;

        Log.WriteLine("object values: " + teamOneObjectValue + " | " + teamTwoObjectValue, LogLevel.DEBUG);

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
        if (int.TryParse(teamTwoObjectValue, out int outputTwo))
        {
            teamTwoOutput = outputTwo;
        }
        else
        {
            Log.WriteLine("Parse failed for value (team two): " + teamTwoObjectValue, LogLevel.CRITICAL);
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

        Log.WriteLine("winnerIndex is = " + winnerIndex, LogLevel.VERBOSE);

        return winnerIndex;
    }
}