using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;
using System.IO;
using System.Threading.Channels;

[DataContract]
public class ChallengeStatus : logClass<ChallengeStatus>, InterfaceLoggableClass
{
    public ConcurrentBag<int> TeamsInTheQueue
    {
        get => teamsInTheQueue.GetValue();
        set => teamsInTheQueue.SetValue(value);
    }

    [DataMember] private logConcurrentBag<int> teamsInTheQueue = new logConcurrentBag<int>();

    public List<string> GetClassParameters()
    {
        return new List<string> { teamsInTheQueue.GetLoggingClassParameters() };
    }

    public Response AddTeamFromPlayerIdToTheQueue(
        InterfaceLeague _interfaceLeague, ulong _playerId, InterfaceMessage _interfaceMessage)
    {
        Team playerTeam;
        try
        {
            playerTeam =
                _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return new Response(ex.Message, false);
        }

        Log.WriteLine("Team found: " + playerTeam.GetTeamName(_interfaceLeague.LeaguePlayerCountPerTeam) +
            " (" + playerTeam.TeamId + ")" + " adding it to the challenge queue.", LogLevel.VERBOSE);

        // Add to method
        foreach (InterfaceLeague league in Database.Instance.Leagues.StoredLeagues)
        {
            Team teamToSearchFor;

            var challengeStatusOfTheTempLeague = league.LeagueData.ChallengeStatus;

            Log.WriteLine("Loop on " + nameof(league) + ": " + league.LeagueCategoryName +
                " with cache: " + _interfaceLeague.LeagueCategoryName, LogLevel.VERBOSE);
            if (league.LeagueCategoryName == _interfaceLeague.LeagueCategoryName)
            {
                Log.WriteLine("on " + league.LeagueCategoryName + ", skipping", LogLevel.VERBOSE);
                continue;
            }

            Log.WriteLine("Searching: " + league.LeagueCategoryName, LogLevel.VERBOSE);

            if (!league.LeagueData.CheckIfPlayerIsParcipiatingInTheLeague(_playerId))
            {
                Log.WriteLine(_playerId + " is not parcipiating in this league: " +
                    _interfaceLeague.LeagueCategoryName + ", disregarding", LogLevel.VERBOSE);
                continue;
            }

            Log.WriteLine(_playerId + " is parcipiating in this league: " + league.LeagueCategoryName, LogLevel.VERBOSE);

            try
            {
                teamToSearchFor = league.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                continue;
                //return new Response(ex.Message, false);
            }

            if (challengeStatusOfTheTempLeague.CheckIfPlayerTeamIsAlreadyInQueue(teamToSearchFor))
            {
                Log.WriteLine(_playerId + " already at queue", LogLevel.VERBOSE);
                // Add link to the channel
                return new Response("You are already in the queue at another league: " + league.LeagueCategoryName, false);
            }

            Log.WriteLine(_playerId + " not in the queue name: " + league.LeagueCategoryName, LogLevel.VERBOSE);
        }

        string response = PostChallengeToThisLeague(_interfaceLeague, playerTeam);
        if (response == "alreadyInQueue")
        {
            Log.WriteLine(_playerId + " was already in the queue!", LogLevel.VERBOSE);
            return new Response("You are already in the queue!", false);
        }
        Log.WriteLine("response was: " + response, LogLevel.VERBOSE);

        _interfaceMessage.GenerateAndModifyTheMessage();

        return new Response(response, true);
    }

    public void RemoveFromTeamsInTheQueue(Team _Team)
    {
        Log.WriteLine("Removing Team: " + _Team + " (" +
            _Team.TeamId + ") from the queue", LogLevel.VERBOSE);

        foreach (int team in TeamsInTheQueue.Where(t => t == _Team.TeamId))
        {
            TeamsInTheQueue.TryTake(out int _removedTeamInt);
            Log.WriteLine("Removed team: " + team, LogLevel.DEBUG);
        }

        Log.WriteLine("Done removing the team from the queue. Count is now: " +
            TeamsInTheQueue.Count, LogLevel.VERBOSE);
    }

    // Returns the teams in the queue as a string
    // (useful for printing, in log on the challenge channel)
    public string ReturnTeamsInTheQueueOfAChallenge(InterfaceLeague _interfaceLeague)
    {
        string teamsString = string.Empty;
        foreach (int teamInt in TeamsInTheQueue)
        {
            try
            {
                Team team = _interfaceLeague.LeagueData.Teams.FindTeamById(_interfaceLeague.LeaguePlayerCountPerTeam, teamInt);
                teamsString += team.GetTeamInAString(true, _interfaceLeague.LeaguePlayerCountPerTeam);
                teamsString += "\n";
            }
            catch(Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                continue;
            }
        }
        return teamsString;
    }

