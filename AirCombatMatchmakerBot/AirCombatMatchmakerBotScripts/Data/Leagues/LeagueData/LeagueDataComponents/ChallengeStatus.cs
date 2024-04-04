using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;
using System.IO;
using System.Threading.Channels;

[DataContract]
public class ChallengeStatus
{
    public ConcurrentBag<int> TeamsInTheQueue
    {
        get => teamsInTheQueue.GetValue();
        set => teamsInTheQueue.SetValue(value);
    }

    [DataMember] private logConcurrentBag<int> teamsInTheQueue = new logConcurrentBag<int>();

    public ChallengeStatus() { }

    public async Task<Response> AddTeamFromPlayerIdToTheQueue(
        ulong _playerId, InterfaceMessage _interfaceMessage, InterfaceLeague _interfaceLeague)
    {
        try
        {
            Team playerTeam = GetPlayerTeam(_playerId, _interfaceLeague);
            List<ulong> playerIdsInTheTeam = GetPlayerIdsInTheTeam(playerTeam);

            Response responseFromLeagues = CheckIfPlayersCanJoinOrSuggestATimeForTheMatch(playerIdsInTheTeam, _playerId, _interfaceLeague);
            if (!responseFromLeagues.serialize)
            {
                return new Response(responseFromLeagues.responseString, false);
            }

            Response responseFromOtherLeagues = CheckIfPlayerIsInQueueInOtherLeagues(_playerId, playerTeam);
            if (!responseFromOtherLeagues.serialize)
            {
                return responseFromOtherLeagues;
            }

            string response = PostChallengeToThisLeague(playerTeam, _interfaceLeague);
            if (response == "alreadyInQueue")
            {
                Log.WriteLine(_playerId + " was already in the queue!");
                return new Response("You are already in the queue!", false);
            }
            Log.WriteLine("response was: " + response);

            await UpdateAndModifyChallengeMessage(_interfaceMessage, _interfaceLeague);

            return new Response(response, true);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return new Response(ex.Message, false);
        }
    }

