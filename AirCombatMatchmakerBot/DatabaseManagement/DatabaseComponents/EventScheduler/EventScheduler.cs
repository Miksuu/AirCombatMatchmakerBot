using Discord;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Runtime.Serialization;

[DataContract]
public class EventScheduler : logClass<EventScheduler>
{
    public ulong LastUnixTimeCheckedOn
    {
        get => lastUnixTimeExecutedOn.GetValue();
        set => lastUnixTimeExecutedOn.SetValue(value);
    }

    public int EventCounter
    {
        get => eventCounter.GetValue();
        set => eventCounter.SetValue(value);
    }

    [DataMember] private logClass<ulong> lastUnixTimeExecutedOn = new logClass<ulong>();
    [DataMember] private logClass<int> eventCounter = new logClass<int>();

    public EventScheduler() { }

    public async Task CheckCurrentTimeAndExecuteScheduledEvents(bool _clearEventOnTheStartup = false)
    {
        ulong currentUnixTime = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();

        Log.WriteLine("Time: " + currentUnixTime + " with: " +
            nameof(_clearEventOnTheStartup) + ": " + _clearEventOnTheStartup);

        // Might get caused by the daylight savings
        if (currentUnixTime < LastUnixTimeCheckedOn)
        {
            Log.WriteLine("Current unix time was smaller than last unix time that was checked on!", LogLevel.ERROR);
        }

        LastUnixTimeCheckedOn = currentUnixTime;

        foreach (InterfaceLeague storedLeague in Database.Instance.Leagues.StoredLeagues)
        {
            storedLeague.HandleLeaguesAndItsMatchesEvents(currentUnixTime);
        }
    }

    public async void EventSchedulerLoop()
    {
        int waitTimeInMs = 1000;

        Log.WriteLine("Starting to execute " + nameof(EventSchedulerLoop), LogLevel.DEBUG);

        while (true)
        {
            Log.WriteLine("Executing " + nameof(CheckCurrentTimeAndExecuteScheduledEvents));

            await CheckCurrentTimeAndExecuteScheduledEvents();
            Log.WriteLine("Done executing " + nameof(CheckCurrentTimeAndExecuteScheduledEvents) +
                ", waiting " + waitTimeInMs + "ms");

            Thread.Sleep(waitTimeInMs);

            Log.WriteLine("Wait done.");
        }
    }
}