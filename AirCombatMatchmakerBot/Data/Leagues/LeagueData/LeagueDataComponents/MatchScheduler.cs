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

    [DataMember] private logConcurrentDictionary<int, TeamMatchmakerData> teamsInTheMatchmaker =
        new logConcurrentDictionary<int, TeamMatchmakerData>();
    [DataMember] private logClass<bool> matchSchedulerActive = new logClass<bool>();

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
        var sortedTeams = TeamsInTheMatchmaker.OrderByDescending(x => x.Value.TeamMissedMatchesFromScheduler);

        foreach (var teamKvp in sortedTeams)
        {
            int teamId = teamKvp.Key;
            TeamMatchmakingState teamMatchmakingState = teamKvp.Value.TeamMatchmakingState;

            Log.WriteLine("Looping on: " + teamId + " with state: " + teamMatchmakingState);

            if (teamMatchmakingState == TeamMatchmakingState.INQUEUE)
            {
                var availableTeamsToChallenge = GetAvailableTeamsToChallenge(teamId);

                if (availableTeamsToChallenge.Count > 0)
                {
                    int randomTeamId = GetRandomTeamId(availableTeamsToChallenge);

                    Log.WriteLine("Matching two teams together - Team 1: " + randomTeamId + ", Team 2: " + teamId);

                    MatchTwoTeamsTogether(TeamsInTheMatchmaker.First(x => x.Key == randomTeamId), teamKvp);
                }
                else
                {
                    Log.WriteLine("No teams were available to challenge from: " + teamId + ", continuing", LogLevel.DEBUG);
                    continue;
                }
            }
        }
    }

    public int GetRandomTeamId(List<int> _teamIds)
    {
        Random random = new Random();
        int index = random.Next(_teamIds.Count);
        return _teamIds[index];
    }

    private List<int> GetAvailableTeamsToChallenge(int _teamIdNotToLookFor)
    {
        List<int> availableTeamIdsToChallenge = new List<int>();

        Log.WriteLine("Starting to see what teams are available to challenge: " + TeamsInTheMatchmaker.Count);

        foreach (var teamKvp in TeamsInTheMatchmaker)
        {
            int teamId = teamKvp.Key;
            TeamMatchmakingState teamMatchmakingState = teamKvp.Value.TeamMatchmakingState;

            Log.WriteLine("Looping on: " + teamId + " with state: " + teamMatchmakingState);

            if (teamId == _teamIdNotToLookFor)
            {
                Log.WriteLine(teamId + " skipped");
                continue;
            }

            if (teamMatchmakingState == TeamMatchmakingState.INQUEUE)
            {
                Log.WriteLine("Found team: " + teamId + " adding them to the list", LogLevel.DEBUG);
                availableTeamIdsToChallenge.Add(teamId);
            }
        }

        // Add more proper debugging here?
        Log.WriteLine("Returning with a count of: " + availableTeamIdsToChallenge.Count);

        foreach (var item in availableTeamIdsToChallenge)
        {
            Log.WriteLine(item + " available to challenge vs: " + _teamIdNotToLookFor);
        }

        return availableTeamIdsToChallenge;
    }

    private async void MatchTwoTeamsTogether
        (KeyValuePair<int, TeamMatchmakerData> _foundOpponentTeam, KeyValuePair<int, TeamMatchmakerData> _seekingTeam)
    {
        // Create a method to enter a team in to a match
        _foundOpponentTeam.Value.TeamMatchmakingState = TeamMatchmakingState.INMATCH;
        _seekingTeam.Value.TeamMatchmakingState = TeamMatchmakingState.INMATCH;
        _foundOpponentTeam.Value.TeamMissedMatchesFromScheduler = 0;
        _seekingTeam.Value.TeamMissedMatchesFromScheduler = 0;

        Log.WriteLine("Matching found opponent: " + _foundOpponentTeam.Key + " vs seeker:" + _seekingTeam.Key, LogLevel.DEBUG);

        int[] teams = new int[2]
        {
            _foundOpponentTeam.Key,
            _seekingTeam.Key,
        };

        await interfaceLeagueRef.LeagueData.Matches.CreateAMatch(teams, MatchState.SCHEDULINGPHASE, true);
    }
}