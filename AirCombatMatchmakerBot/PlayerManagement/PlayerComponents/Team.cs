using Discord;
using System.Runtime.Serialization;

[DataContract]
public class Team
{
    [DataMember] private float skillRating { get; set; }
    [DataMember] private string teamName { get; set; }
    [DataMember] private List<Player> Players { get; set; }
    [DataMember] private bool teamActive { get; set; }
    [DataMember] private int teamId { get; set; }

    public Team()
    {
        skillRating = 1600;
        teamName = string.Empty;
        Players = new List<Player>();
        teamActive = false;
    }

    public Team(List<Player> _players, string _teamName, int _teamId)
    {
        skillRating = 1600;
        teamName = _teamName;
        Players = _players;
        teamId = _teamId;
    }

    public float GetTeamSkillRating()
    {
        Log.WriteLine("Getting team's: " + teamName + " skill rating: " +
            skillRating, LogLevel.VERBOSE);
        return skillRating;
    }

    public string GetTeamMembersInAString()
    {
        string playersInATeam = string.Empty;
        for (int p = 0; p < Players.Count; p++)
        {
            playersInATeam += Players[p].GetPlayerIdAsMention();
            if (p !=  Players.Count - 1) playersInATeam += ", ";
        }
        return playersInATeam;
    }

    public List<Player> GetListOfPlayersInATeam()
    {
        Log.WriteLine("Getting list of Players with count of: " +
            Players.Count, LogLevel.VERBOSE);
        return Players;
    }

    public string GetTeamName(int _leagueTeamSize, bool _getAsMention = false)
    {
        Log.WriteLine("Getting team name: " + teamName +
            " with mention: " + _getAsMention, LogLevel.VERBOSE);

        if (_leagueTeamSize < 2 && _getAsMention)
        {
            Player player = Players.First();
            Log.WriteLine("Found player: " + player.GetPlayerNickname() +
                " (" + player.GetPlayerDiscordId() + ")", LogLevel.VERBOSE);

            return player.GetPlayerIdAsMention();
        }

        return teamName;
    }

    public bool GetIfTheTeamIsActive()
    {
        Log.WriteLine("Getting if the team: " + teamName + " is active: " +
            teamActive, LogLevel.VERBOSE);
        return teamActive;
    }

    public string GetTeamSkillRatingAndNameInAString(
        int _leagueTeamSize)
    {
        string result = string.Empty;

        result += "[" + GetTeamSkillRating() + "] " + GetTeamName(_leagueTeamSize, true);
        if (_leagueTeamSize > 1)
        {
            result += " (" + GetTeamMembersInAString() + ")";
        }

        Log.WriteLine("Final result of a team's string with skillrating: " +
            result, LogLevel.VERBOSE);

        return result;
    }

    public void SetTheActive(bool _active)
    {
        Log.WriteLine("Setting: " + teamActive + " to: " + _active, LogLevel.VERBOSE);
        teamActive = _active;    
    }

    public bool CheckIfATeamContainsAPlayerById(ulong _playerId)
    {
        bool contains = false;
        Log.WriteLine("Checking if " + _playerId + " if is in the team.", LogLevel.VERBOSE);
        contains = Players.Any(x => x.GetPlayerDiscordId() == _playerId);
        Log.WriteLine(_playerId + " contains: " + contains, LogLevel.VERBOSE);
        return contains;
    }

    public Team? CheckIfTeamIsActiveAndContainsAPlayer(ulong _playerId)
    {
        bool teamActive = GetIfTheTeamIsActive();
        bool teamContainsPlayer = CheckIfATeamContainsAPlayerById(_playerId);

        Log.WriteLine("Starting to loop through team: " + teamName + " (" + teamId +
            ") with player count of: " + Players.Count +
            " with player id: " + _playerId, LogLevel.VERBOSE);

        if (teamActive && teamContainsPlayer)
        {
            Log.WriteLine("Found: " + _playerId + " in team: " + teamName +
                " (" + teamId + ")" + " that has SR of: " + skillRating, LogLevel.DEBUG);
            return this;
        }

        return null;
    }

    public int GetTeamId()
    {
        Log.WriteLine("Getting team's: " + teamName + " teamId:" + teamId, LogLevel.VERBOSE);
        return teamId;
    }
}