using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.Serialization;

[DataContract]
public class Team
{
    public float SkillRating
    {
        get => skillRating.GetValue();
        set => skillRating.SetValue(value);
    }

    public string TeamName
    {
        get => teamName.GetValue();
        set => teamName.SetValue(value);
    }

    public ConcurrentBag<Player> Players
    {
        get
        {
            Log.WriteLine("Getting " + nameof(players) + " with count of: " +
                players.Count, LogLevel.VERBOSE);
            return players;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(players) + players
                + " to: " + value, LogLevel.VERBOSE);
            players = value;
        }
    }

    public bool TeamActive
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamActive), LogLevel.VERBOSE);
            return teamActive;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamActive)
                + " to: " + value, LogLevel.VERBOSE);
            teamActive = value;
        }
    }

    public int TeamId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamId), LogLevel.VERBOSE);
            return teamId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamId)
                + " to: " + value, LogLevel.VERBOSE);
            teamId = value;
        }
    }

    [DataMember] private logFloat skillRating = new logFloat();
    [DataMember] private logString teamName = new logString();
    [DataMember] private ConcurrentBag<Player> players { get; set; }
    [DataMember] private bool teamActive { get; set; }
    [DataMember] private int teamId { get; set; }

    public Team()
    {
        SkillRating = 1600f;
        TeamName = string.Empty;
        players = new ConcurrentBag<Player>();
        teamActive = false;
    }

    public Team(ConcurrentBag<Player> _players, string _TeamName, int _teamId)
    {
        SkillRating = 1600f;
        TeamName = _TeamName;
        players = _players;
        teamId = _teamId;
    }

    public string GetTeamMembersInAString()
    {
        string playersInATeam = string.Empty;
        for (int p = 0; p < players.Count; p++)
        {
            playersInATeam += players.ElementAt(p).GetPlayerIdAsMention();
            if (p != players.Count - 1) playersInATeam += ", ";
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
            Player? player = players.FirstOrDefault();
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
        contains = players.Any(x => x.PlayerDiscordId == _playerId);
        Log.WriteLine(_playerId + " contains: " + contains, LogLevel.VERBOSE);
        return contains;
    }

    public Team? CheckIfTeamIsActiveAndContainsAPlayer(ulong _playerId)
    {
        bool teamContainsPlayer = CheckIfATeamContainsAPlayerById(_playerId);

        Log.WriteLine("Starting to loop through team: " + TeamName + " (" + TeamId +
            ") with player count of: " + Players.Count +
            " with player id: " + _playerId, LogLevel.VERBOSE);

        if (TeamActive && teamContainsPlayer)
        {
            Log.WriteLine("Found: " + _playerId + " in team: " + TeamName +
                " (" + TeamId + ")" + " that has SR of: " + SkillRating, LogLevel.DEBUG);
            return this;
        }

        return null;
    }
}