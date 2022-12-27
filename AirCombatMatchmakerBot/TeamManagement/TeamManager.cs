public static class TeamManager
{
    public static Team? FindActiveTeamByPlayerIdInAPredefinedLeague(
        ILeague _dbLeagueInstance, ulong _playerId)
    {
        Log.WriteLine("Starting to find a active team by player id: " + _playerId +
            " in league: " + _dbLeagueInstance.LeagueCategoryName, LogLevel.VERBOSE);

        foreach (Team team in _dbLeagueInstance.LeagueData.Teams.GetListOfTeams())
        {
            Team? foundTeam = team.CheckIfTeamIsActiveAndContainsAPlayer(_playerId);

            if (foundTeam != null) 
            {
                return foundTeam;
            }
        }

        Log.WriteLine("Team not found! Admin trying to access challenge" +
    " of a league that he's not registered to?", LogLevel.WARNING);

        return null;
    }
}