using Discord.WebSocket;
using System.Runtime.Serialization;

[DataContract]
public class MatchReporting
{
    public Dictionary<int, int> TeamIdWithReportedResult
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamIdWithReportedResult) + " with count of: " +
                teamIdWithReportedResult.Count, LogLevel.VERBOSE);
            return teamIdWithReportedResult;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamIdWithReportedResult)
                + " to: " + value, LogLevel.VERBOSE);
            teamIdWithReportedResult = value;
        }
    }

    [DataMember] private Dictionary<int, int> teamIdWithReportedResult { get; set; }

    public MatchReporting()
    {
        teamIdWithReportedResult = new Dictionary<int, int>();
    }

    public Task<string> ReportMatchResult(
        InterfaceLeague _interfaceLeague, ulong _playerId, int _playerReportedResult)
    {
        string response = string.Empty;

        Team? reportingTeam = _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeague(_playerId);
        if (reportingTeam == null)
        {
            Log.WriteLine(nameof(reportingTeam) + " was null!", LogLevel.CRITICAL);
            return Task.FromResult(response);
        }

        // First time pressing the report button for the team
        if (!TeamIdWithReportedResult.ContainsKey(reportingTeam.TeamId))
        {
            Log.WriteLine("Key wasn't found, the team is first time reporting.", LogLevel.VERBOSE);
            TeamIdWithReportedResult.Add(reportingTeam.TeamId, _playerReportedResult);
            response = "You reported score of: " + _playerReportedResult;
        }
        // Replacing the result
        else
        {
            Log.WriteLine("Key was, the team is not their first time reporting.", LogLevel.VERBOSE);
            TeamIdWithReportedResult[reportingTeam.TeamId] = _playerReportedResult;
            response = "You replaced the reported score to: " + _playerReportedResult;
        }

        foreach (var reportedTeamKvp in TeamIdWithReportedResult)
        {
            Log.WriteLine("Reported team: " + reportedTeamKvp.Key +
                " with value: " + reportedTeamKvp.Value, LogLevel.VERBOSE);
        }

        int reportedTeamsCount = TeamIdWithReportedResult.Count;

        Log.WriteLine("Reported teams count: " + reportedTeamsCount, LogLevel.VERBOSE);

        if (reportedTeamsCount > 2)
        {
            Log.WriteLine("Count was: " + reportedTeamsCount + ", Error!", LogLevel.ERROR);

            // Maybe handle the error
        }

        return Task.FromResult(response);
    }

    private void CalculatePoints(int _winnerIndex)
    {
        int eloDelta = (int)(32 * (1 - _winnerIndex - ExpectationToWin(pids.ElementAt(0).Value, pids.ElementAt(1).Value)));

        Console.WriteLine("EloDelta: " + eloDelta);

        // Make the change in the player's ratings
        Database.Instance.PlayerManager.PlayerIDs[pids.ElementAt(0).Key].skillRating += eloDelta;
        Database.Instance.PlayerManager.PlayerIDs[pids.ElementAt(1).Key].skillRating -= eloDelta;
    }

    private double ExpectationToWin(int _playerOneRating, int _playerTwoRating)
    {
        return 1 / (1 + Math.Pow(10, (_playerTwoRating - _playerOneRating) / 400.0));
    }
}