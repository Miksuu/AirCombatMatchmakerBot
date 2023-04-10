﻿using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class ChallengeStatus
{
    public ConcurrentBag<int> TeamsInTheQueue
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamsInTheQueue) +  " with count of: " +
                teamsInTheQueue.Count, LogLevel.VERBOSE);
            return teamsInTheQueue;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamsInTheQueue) + teamsInTheQueue
                + " to: " + value, LogLevel.VERBOSE);
            teamsInTheQueue = value;
        }
    }

    [DataMember] private ConcurrentBag<int> teamsInTheQueue { get; set; }

    public ChallengeStatus()
    {
        teamsInTheQueue = new ConcurrentBag<int>();
    }

    public void AddToTeamsInTheQueue(Team _Team)
    {
        Log.WriteLine("Adding Team: " + _Team + " (" + 
            _Team.TeamId + ") to the queue", LogLevel.VERBOSE);
        teamsInTheQueue.Add(_Team.TeamId);
        Log.WriteLine("Done adding the team to the queue. Count is now: " +
            teamsInTheQueue.Count, LogLevel.VERBOSE);
    }

    public void RemoveFromTeamsInTheQueue(Team _Team)
    {
        Log.WriteLine("Removing Team: " + _Team + " (" +
            _Team.TeamId + ") from the queue", LogLevel.VERBOSE);
        //teamsInTheQueue.TryTake(out int element) && !element.Equals(_Team.TeamId));

        foreach (int team in teamsInTheQueue.Where(t => t == _Team.TeamId))
        {
            teamsInTheQueue.TryTake(out int _removedTeamInt);
            Log.WriteLine("Removed team: " + team, LogLevel.DEBUG);
        }

        Log.WriteLine("Done removing the team from the queue. Count is now: " +
            teamsInTheQueue.Count, LogLevel.VERBOSE);
    }

    // Returns the teams in the queue as a string
    // (useful for printing, in log on the challenge channel)
    public string ReturnTeamsInTheQueueOfAChallenge(int _leagueTeamSize, LeagueData _leagueData)
    {
        string teamsString = string.Empty;
        foreach (int teamInt in teamsInTheQueue)
        {
            Team team = _leagueData.Teams.FindTeamById(_leagueTeamSize, teamInt);
            teamsString += team.GetTeamInAString(true, _leagueTeamSize);
            teamsString += "\n";
        }
        return teamsString;
    }

    // Remove the _leaguePlayerCountPerTeam param, might be useless
    public string PostChallengeToThisLeague(
        ulong _playerId, int _leaguePlayerCountPerTeam, InterfaceLeague _interfaceLeague)
    {
        Team? team =
            _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);

        if (team == null)
        {
            Log.WriteLine(nameof(team) +
                " was null! Could not find the team.", LogLevel.CRITICAL);
            return "Error! Team not found";
        }

        Log.WriteLine("Team found: " + team.GetTeamName(_leaguePlayerCountPerTeam) +
            " (" + team.TeamId + ")" +" adding it to the challenge queue: " +
            TeamsInTheQueue, LogLevel.VERBOSE);

        if (TeamsInTheQueue.Any(x => x == team.TeamId))
        {
            Log.WriteLine("Team " + team.GetTeamName(_leaguePlayerCountPerTeam) +
                " (" + team.TeamId + ")" + " was already in queue!", LogLevel.DEBUG);
            return "alreadyInQueue";
        }

        AddToTeamsInTheQueue(team);

        CheckChallengeStatus(_interfaceLeague);

        string teamsInTheQueue =
            ReturnTeamsInTheQueueOfAChallenge(_leaguePlayerCountPerTeam, _interfaceLeague.LeagueData);

        Log.WriteLine("Teams in the queue: " + teamsInTheQueue, LogLevel.VERBOSE);

        return teamsInTheQueue;
    }

    public string RemoveChallengeFromThisLeague(
        ulong _playerId, int _leaguePlayerCountPerTeam, InterfaceLeague _interfaceLeague)
    {
        Team? team =
            _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);

        if (team == null)
        {
            Log.WriteLine(nameof(team) +
                " was null! Could not find the team.", LogLevel.CRITICAL);
            return "Error! Team not found";
        }

        Log.WriteLine("Team found: " + team.GetTeamName(_leaguePlayerCountPerTeam) +
            " (" + team.TeamId + ")" + " adding it to the challenge queue: " +
            TeamsInTheQueue, LogLevel.VERBOSE);

        if (!TeamsInTheQueue.Any(x => x == team.TeamId))
        {
            Log.WriteLine("Team " + team.GetTeamName(_leaguePlayerCountPerTeam) +
                " (" + team.TeamId + ")" + " was already in queue!", LogLevel.DEBUG);
            return "notInTheQueue";
        }

        RemoveFromTeamsInTheQueue(team);

        //CheckChallengeStatus(_interfaceLeague);

        string teamsInTheQueue =
            ReturnTeamsInTheQueueOfAChallenge(_leaguePlayerCountPerTeam, _interfaceLeague.LeagueData);

        Log.WriteLine("Teams in the queue: " + teamsInTheQueue, LogLevel.VERBOSE);

        return teamsInTheQueue;
    }

    public async void CheckChallengeStatus(InterfaceLeague _interfaceLeague)
    {
        int[] teamsToFormMatchOn = new int[2];

        Log.WriteLine("Checking challenge status with team amount: " +
            TeamsInTheQueue.Count, LogLevel.VERBOSE);

        // Replace this with some method later on that calculates ELO between the teams in the queue
        if (TeamsInTheQueue.Count < 2)
        {
            Log.WriteLine(nameof(teamsInTheQueue) + " count: " + TeamsInTheQueue.Count +
                " is smaller than 2, returning.", LogLevel.DEBUG);
            return;
        }

        Log.WriteLine(nameof(teamsInTheQueue) + " count: " + teamsInTheQueue.Count +
            ", match found!", LogLevel.DEBUG);

        for (int t = 0; t < 2; t++)
        {
            Log.WriteLine("Looping on team index: " + t, LogLevel.VERBOSE);
            teamsToFormMatchOn[t] = teamsInTheQueue.FirstOrDefault();
            Log.WriteLine("Done adding to " + nameof(teamsToFormMatchOn) +
                ", Length: " + teamsToFormMatchOn.Length, LogLevel.VERBOSE);
            teamsInTheQueue = new ConcurrentBag<int>(teamsInTheQueue.Except(new[] { teamsToFormMatchOn[t] }));
            Log.WriteLine("Done removing from " + nameof(teamsInTheQueue) +
                ", count: " + teamsInTheQueue.Count, LogLevel.VERBOSE);
        }

        Log.WriteLine("Done looping.", LogLevel.VERBOSE);

        await _interfaceLeague.LeagueData.Matches.CreateAMatch(_interfaceLeague, teamsToFormMatchOn);

        return;
    }
}