using System.Runtime.Serialization;

[DataContract]
public class LeagueMatch
{
    public int[] TeamsInTheMatch
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamsInTheMatch) + " with Length of: " +
                teamsInTheMatch.Length, LogLevel.VERBOSE);
            return teamsInTheMatch;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamsInTheMatch) + teamsInTheMatch
                + " to: " + value, LogLevel.VERBOSE);
            teamsInTheMatch = value;
        }
    }

    public int MatchId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(matchId)
                + ": " + matchId, LogLevel.VERBOSE);
            return matchId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(matchId) +
                matchId + " to: " + value, LogLevel.VERBOSE);
            matchId = value;
        }
    }

    public ulong MatchChannelId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(matchChannelId)
                + ": " + matchChannelId, LogLevel.VERBOSE);
            return matchChannelId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(matchChannelId) +
                matchChannelId + " to: " + value, LogLevel.VERBOSE);
            matchChannelId = value;
        }
    }


    public MatchReporting MatchReporting
    {
        get
        {
            Log.WriteLine("Getting " + nameof(matchReporting)
                + ": " + matchReporting, LogLevel.VERBOSE);
            return matchReporting;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(matchReporting) +
                matchReporting + " to: " + value, LogLevel.VERBOSE);
            matchReporting = value;
        }
    }

    [DataMember] private int[] teamsInTheMatch { get; set; }
    [DataMember] private int matchId { get; set; }
    [DataMember] private ulong matchChannelId { get; set; }
    [DataMember] private MatchReporting matchReporting { get; set; }

    public LeagueMatch()
    {
        teamsInTheMatch = new int[0];
        matchReporting = new MatchReporting();
    }

    public LeagueMatch(int[] _teamsToFormMatchOn)
    {
        teamsInTheMatch = _teamsToFormMatchOn;

        matchId = Database.Instance.Leagues.LeaguesMatchCounter;
        Database.Instance.Leagues.LeaguesMatchCounter++;

        matchReporting = new MatchReporting();   

        Log.WriteLine("Constructed a new match with teams ids: " + teamsInTheMatch[0] +
            teamsInTheMatch[1] + " with matchId of: " + matchId, LogLevel.DEBUG);
    }
    
    public ulong[] GetIdsOfThePlayersInTheMatchAsArray(InterfaceLeague _interfaceLeague)
    {
        int playerCounter = 0;

        // Calculate how many users need to be granted roles
        int userAmountToGrantRolesTo = _interfaceLeague.LeaguePlayerCountPerTeam * 2;
        ulong[] allowedUserIds = new ulong[userAmountToGrantRolesTo];

        Log.WriteLine(nameof(allowedUserIds) + " length: " +
            allowedUserIds.Length, LogLevel.VERBOSE);

        foreach (int teamId in TeamsInTheMatch)
        {
            Log.WriteLine("Looping on team id: " + teamId, LogLevel.VERBOSE);
            Team foundTeam = _interfaceLeague.LeagueData.Teams.FindTeamById(
                _interfaceLeague.LeaguePlayerCountPerTeam, teamId);

            if (foundTeam == null)
            {
                Log.WriteLine(nameof(foundTeam) + " was null!", LogLevel.ERROR);
                continue;
            }

            foreach (Player player in foundTeam.Players)
            {
                allowedUserIds[playerCounter] = player.PlayerDiscordId;
                Log.WriteLine("Added " + allowedUserIds[playerCounter] + " to: " +
                    nameof(allowedUserIds) + ". " + nameof(playerCounter) + " is now: " +
                    playerCounter+1 + " out of: " + (allowedUserIds.Length - 1).ToString(), LogLevel.VERBOSE);

                playerCounter++;
            }
        }

        return allowedUserIds;
    }
}