﻿using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Serialization;

[DataContract]
public class Team
{
    public int TeamId
    {
        get => teamId.GetValue();
        set => teamId.SetValue(value);
    }

    public string TeamName
    {
        get => teamName.GetValue();
        set => teamName.SetValue(value);
    }

    public ConcurrentBag<Player> Players
    {
        get => players.GetValue();
        set => players.SetValue(value);
    }

    public float SkillRating
    {
        get => skillRating.GetValue();
        set => skillRating.SetValue(value);
    }

    public bool TeamActive
    {
        get => teamActive.GetValue();
        set => teamActive.SetValue(value);
    }

    [DataMember] private logVar<int> teamId = new logVar<int>();
    [DataMember] private logString teamName = new logString();
    [DataMember] private logConcurrentBag<Player> players = new logConcurrentBag<Player>();    
    [DataMember] private logVar<float> skillRating = new logVar<float>(1600f);
    [DataMember] private logVar<bool> teamActive = new logVar<bool>();
    [DataMember] public Stats TeamStats = new Stats();

    public Team(){ }

    public Team(ConcurrentBag<Player> _players, string _TeamName, int _teamId)
    {
        TeamName = _TeamName;
        Players = _players;
        TeamId = _teamId;
    }

    public string GetTeamMembersInAString()
    {
        string playersInATeam = string.Empty;
        for (int p = 0; p < Players.Count; p++)
        {
            playersInATeam += players.ElementAt(p).GetPlayerIdAsMention();
            if (p != Players.Count - 1) playersInATeam += ", ";
        }

        Log.WriteLine("Players in the team: " + playersInATeam, LogLevel.DEBUG);

        return playersInATeam;
    }

    public string GetTeamName(bool _getAsMention = false)
    {
        Log.WriteLine("Getting team name: " + TeamName +
            " with mention: " + _getAsMention);

        if (_getAsMention)
        {
            Player? player = Players.FirstOrDefault();
            if (player == null)
            {
                Log.WriteLine(nameof(player) + " was null!", LogLevel.ERROR);
                return "";
            }

            Log.WriteLine("Found player: " + player.PlayerNickName +
                " (" + player.PlayerDiscordId + ")");

            return player.GetPlayerIdAsMention();
        }

        return TeamName;
    }

    public string GetTeamInAString(
        bool _includeSkillRating, int _leagueTeamSize)
    {
        string result = string.Empty;

        if (_includeSkillRating) result += "[" + SkillRating + "] ";

        result += GetTeamName(true);
        if (_leagueTeamSize > 1)
        {
            result += " (" + GetTeamMembersInAString() + ")";
        }

        Log.WriteLine("Final result of a team's string with skillrating: " +
            result);

        return result;
    }

    public bool CheckIfATeamContainsAPlayerById(ulong _playerId)
    {
        bool contains = false;
        Log.WriteLine("Checking if " + _playerId + " if is in the team.");
        contains = Players.Any(x => x.PlayerDiscordId == _playerId);
        Log.WriteLine(_playerId + " contains: " + contains);
        return contains;
    }

    public (Team?, bool) CheckIfTeamIsActiveAndContainsAPlayer(ulong _playerId)
    {
        bool teamContainsPlayer = CheckIfATeamContainsAPlayerById(_playerId);

        Log.WriteLine("Starting to loop through team: " + TeamName + " (" + TeamId +
            ") with player count of: " + Players.Count +
            " with player id: " + _playerId);

        if (TeamActive && teamContainsPlayer)
        {
            Log.WriteLine("Found: " + _playerId + " in team: " + TeamName +
                " (" + TeamId + ")" + " that has SR of: " + SkillRating, LogLevel.DEBUG);
            return (this, true);
        }

        Log.WriteLine("Team was not found!");
        return (null, false);
        //throw new InvalidOperationException("Team was not found!");
    }

    public void CalculateTeamStatsAfterAMatch(ReportData _thisTeamReportData, ReportData _opponentTeamReportData)
    {
        TeamStats.CalculateStatsAfterAMatch(_thisTeamReportData, _opponentTeamReportData);
    }

    public string GetTeamStats()
    {
        Log.WriteLine("Getting team: " + teamName + " with id: " + teamId + " stats.");
        return TeamStats.CalculateAndReturnTotalStatValuesAsString();
    }

    // Add 2v2, 3v3 etc functionality here
    public async Task<string> GetMatchesThatAreCloseToTeamsMembers(InterfaceLeague _interfaceLeague, ulong _timeOffset = 0)
    {
        string generatedJumpUrlsWithTime = string.Empty;

        var listOfPlayers = Players.ToList();

        List<ulong> listOfPlayersInUlong = new List<ulong>();
        foreach (var player in listOfPlayers)
        {
            listOfPlayersInUlong.Add(player.PlayerDiscordId);
        }

        var listOfLeagueMatches = Database.GetInstance<ApplicationDatabase>().Leagues.CheckAndReturnTheListOfMatchesThatListPlayersAreIn(
            listOfPlayersInUlong, TimeService.GetCurrentUnixTime(), _interfaceLeague);

        var listOfMatchesClose = Database.GetInstance<ApplicationDatabase>().Leagues.GetListOfMatchesClose(listOfLeagueMatches, _timeOffset);

        foreach (LeagueMatch leagueMatch in listOfMatchesClose)
        {
            var timeLeft = TimeService.ReturnTimeLeftAsStringFromTheTimeTheActionWillTakePlaceWithTimeLeft(
                leagueMatch.MatchEventManager.GetTimeUntilEventOfType(typeof(MatchQueueAcceptEvent)));

            generatedJumpUrlsWithTime += " [" + timeLeft + " until " + await Database.GetInstance<DiscordBotDatabase>().Categories.GetMessageJumpUrl(
                    _interfaceLeague.LeagueCategoryId, leagueMatch.MatchChannelId,
                    MessageName.MATCHSTARTMESSAGE) + "]";
        }

        return generatedJumpUrlsWithTime;
    }
}