using Discord;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class MatchScheduler
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
    [DataMember] private logVar<bool> matchSchedulerActive = new logVar<bool>();
    //[DataMember] private logConcurrentBag<int> addedTeamsToTheMatches = new logConcurrentBag<int>();

    public MatchScheduler() { }

    // TODO: Implement this method so it's executable from some command that admin can use (for initiation of a season, for example)
    public void ActivateMatchScheduler(ulong _duration, InterfaceLeague _interfaceLeague)
    {
        if (MatchSchedulerActive)
        {
            Log.WriteLine(_interfaceLeague.LeagueCategoryName + "' " + nameof(MatchScheduler) +
                " already active, returning");
            return;
        }

        Log.WriteLine("Activating " + _interfaceLeague.LeagueCategoryName + "' " + nameof(MatchScheduler) +
            " with duration: " + _duration);

        MatchSchedulerActive = true;
        new LeagueMatchSchedulerEvent(
            _duration, _interfaceLeague.LeagueCategoryId, _interfaceLeague.LeagueEventManager.ClassScheduledEvents);

        Log.WriteLine("Done activating " + _interfaceLeague.LeagueCategoryName + "' " + nameof(MatchScheduler) +
             " with duration: " + _duration, LogLevel.DEBUG);
    }

    public Response AddTeamToTheMatchSchedulerWithPlayerId(ulong _playerId, InterfaceLeague _interfaceLeague)// InterfaceMessage _interfaceMessage)
    {
        try
        {
            Team playerTeam =
                _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);

            Log.WriteLine("Team found: " + playerTeam.GetTeamName() +
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
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return new Response(ex.Message, false);
        }
    }

    public Response RemoveTeamFromTheMatchSchedulerWithPlayerId(ulong _playerId, InterfaceLeague _interfaceLeague)
    {
        Team playerTeam =
            _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);

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

    public void CheckCurrentStateOfTheMatchmakerAndAssignMatches(InterfaceLeague _interfaceLeague)
    {
        Log.WriteLine("Starting to check the status of the matchmaker with: " + TeamsInTheMatchmaker.Count);

        foreach (var teamKvp in TeamsInTheMatchmaker)
        {
            if (teamKvp.Value.TeamMatchmakingState != TeamMatchmakingState.INQUEUE)
            {
                continue;
            }

            var teamsToMatch = new List<KeyValuePair<int, TeamMatchmakerData>>();

            Log.WriteLine("Looping on: " + teamKvp.Key + " with state: " + teamKvp.Value.TeamMatchmakingState);

            var foundTeam = GetAvailableTeamToChallenge(teamKvp.Key);

            Log.WriteLine("Team: " + foundTeam + " not in the queue");

            if (foundTeam == 0)
            {
                Log.WriteLine("No teams found to challenge, returning", LogLevel.DEBUG);
                continue;
            }

            var foundTeamKvp = TeamsInTheMatchmaker.First(x => x.Key == foundTeam);

            teamsToMatch.Add(teamKvp);
            teamsToMatch.Add(foundTeamKvp);

            MatchTwoTeamsTogether(teamsToMatch, _interfaceLeague);

            break;
        }
    }

    private async void MatchTwoTeamsTogether(List<KeyValuePair<int, TeamMatchmakerData>> _teamsToMatch, InterfaceLeague _interfaceLeague)
    {
        Log.WriteLine("Matching found opponent: " + _teamsToMatch[0].Key + " vs seeker: " + _teamsToMatch[1].Key, LogLevel.DEBUG);

        var teamsArray = _teamsToMatch.ToArray();

        for (int t = 0; t < teamsArray.Length; t++)
        {
            teamsArray[t].Value.SetValuesOnFindingAMatch(teamsArray[1 - t].Key);
            Log.WriteLine("Setting values on team: " + teamsArray[t].Key);
        }

        await _interfaceLeague.LeagueData.Matches.CreateAMatch(
            teamsArray.Select(x => x.Key).ToArray(),
            MatchState.SCHEDULINGPHASE,
            _interfaceLeague, true
        );

        Log.WriteLine("Match creation request sent for teams: " + teamsArray[0].Key + " and " + teamsArray[1].Key);
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
            Log.WriteLine("Looping on: " + teamKvp.Key + " with state: " + teamKvp.Value.TeamMatchmakingState);// +
              //  " with priority: " + teamKvp.Value.TeamMissedMatchesFromScheduler);

            if (teamKvp.Value.TeamThatWasFoughtPreviously == _teamIdSearching)
            {
                continue;
            }

            return teamKvp.Key;
        }

        Log.WriteLine("No available team found for: " + _teamIdSearching);
        return 0;
    }
}