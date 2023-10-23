using System.Collections.Concurrent;

public static class AdditionalEvents
{
    public static ConcurrentBag<ScheduledEvent> GetAdditionalEvents()
    {
        ConcurrentBag<ScheduledEvent> events = new ConcurrentBag<ScheduledEvent>();

        foreach (var league in Database.GetInstance<ApplicationDatabase>().Leagues.StoredLeagues)
        {
            foreach (var leagueEvent in league.GetLeagueEventsAndReturnThemInConcurrentBag())
            {
                events.Add(leagueEvent);
            }
        }

        return events;
    }
}