    private Team GetPlayerTeam(ulong _playerId, InterfaceLeague _interfaceLeague)
    {
        Team playerTeam = _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);
        Log.WriteLine("Team found: " + playerTeam.GetTeamName() + " (" + playerTeam.TeamId + ")" + " adding it to the challenge queue.");
        return playerTeam;
    }

    private List<ulong> GetPlayerIdsInTheTeam(Team _playerTeam)
    {
        return _playerTeam.Players.Select(player => player.PlayerDiscordId).ToList();
    }

    private Response CheckIfPlayersCanJoinOrSuggestATimeForTheMatch(
        List<ulong> _playerIdsInTheTeam, ulong _playerId, InterfaceLeague _interfaceLeague)
    {
        return Database.GetInstance<ApplicationDatabase>().Leagues.CheckIfListOfPlayersCanJoinOrSuggestATimeForTheMatchWithTime(
            _playerIdsInTheTeam, TimeService.GetCurrentUnixTime(), _playerId, _interfaceLeague).Result;
    }

    private Response CheckIfPlayerIsInQueueInOtherLeagues(ulong _playerId, Team _playerTeam)
    {
        foreach (InterfaceLeague league in Database.GetInstance<ApplicationDatabase>().Leagues.StoredLeagues)
        {
            if (IsPlayerInQueueInLeague(_playerId, _playerTeam, league))
            {
                return new Response("You are already in the queue at another league: " + league.LeagueCategoryName, false);
            }
        }

        return new Response("", true);
    }

    private bool IsPlayerInQueueInLeague(ulong _playerId, Team _playerTeam, InterfaceLeague _league)
    {
        if (_league.LeagueCategoryName == _league.LeagueCategoryName)
        {
            return false;
        }

        if (!_league.LeagueData.CheckIfPlayerIsParcipiatingInTheLeague(_playerId))
        {
            return false;
        }

        Team teamToSearchFor = _league.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);
        return _league.LeagueData.ChallengeStatus.CheckIfPlayerTeamIsAlreadyInQueue(teamToSearchFor);
    }

    private async Task UpdateAndModifyChallengeMessage(InterfaceMessage _interfaceMessage, InterfaceLeague _interfaceLeague)
    {
        CHALLENGEMESSAGE challengeMessage = _interfaceMessage as CHALLENGEMESSAGE;
        await challengeMessage.UpdateTeamsThatHaveMatchesClose(_interfaceLeague);
        challengeMessage.GenerateAndModifyTheMessageAsync();
    }

    public void RemoveFromTeamFromTheQueue(Team _Team)
    {
        Log.WriteLine("Removing Team: " + _Team + " (" +
            _Team.TeamId + ") from the queue");

        foreach (int team in TeamsInTheQueue.Where(t => t == _Team.TeamId))
        {
            TeamsInTheQueue.TryTake(out int _removedTeamInt);
            Log.WriteLine("Removed team: " + team, LogLevel.DEBUG);
        }

        Log.WriteLine("Done removing the team from the queue. Count is now: " +
            TeamsInTheQueue.Count);
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
                Team team = _interfaceLeague.LeagueData.Teams.FindTeamById(teamInt);
                teamsString += team.GetTeamInAString(true, _interfaceLeague.LeaguePlayerCountPerTeam);
                teamsString += "\n";
            }
            catch(Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.ERROR);
                continue;
            }
        }
        return teamsString;
    }

    public string PostChallengeToThisLeague(Team _playerTeam, InterfaceLeague _interfaceLeague)
    {
        if (CheckIfPlayerTeamIsAlreadyInQueue(_playerTeam))
        {
            Log.WriteLine("Team " + _playerTeam.GetTeamName() +
                " (" + _playerTeam.TeamId + ")" + " was already in queue!", LogLevel.DEBUG);
            return "alreadyInQueue";
        }

        Log.WriteLine("Adding Team: " + _playerTeam + " (" +
            _playerTeam.TeamId + ") to the queue");
        TeamsInTheQueue.Add(_playerTeam.TeamId);
        Log.WriteLine("Done adding the team to the queue. Count is now: " +
            TeamsInTheQueue.Count);

        CheckChallengeStatus(_interfaceLeague);

        string teamsInTheQueue = ReturnTeamsInTheQueueOfAChallenge(_interfaceLeague);

        Log.WriteLine("Teams in the queue: " + teamsInTheQueue);

        return "";
    }

    public string RemoveChallengeFromThisLeague(ulong _playerId, InterfaceLeague _interfaceLeague)
    {
        try
        {
            Team team =
                _interfaceLeague.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(_playerId);
            Log.WriteLine("Team found: " + team.GetTeamName() +
                " (" + team.TeamId + ")" + " adding it to the challenge queue: " + TeamsInTheQueue);

            if (!CheckIfPlayerTeamIsAlreadyInQueue(team))
            {
                Log.WriteLine(team.TeamId + " not in queue");
                return "notInTheQueue";
            }

            RemoveFromTeamFromTheQueue(team);

            string teamsInTheQueue =
                ReturnTeamsInTheQueueOfAChallenge(_interfaceLeague);

            Log.WriteLine("Teams in the queue: " + teamsInTheQueue);

            return teamsInTheQueue;
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return ex.Message;
        }
    }

    public bool CheckIfPlayerTeamIsAlreadyInQueue(Team _playerTeam)
    {
        int teamId = _playerTeam.TeamId;

        Log.WriteLine("Checking if " + teamId + " was already in queue.");

        foreach (var item in TeamsInTheQueue)
        {
            Log.WriteLine("team in queue: " + item);
        }

        if (TeamsInTheQueue.Any(x => x == teamId))
        {
            Log.WriteLine("TeamId: " + teamId + " was already in queue!", LogLevel.DEBUG);
            return true;
        }

        Log.WriteLine("TeamId: " + teamId + " was not already in queue!");

        return false;
    }

    public async void CheckChallengeStatus(InterfaceLeague _interfaceLeague)
    {
        int[] teamsToFormMatchOn = new int[2];

        Log.WriteLine("Checking challenge status with team amount: " +
            TeamsInTheQueue.Count);

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
            Log.WriteLine("Looping on team index: " + t);
            teamsToFormMatchOn[t] = TeamsInTheQueue.FirstOrDefault();
            Log.WriteLine("Done adding to " + nameof(teamsToFormMatchOn) +
                ", Length: " + teamsToFormMatchOn.Length);
            TeamsInTheQueue = new ConcurrentBag<int>(TeamsInTheQueue.Except(new[] { teamsToFormMatchOn[t] }));
            Log.WriteLine("Done removing from " + nameof(TeamsInTheQueue) +
                ", count: " + TeamsInTheQueue.Count);
        }

        Log.WriteLine("Done looping.");

        await _interfaceLeague.LeagueData.Matches.CreateAMatch(teamsToFormMatchOn, MatchState.PLAYERREADYCONFIRMATIONPHASE, _interfaceLeague);
    }
}