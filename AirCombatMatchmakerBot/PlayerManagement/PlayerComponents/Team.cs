using Discord;
using System.Runtime.Serialization;

[DataContract]
public class Team
{
    public float SkillRating
    {
        get
        {
            Log.WriteLine("Getting " + nameof(skillRating), LogLevel.VERBOSE);
            return skillRating;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(skillRating)
                + " to: " + value, LogLevel.VERBOSE);
            skillRating = value;
        }
    }

    public string TeamName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamName), LogLevel.VERBOSE);
            return teamName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamName)
                + " to: " + value, LogLevel.VERBOSE);
            teamName = value;
        }
    }

    public List<Player> Players
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

    [DataMember] private float skillRating { get; set; }
    [DataMember] private string teamName { get; set; }
    [DataMember] private List<Player> players { get; set; }
    [DataMember] private bool teamActive { get; set; }
    [DataMember] private int teamId { get; set; }

    public Team()
    {
        skillRating = 1600;
        teamName = string.Empty;
        players = new List<Player>();
        teamActive = false;
    }

    public Team(List<Player> _players, string _teamName, int _teamId)
    {
        skillRating = 1600;
        teamName = _teamName;
        players = _players;
        teamId = _teamId;
    }

    public string GetTeamMembersInAString()
    {
        string playersInATeam = string.Empty;
        for (int p = 0; p < players.Count; p++)
        {
            playersInATeam += players[p].GetPlayerIdAsMention();
            if (p !=  players.Count - 1) playersInATeam += ", ";
        }
        return playersInATeam;
    }

    public string GetTeamName(int _leagueTeamSize, bool _getAsMention = false)
    {
        Log.WriteLine("Getting team name: " + teamName +
            " with mention: " + _getAsMention, LogLevel.VERBOSE);

        if (_leagueTeamSize < 2 && _getAsMention)
        {
            Player player = players.First();
            Log.WriteLine("Found player: " + player.PlayerNickName +
                " (" + player.PlayerDiscordId + ")", LogLevel.VERBOSE);

            return player.GetPlayerIdAsMention();
        }

        return teamName;
    }

    public string GetTeamSkillRatingAndNameInAString(
    int _leagueTeamSize)
    {
        string result = string.Empty;

        result += "[" + SkillRating + "] " + GetTeamName(_leagueTeamSize, true);
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

        /*
    public float GetTeamSkillRating()
    {
        Log.WriteLine("Getting team's: " + teamName + " skill rating: " +
            skillRating, LogLevel.VERBOSE);
        return skillRating;
    }*/

        /*
    public List<Player> GetListOfPlayersInATeam()
    {
        Log.WriteLine("Getting list of Players with count of: " +
            players.Count, LogLevel.VERBOSE);
        return players;
    }*/

    /*
    public bool GetIfTheTeamIsActive()
    {
        Log.WriteLine("Getting if the team: " + teamName + " is active: " +
            teamActive, LogLevel.VERBOSE);
        return teamActive;
    }*/


    /*
    public void SetTheActive(bool _active)
    {
        Log.WriteLine("Setting: " + teamActive + " to: " + _active, LogLevel.VERBOSE);
        teamActive = _active;    
    }*/



    /*
    public int GetTeamId()
    {
        Log.WriteLine("Getting team's: " + TeamName + " teamId:" + TeamId, LogLevel.VERBOSE);
        return TeamId;
    }*/
}