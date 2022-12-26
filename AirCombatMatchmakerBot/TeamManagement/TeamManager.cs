public static class TeamManager
{
    public static Team FindActiveTeamByPlayerIdInAPredefinedLeague(ILeague _dbLeagueInstance, ulong _playerId)
    {
        Log.WriteLine("Starting to find a active team by player id: " + _playerId +
            " in league: " + _dbLeagueInstance.LeagueCategoryName, LogLevel.VERBOSE);

        foreach (Team team in _dbLeagueInstance.LeagueData.Teams)
        {
            Log.WriteLine("Starting to loop through team: " + team.teamName + " (" + team.teamId +
                ") with player count of: " + team.players.Count + " with player id: " + _playerId +
                " in league: " + _dbLeagueInstance.LeagueCategoryName, LogLevel.VERBOSE);

            bool teamActive = team.teamActive;
            bool teamContainsPlayer = team.players.Any(x => x.playerDiscordId == _playerId);
            Log.WriteLine("active: " + teamActive + " | teamContainsPlayer: " + teamContainsPlayer, LogLevel.VERBOSE);

            if (teamActive && teamContainsPlayer)
            {
                Log.WriteLine("Found: " + _playerId + " in team: " + team.teamName + " (" + team.teamId + ")" +
                    " in league: " + _dbLeagueInstance.LeagueCategoryName +
                    " that has SR of: " + team.skillRating, LogLevel.DEBUG);
                return team;
            }
        }

        Log.WriteLine("Team not found! Admin trying to access challenge" +
            " of a league that he's not registered to?", LogLevel.WARNING);

        return null;
    }

    // Returns the teams in the queue as a string (useful for printing, in log on the challenge channel)
    public static string ReturnTeamsInTheQueueOfAChallenge(List<Team> _teams, int _leagueTeamSize)
    {
        string teamsString = string.Empty;
        foreach (Team team in _teams)
        {
            teamsString += "[" + team.skillRating + "] " + team.teamName;
            if (_leagueTeamSize > 1)
            {
                teamsString += " (" + GetTeamMembersInAString(team) + ")";
            }
            teamsString += "\n";
        }
        return teamsString;
    }

    private static string GetTeamMembersInAString(Team _team)
    {
        string playersInATeam = string.Empty;
        for (int p = 0; p < _team.players.Count; p++)
        {
            playersInATeam += _team.players[p];
            if (p != _team.players.Count - 1) playersInATeam += ", ";
        }
        return playersInATeam;
    }
}