    // Remove the _leaguePlayerCountPerTeam param, might be useless
    public string PostChallengeToThisLeague(InterfaceLeague _interfaceLeague, Team _playerTeam)
    {
        if (CheckIfPlayerTeamIsAlreadyInQueue(_playerTeam))
        {
            Log.WriteLine("Team " + _playerTeam.GetTeamName(_interfaceLeague.LeaguePlayerCountPerTeam) +
                " (" + _playerTeam.TeamId + ")" + " was already in queue!", LogLevel.DEBUG);
            return "alreadyInQueue";
        }

        Log.WriteLine("Adding Team: " + _playerTeam + " (" +
            _playerTeam.TeamId + ") to the queue", LogLevel.VERBOSE);
        TeamsInTheQueue.Add(_playerTeam.TeamId);
        Log.WriteLine("Done adding the team to the queue. Count is now: " +
            TeamsInTheQueue.Count, LogLevel.VERBOSE);

        CheckChallengeStatus(_interfaceLeague);

        string teamsInTheQueue = ReturnTeamsInTheQueueOfAChallenge(_interfaceLeague);

        Log.WriteLine("Teams in the queue: " + teamsInTheQueue, LogLevel.VERBOSE);

        return teamsInTheQueue;
    }

    public string RemoveChallengeFromThisLeague(
        ulong _playerId, int _leaguePlayerCountPerTeam, InterfaceLeague _interfaceLeague)
    {
        Team team;
        try
        {
            team =
                _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return ex.Message;
        }            

        Log.WriteLine("Team found: " + team.GetTeamName(_leaguePlayerCountPerTeam) +
            " (" + team.TeamId + ")" + " adding it to the challenge queue: " +
            TeamsInTheQueue, LogLevel.VERBOSE);

        if (!CheckIfPlayerTeamIsAlreadyInQueue(team))
        {
            Log.WriteLine(team.TeamId + " not in queue", LogLevel.VERBOSE);
            return "notInTheQueue";
        }

        RemoveFromTeamsInTheQueue(team);

        string teamsInTheQueue =
            ReturnTeamsInTheQueueOfAChallenge(_interfaceLeague);

        Log.WriteLine("Teams in the queue: " + teamsInTheQueue, LogLevel.VERBOSE);

        return teamsInTheQueue;
    }

    public bool CheckIfPlayerTeamIsAlreadyInQueue(Team _playerTeam)
    {
        int teamId = _playerTeam.TeamId;

        Log.WriteLine("Checking if " + teamId + " was already in queue.", LogLevel.VERBOSE);

        foreach (var item in TeamsInTheQueue)
        {
            Log.WriteLine("team in queue: " + item, LogLevel.VERBOSE);
        }

        if (TeamsInTheQueue.Any(x => x == teamId))
        {
            Log.WriteLine("TeamId: " + teamId + " was already in queue!", LogLevel.DEBUG);
            return true;
        }

        Log.WriteLine("TeamId: " + teamId + " was not already in queue!", LogLevel.VERBOSE);

        return false;
    }

    public async void CheckChallengeStatus(InterfaceLeague _interfaceLeague)
    {
        int[] teamsToFormMatchOn = new int[2];

        Log.WriteLine("Checking challenge status with team amount: " +
            TeamsInTheQueue.Count, LogLevel.VERBOSE);

        // Replace this with some method later on that calculates ELO between the teams in the queue
        if (TeamsInTheQueue.Count < 2)
        {
            Log.WriteLine(nameof(TeamsInTheQueue) + " count: " + TeamsInTheQueue.Count +
                " is smaller than 2, returning.", LogLevel.DEBUG);
            return;
        }

        Log.WriteLine(nameof(TeamsInTheQueue) + " count: " + TeamsInTheQueue.Count +
            ", match found!", LogLevel.DEBUG);

        for (int t = 0; t < 2; t++)
        {
            Log.WriteLine("Looping on team index: " + t, LogLevel.VERBOSE);
            teamsToFormMatchOn[t] = TeamsInTheQueue.FirstOrDefault();
            Log.WriteLine("Done adding to " + nameof(teamsToFormMatchOn) +
                ", Length: " + teamsToFormMatchOn.Length, LogLevel.VERBOSE);
            TeamsInTheQueue = new ConcurrentBag<int>(TeamsInTheQueue.Except(new[] { teamsToFormMatchOn[t] }));
            Log.WriteLine("Done removing from " + nameof(TeamsInTheQueue) +
                ", count: " + TeamsInTheQueue.Count, LogLevel.VERBOSE);
        }

        Log.WriteLine("Done looping.", LogLevel.VERBOSE);

        await _interfaceLeague.LeagueData.Matches.CreateAMatch(_interfaceLeague, teamsToFormMatchOn);
    }
}