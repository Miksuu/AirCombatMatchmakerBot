using Discord;
using Discord.WebSocket;
using System.Runtime.Serialization;

[DataContract]
public class MatchReporting
{
    public Dictionary<int, int> TeamIdsWithReportedResult
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamIdsWithReportedResult) + " with count of: " +
                teamIdsWithReportedResult.Count, LogLevel.VERBOSE);
            return teamIdsWithReportedResult;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamIdsWithReportedResult)
                + " to: " + value, LogLevel.VERBOSE);
            teamIdsWithReportedResult = value;
        }
    }

    [DataMember] private Dictionary<int, int> teamIdsWithReportedResult { get; set; }

    public MatchReporting()
    {
        teamIdsWithReportedResult = new Dictionary<int, int>();
    }

    public Task<string> ReportMatchResult(
        InterfaceLeague _interfaceLeague, ulong _playerId, int _playerReportedResult)
    {
        string response = string.Empty;

        Team? reportingTeam = _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);
        if (reportingTeam == null)
        {
            Log.WriteLine(nameof(reportingTeam) + " was null!", LogLevel.CRITICAL);
            return Task.FromResult(response);
        }

        // First time pressing the report button for the team
        if (!TeamIdsWithReportedResult.ContainsKey(reportingTeam.TeamId))
        {
            Log.WriteLine("Key wasn't found, the team is first time reporting.", LogLevel.VERBOSE);
            TeamIdsWithReportedResult.Add(reportingTeam.TeamId, _playerReportedResult);
            response = "You reported score of: " + _playerReportedResult;
        }
        // Replacing the result
        else
        {
            Log.WriteLine("Key was, the team is not their first time reporting.", LogLevel.VERBOSE);
            TeamIdsWithReportedResult[reportingTeam.TeamId] = _playerReportedResult;
            response = "You replaced the reported score to: " + _playerReportedResult;
        }

        foreach (var reportedTeamKvp in TeamIdsWithReportedResult)
        {
            Log.WriteLine("Reported team: " + reportedTeamKvp.Key +
                " with value: " + reportedTeamKvp.Value, LogLevel.VERBOSE);
        }

        int reportedTeamsCount = TeamIdsWithReportedResult.Count;

        Log.WriteLine("Reported teams count: " + reportedTeamsCount, LogLevel.VERBOSE);

        if (reportedTeamsCount > 2)
        {
            Log.WriteLine("Count was: " + reportedTeamsCount + ", Error!", LogLevel.ERROR);

            // Maybe handle the error
            return Task.FromResult(response);
        }

        if (reportedTeamsCount == 2)
        {
            Log.WriteLine("count is 2", LogLevel.DEBUG);
            response = CalculateFinalMatchResult(_interfaceLeague, reportingTeam);
        }

        return Task.FromResult(response);
    }

    private string CalculateFinalMatchResult(InterfaceLeague _interfaceLeague, Team _reportingTeam)
    {
        
        /*
        Log.WriteLine("Starting to calculate the final match result with teams: " +
            TeamIdsWithReportedResult[0] + " and: " + TeamIdsWithReportedResult[1], LogLevel.VERBOSE);
        */

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

        int otherTeamId = TeamIdsWithReportedResult.FirstOrDefault(t => t.Key != _excludedTeamId).Key;
        Log.WriteLine("Found other team id: " + otherTeamId, LogLevel.VERBOSE);
        return _interfaceLeague.LeagueData.FindTeamWithTeamId(otherTeamId);
    }

    private int DecideWinnerIndex()
    {
        int winnerIndex = 0;

        if (TeamIdsWithReportedResult.ElementAt(1).Value > TeamIdsWithReportedResult.ElementAt(0).Value)
        {
            winnerIndex = 1;
        }
        else if (TeamIdsWithReportedResult.ElementAt(1).Value == TeamIdsWithReportedResult.ElementAt(0).Value)
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

        if (_interfaceLeague == null)
        {
            Log.WriteLine(nameof(_interfaceLeague) + " was null!", LogLevel.CRITICAL);
            return "";
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

        return "Success";
    }

    private double ExpectationToWin(float _playerOneRating, float _playerTwoRating)
    {
        return 1 / (1 + Math.Pow(10, (_playerTwoRating - _playerOneRating) / 400.0));
    }
}