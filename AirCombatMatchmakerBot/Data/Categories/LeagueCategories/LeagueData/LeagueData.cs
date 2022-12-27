using Discord;
using System.Runtime.Serialization;

[DataContract]
public class LeagueData
{
    [DataMember] public Teams Teams { get; set; }
    [DataMember] public ChallengeStatus ChallengeStatus { get; set; }
    [DataMember] private bool matchmakerActive { get; set; }
    public LeagueData()
    {
        Teams = new();
        ChallengeStatus = new();
    }

    public Team? FindActiveTeamByPlayerIdInAPredefinedLeague(ulong _playerId)
    {
        Log.WriteLine("Starting to find a active team by player id: " + _playerId, LogLevel.VERBOSE);

        foreach (Team team in Teams.TeamsList)
        {
            Team? foundTeam = team.CheckIfTeamIsActiveAndContainsAPlayer(_playerId);

            if (foundTeam != null)
            {
                return foundTeam;
            }
        }

        Log.WriteLine("Team not found! Admin trying to access challenge" +
            " of a league that he's not registered to?", LogLevel.WARNING);

        return null;
    }

    public void PostChallengeToThisLeague(ulong _playerId, int _leaguePlayerCountPerTeam)
    {
        Team? team = FindActiveTeamByPlayerIdInAPredefinedLeague(_playerId);

        if (team == null)
        {
            Log.WriteLine(nameof(team) +
                " was null! Could not find the team.", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Team found: " + team.GetTeamName() + " (" + team.GetTeamId() + ")" +
            " adding it to the challenge queue: " +
            ChallengeStatus.TeamsInTheQueue,
            LogLevel.VERBOSE);

        ChallengeStatus.AddToTeamsInTheQueue(team);

        Log.WriteLine(ChallengeStatus.ReturnTeamsInTheQueueOfAChallenge(
            _leaguePlayerCountPerTeam), LogLevel.VERBOSE);
    }
}