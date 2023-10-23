using System.Collections.Concurrent;

public static class AdditionalEvents
{
    public static void HandleAdditionalEvents(ulong _currentUnixTime)
    {
        foreach (var league in Database.GetInstance<ApplicationDatabase>().Leagues.StoredLeagues)
        {
            league.LeagueEventManager.HandleEvents(_currentUnixTime);

            foreach(var match in league.LeagueData.Matches.MatchesConcurrentBag)
            {
                match.MatchEventManager.HandleEvents(_currentUnixTime);
            }
        }
    }
}