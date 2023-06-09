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
    public void ActivateMatchScheduler(int _duration)
    {
        if (MatchSchedulerActive)
        {
            Log.WriteLine(interfaceLeagueRef.LeagueCategoryName + "' " + nameof(MatchScheduler) +
                " already active, returning", LogLevel.VERBOSE);
            return;
        }

        Log.WriteLine("Activating " + interfaceLeagueRef.LeagueCategoryName + "' " + nameof(MatchScheduler) +
            " with duration: " + _duration, LogLevel.VERBOSE);

        MatchSchedulerActive = true;
        new LeagueMatchSchedulerEvent(_duration, interfaceLeagueRef.LeagueCategoryId);

        Log.WriteLine("Done activating " + interfaceLeagueRef.LeagueCategoryName + "' " + nameof(MatchScheduler) +
             " with duration: " + _duration, LogLevel.DEBUG);
    }

    public void DeActivateMatchScheduler()
    {

    }

    public Response AddTeamToTheMatchSchedulerWithPlayerId(ulong _playerId)// InterfaceMessage _interfaceMessage)
    {
        try
        {
            Team playerTeam =
                interfaceLeagueRef.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);

            Log.WriteLine("Team found: " + playerTeam.GetTeamName(interfaceLeagueRef.LeaguePlayerCountPerTeam) +
                " (" + playerTeam.TeamId + ")" + " adding it to the challenge queue.", LogLevel.VERBOSE);

            if (TeamsInTheMatchmaker.Any(x => x.Key == playerTeam.TeamId))
            {
                Log.WriteLine(_playerId + " already in the matchmaker!", LogLevel.VERBOSE);
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
            playerTeam.TeamId + ") from the queue", LogLevel.VERBOSE);

        bool removed = TeamsInTheMatchmaker
            .TryRemove(new KeyValuePair<int, TeamMatchmakerData>(playerTeam.TeamId, null));

        Log.WriteLine("Done removing: " + removed + "the team from the queue. Count is now: " +
            TeamsInTheMatchmaker.Count, LogLevel.VERBOSE);

        if (removed)
        {
            Log.WriteLine("Removed team: " + playerTeam.TeamId, LogLevel.DEBUG);
            return new Response("Successfully removed your team from the matchmaker", true);
        }
        else
        {
            Log.WriteLine("Failed to find team: " + playerTeam.TeamId, LogLevel.VERBOSE);
            return new Response("Could not find the team in the matchmaker!", false);
        }
    }

    public void CheckCurrentStateOfTheMatchmakerAndAssignMatches()
    {
        Log.WriteLine("Starting to check the status of the matchmaker with: " + TeamsInTheMatchmaker.Count, LogLevel.VERBOSE);

        foreach (var teamKvp in TeamsInTheMatchmaker)
        {
            int teamId = teamKvp.Key;
            TeamMatchmakingState teamMatchmakingState = teamKvp.Value.TeamMatchmakingState;

            Log.WriteLine("Looping on: " + teamId + " with state: " + teamMatchmakingState, LogLevel.VERBOSE);

            if (teamMatchmakingState == TeamMatchmakingState.INQUEUE)
            {
                var availableTeamsToChallenge = GetAvailableTeamsToChallenge(teamId);

                //Log.WriteLine("Available teams to match for: " + teamId + ": " + availableTeamsToChallenge)

                // Add some mechanic here to avoid matching against same teams constantly, if there's multiple choices?
                if (availableTeamsToChallenge.Count > 0)
                {
                    int randomTeamId = GetRandomTeamId(availableTeamsToChallenge);
                    MatchTwoTeamsTogether(TeamsInTheMatchmaker.First(x => x.Key == randomTeamId), teamKvp);
                }
                else
                {
                    Log.WriteLine("No teams were available to challenge to from: " + teamId + ", continuing", LogLevel.DEBUG);
                    // Might use return here to save performance before skill based matchmaking is added, unnecessary?
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

        Log.WriteLine("Starting to see what teams are available to challenge: " + TeamsInTheMatchmaker.Count, LogLevel.VERBOSE);

        foreach (var teamKvp in TeamsInTheMatchmaker)
        {
            int teamId = teamKvp.Key;
            TeamMatchmakingState teamMatchmakingState = teamKvp.Value.TeamMatchmakingState;

            Log.WriteLine("Looping on: " + teamId + " with state: " + teamMatchmakingState, LogLevel.VERBOSE);

            if (teamId == _teamIdNotToLookFor)
            {
                Log.WriteLine(teamId + " skipped", LogLevel.VERBOSE);
                continue;
            }

            if (teamMatchmakingState == TeamMatchmakingState.INQUEUE)
            {
                Log.WriteLine("Found team: " + teamId + " adding them to the list", LogLevel.DEBUG);
                availableTeamIdsToChallenge.Add(teamId);
            }
        }

        // Add more proper debugging here?
        Log.WriteLine("Returning with a count of: " + availableTeamIdsToChallenge.Count, LogLevel.VERBOSE);

        foreach (var item in availableTeamIdsToChallenge)
        {
            Log.WriteLine(item + " available to challenge vs: " + _teamIdNotToLookFor, LogLevel.VERBOSE);
        }

        return availableTeamIdsToChallenge;
    }

    private async void MatchTwoTeamsTogether
        (KeyValuePair<int, TeamMatchmakerData> _foundOpponentTeam, KeyValuePair<int, TeamMatchmakerData> _seekingTeam)
    {
        _foundOpponentTeam.Value.TeamMatchmakingState = TeamMatchmakingState.INMATCH;
        _seekingTeam.Value.TeamMatchmakingState = TeamMatchmakingState.INMATCH; 

        Log.WriteLine("Matching found opponent: " + _foundOpponentTeam.Key + " vs seeker:" + _seekingTeam.Key, LogLevel.DEBUG);

        int[] teams = new int[2]
        {
            _foundOpponentTeam.Key,
            _seekingTeam.Key,
        };

        await interfaceLeagueRef.LeagueData.Matches.CreateAMatch(teams, MatchState.SCHEDULINGPHASE);
    }
}