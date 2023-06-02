using System.Collections.Concurrent;
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

    public string GetTeamName(int _leagueTeamSize, bool _getAsMention = false)
    {
        Log.WriteLine("Getting team name: " + TeamName +
            " with mention: " + _getAsMention, LogLevel.VERBOSE);

        if (_leagueTeamSize < 2 && _getAsMention)
        {
            Player? player = Players.FirstOrDefault();
            if (player == null)
            {
                Log.WriteLine(nameof(player) + " was null!", LogLevel.CRITICAL);
                return "";
            }

            Log.WriteLine("Found player: " + player.PlayerNickName +
                " (" + player.PlayerDiscordId + ")", LogLevel.VERBOSE);

            return player.GetPlayerIdAsMention();
        }

        return TeamName;
    }

    public string GetTeamInAString(
        bool _includeSkillRating, int _leagueTeamSize)
    {
        string result = string.Empty;

        if (_includeSkillRating) result += "[" + SkillRating + "] ";

        result += GetTeamName(_leagueTeamSize, true);
        if (_leagueTeamSize > 1)
        {
            result += " (" + GetTeamMembersInAString() + ")";
        }

        Log.WriteLine("Final result of a team's string with skillrating: " +
            result, LogLevel.VERBOSE);

        return result;
    }

    public bool CheckIfATeamContainsAPlayerById(ulong _playerId)
    {
        bool contains = false;
        Log.WriteLine("Checking if " + _playerId + " if is in the team.", LogLevel.VERBOSE);
        contains = Players.Any(x => x.PlayerDiscordId == _playerId);
        Log.WriteLine(_playerId + " contains: " + contains, LogLevel.VERBOSE);
        return contains;
    }

    public (Team?, bool) CheckIfTeamIsActiveAndContainsAPlayer(ulong _playerId)
    {
        bool teamContainsPlayer = CheckIfATeamContainsAPlayerById(_playerId);

        Log.WriteLine("Starting to loop through team: " + TeamName + " (" + TeamId +
            ") with player count of: " + Players.Count +
            " with player id: " + _playerId, LogLevel.VERBOSE);

        if (TeamActive && teamContainsPlayer)
        {
            Log.WriteLine("Found: " + _playerId + " in team: " + TeamName +
                " (" + TeamId + ")" + " that has SR of: " + SkillRating, LogLevel.DEBUG);
            return (this, true);
        }

        Log.WriteLine("Team was not found!", LogLevel.VERBOSE);
        return (null, false);
        //throw new InvalidOperationException("Team was not found!");
    }
}