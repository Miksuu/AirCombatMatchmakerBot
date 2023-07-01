using Discord;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class MatchScheduler : logClass<MatchScheduler>
{
    [IgnoreDataMember]
    public ConcurrentDictionary<int, TeamMatchmakerData> TeamsInTheMatchmaker
    {
        get => teamsInTheMatchmaker.GetValue();
        set => teamsInTheMatchmaker.SetValue(value);
    }

    public bool MatchSchedulerActive
    {
        get => matchSchedulerActive.GetValue();
        set => matchSchedulerActive.SetValue(value);
    }

    //public ConcurrentBag<int> AddedTeamsToTheMatches
    //{
    //    get => addedTeamsToTheMatches.GetValue();
    //    set => addedTeamsToTheMatches.SetValue(value);
    //}

    [DataMember] private logConcurrentDictionary<int, TeamMatchmakerData> teamsInTheMatchmaker =
        new logConcurrentDictionary<int, TeamMatchmakerData>();
    [DataMember] private logClass<bool> matchSchedulerActive = new logClass<bool>();
    //[DataMember] private logConcurrentBag<int> addedTeamsToTheMatches = new logConcurrentBag<int>();

    // Doesn't need to be serialized, it's gotten from a class that loads the data from it's serialization
    public InterfaceLeague interfaceLeagueRef;

    public MatchScheduler() { }

    // TODO: Implement this method so it's executable from some command that admin can use (for initiation of a season, for example)
    public void ActivateMatchScheduler(ulong _duration)
    {
        if (MatchSchedulerActive)
        {
            Log.WriteLine(interfaceLeagueRef.LeagueCategoryName + "' " + nameof(MatchScheduler) +
                " already active, returning");
            return;
        }

        Log.WriteLine("Activating " + interfaceLeagueRef.LeagueCategoryName + "' " + nameof(MatchScheduler) +
            " with duration: " + _duration);

        MatchSchedulerActive = true;
        new LeagueMatchSchedulerEvent(
            _duration, interfaceLeagueRef.LeagueCategoryId, interfaceLeagueRef.LeagueEventManager.ClassScheduledEvents);

        Log.WriteLine("Done activating " + interfaceLeagueRef.LeagueCategoryName + "' " + nameof(MatchScheduler) +
             " with duration: " + _duration, LogLevel.DEBUG);
    }

    public Response AddTeamToTheMatchSchedulerWithPlayerId(ulong _playerId)// InterfaceMessage _interfaceMessage)
    {
        try
        {
            Team playerTeam =
                interfaceLeagueRef.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);

            Log.WriteLine("Team found: " + playerTeam.GetTeamName(interfaceLeagueRef.LeaguePlayerCountPerTeam) +
                " (" + playerTeam.TeamId + ")" + " adding it to the challenge queue.");

            if (TeamsInTheMatchmaker.Any(x => x.Key == playerTeam.TeamId))
            {
                Log.WriteLine(_playerId + " already in the matchmaker!");
                return new Response("You are already in the matchmaker!", false);
            }

            TeamsInTheMatchmaker.TryAdd(playerTeam.TeamId, new TeamMatchmakerData());

            return new Response("Successfully joined!", true);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return new Response(ex.Message, false);
        }
    }

    public Response RemoveTeamFromTheMatchSchedulerWithPlayerId(ulong _playerId)
    {
        Team playerTeam =
            interfaceLeagueRef.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);

        Log.WriteLine("Removing Team: " + playerTeam + " (" +
            playerTeam.TeamId + ") from the queue");

        bool removed = TeamsInTheMatchmaker
            .TryRemove(new KeyValuePair<int, TeamMatchmakerData>(playerTeam.TeamId, null));

        Log.WriteLine("Done removing: " + removed + "the team from the queue. Count is now: " +
            TeamsInTheMatchmaker.Count);

        if (removed)
        {
            Log.WriteLine("Removed team: " + playerTeam.TeamId, LogLevel.DEBUG);
            return new Response("Successfully removed your team from the matchmaker", true);
        }
        else
        {
            Log.WriteLine("Failed to find team: " + playerTeam.TeamId);
            return new Response("Could not find the team in the matchmaker!", false);
        }
    }

    public void CheckCurrentStateOfTheMatchmakerAndAssignMatches()
    {
        Log.WriteLine("Starting to check the status of the matchmaker with: " + TeamsInTheMatchmaker.Count);

        // Sort teams based on TeamMissedMatchesFromScheduler in descending order
        var sortedTeams = TeamsInTheMatchmaker.OrderByDescending(x => x.Value.TeamMissedMatchesFromScheduler).ToList();

        sortedTeams.RemoveAll(x => x.Value.TeamMatchmakingState == TeamMatchmakingState.INMATCH);
        if (sortedTeams.Count < 2)
        {
            return;
        }

        Log.WriteLine("Starting to loop through teams (Count: " + sortedTeams.Count + ")");

        foreach (var teamKvp in sortedTeams)
        {
            Log.WriteLine("Looping on: " + teamKvp.Key + " with state: " + teamKvp.Value.TeamMatchmakingState +
                " with priority: " + teamKvp.Value.TeamMissedMatchesFromScheduler);

            var foundTeam = GetAvailableTeamToChallenge(teamKvp.Key);

            Log.WriteLine("Team: " + foundTeam + " not in the queue");

            if (foundTeam == 0)
            {
                Log.WriteLine("No teams found to challenge, returning", LogLevel.DEBUG);
                continue;
            }

            var foundTeamKvp = TeamsInTheMatchmaker.First(x => x.Key == foundTeam);

            Log.WriteLine("Incrementing teamId: " + foundTeamKvp.Key + " with value: " + foundTeamKvp.Value.TeamMissedMatchesFromScheduler +
                " and state: " + foundTeamKvp.Value.TeamMatchmakingState);

            TeamsInTheMatchmaker.First(x => x.Key == foundTeamKvp.Key).Value.TeamMissedMatchesFromScheduler++;

            MatchTwoTeamsTogether(foundTeamKvp, teamKvp);

            break;
        }
    }


    public int GetRandomTeamId(List<int> _teamIds)
    {
        Random random = new Random();
        int index = random.Next(_teamIds.Count);
        var foundTeamId = _teamIds[index];
        Log.WriteLine("Found teamId: " + foundTeamId + " from amount of: " + _teamIds.Count);
        return _teamIds[index];
    }

    private int GetAvailableTeamToChallenge(int _teamIdSearching)
    {
        Log.WriteLine("Starting to see what teams are available to challenge: " + TeamsInTheMatchmaker.Count);

        var teamSearching = TeamsInTheMatchmaker.First(x => x.Key == _teamIdSearching);

        var sortedTeams = TeamsInTheMatchmaker
            .Where(x => x.Value.TeamMatchmakingState == TeamMatchmakingState.INQUEUE && x.Key != _teamIdSearching).ToList();

        Log.WriteLine("Sorted teams count: " + sortedTeams.Count);

        foreach (var teamKvp in sortedTeams)
        {
            Log.WriteLine("Looping on: " + teamKvp.Key + " with state: " + teamKvp.Value.TeamMatchmakingState +
                " with priority: " + teamKvp.Value.TeamMissedMatchesFromScheduler);

            if (teamKvp.Value.TeamThatWasFoughtPreviously == _teamIdSearching)
            {
                continue;
            }

            return teamKvp.Key;
        }

        Log.WriteLine("No available team found for: " + _teamIdSearching);
        return 0;
    }

    private async void MatchTwoTeamsTogether
        (KeyValuePair<int, TeamMatchmakerData> _foundOpponentTeam, KeyValuePair<int, TeamMatchmakerData> _seekingTeam)
    {
        Log.WriteLine("Matching found opponent: " + _foundOpponentTeam.Key + " [" + _foundOpponentTeam.Value.TeamMissedMatchesFromScheduler +
            "] vs seeker:" + _seekingTeam.Key + +_foundOpponentTeam.Value.TeamMissedMatchesFromScheduler +
            "]", LogLevel.DEBUG);

        Log.WriteLine("Teams left the matchmaker: ", LogLevel.VERBOSE);
        var sortedTeams = TeamsInTheMatchmaker.OrderByDescending(x => x.Value.TeamMissedMatchesFromScheduler);
        foreach (var item in sortedTeams)
        {
            Log.WriteLine("[" + item.Value.TeamMissedMatchesFromScheduler + "] id: " + item.Key + " | " + item.Value.TeamMatchmakingState);
        }

        _foundOpponentTeam.Value.SetValuesOnFindingAMatch(_seekingTeam.Key);
        _seekingTeam.Value.SetValuesOnFindingAMatch(_foundOpponentTeam.Key);

        int[] teams = new int[2]
        {
            _foundOpponentTeam.Key,
            _seekingTeam.Key,
        };

        await interfaceLeagueRef.LeagueData.Matches.CreateAMatch(teams, MatchState.SCHEDULINGPHASE, true);
    }
}