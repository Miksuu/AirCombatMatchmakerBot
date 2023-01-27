using System.Runtime.Serialization;

public class EloSystem
{
    public string CalculateAndChangeFinalEloPoints(
    InterfaceLeague _interfaceLeague, Team[] _teamsInTheMatch, Dictionary<int, ReportData> _teamIdsWithReportData)
    {
        float firstTeamSkillRating = _teamsInTheMatch[0].SkillRating;
        float secondTeamSkillRating = _teamsInTheMatch[1].SkillRating;

        Log.WriteLine("Calculating final elo points for: " + firstTeamSkillRating +
            " | " + secondTeamSkillRating, LogLevel.VERBOSE);

        int winnerIndex = DecideWinnerIndex(_teamIdsWithReportData);

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

    public int DecideWinnerIndex(Dictionary<int, ReportData> _teamIdsWithReportData)
    {
        int winnerIndex = 0;

        string? teamOneObjectValue = _teamIdsWithReportData.ElementAt(0).Value.ReportedScore.ObjectValue;
        string? teamTwoObjectValue = _teamIdsWithReportData.ElementAt(1).Value.ReportedScore.ObjectValue;

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
}