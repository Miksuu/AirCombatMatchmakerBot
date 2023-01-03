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

    [DataMember] private int[] teamsInTheMatch { get; set; }
    [DataMember] private int matchId { get; set; }
    [DataMember] private ulong matchChannelId { get; set; }

    public LeagueMatch() { }

    public LeagueMatch(int[] _teamsToFormMatchOn)
    {
        teamsInTheMatch = _teamsToFormMatchOn;

        matchId = Database.Instance.Leagues.LeaguesMatchCounter;
        Database.Instance.Leagues.LeaguesMatchCounter++;

        Log.WriteLine("Constructed a new match with teams ids: " + teamsInTheMatch[0] +
            teamsInTheMatch[1] + " with matchId of: " + matchId, LogLevel.DEBUG);
    }
}