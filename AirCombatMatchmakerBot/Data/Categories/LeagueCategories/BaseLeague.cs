using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;

[DataContract]
public abstract class BaseLeague : ILeague
{
    CategoryName ILeague.LeagueCategoryName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueCategoryName) + ": " + leagueCategoryName, LogLevel.VERBOSE);
            return leagueCategoryName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueCategoryName) + leagueCategoryName
                + " to: " + value, LogLevel.VERBOSE);
            leagueCategoryName = value;
        }
    }

    Era ILeague.LeagueEra
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueEra) + ": " + leagueEra, LogLevel.VERBOSE);
            return leagueEra;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueEra) + leagueEra
                + " to: " + value, LogLevel.VERBOSE);
            leagueEra = value;
        }
    }

    int ILeague.LeaguePlayerCountPerTeam
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leaguePlayerCountPerTeam) + ": " + leaguePlayerCountPerTeam, LogLevel.VERBOSE);
            return leaguePlayerCountPerTeam;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leaguePlayerCountPerTeam) + leaguePlayerCountPerTeam
                + " to: " + value, LogLevel.VERBOSE);
            leaguePlayerCountPerTeam = value;
        }
    }

    List<UnitName> ILeague.LeagueUnits
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueUnits) + ": " + leagueUnits, LogLevel.VERBOSE);
            return leagueUnits;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueUnits) + leagueUnits
                + " to: " + value, LogLevel.VERBOSE);
            leagueUnits = value;
        }
    }

    LeagueData ILeague.LeagueData
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueData) + ": " + leagueData, LogLevel.VERBOSE);
            return leagueData;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueData) + leagueData
                + " to: " + value, LogLevel.VERBOSE);
            leagueData = value;
        }
    }

    DiscordLeagueReferences ILeague.DiscordLeagueReferences
    {
        get
        {
            Log.WriteLine("Getting " + nameof(discordleagueReferences) + ": " + discordleagueReferences, LogLevel.VERBOSE);
            return discordleagueReferences;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(discordleagueReferences) + discordleagueReferences
                + " to: " + value, LogLevel.VERBOSE);
            discordleagueReferences = value;
        }
    }

    // Generated based on the implementation
    [DataMember] protected CategoryName leagueCategoryName;
    [DataMember] protected Era leagueEra;
    [DataMember] protected int leaguePlayerCountPerTeam;
    [DataMember] protected List<UnitName> leagueUnits = new List<UnitName>();
    [DataMember] protected LeagueData leagueData = new LeagueData();
    [DataMember] protected DiscordLeagueReferences discordleagueReferences = new DiscordLeagueReferences();

    public BaseLeague()
    {
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);

    private Team? FindActiveTeamByPlayerIdInAPredefinedLeague(ulong _playerId)
    {
        Log.WriteLine("Starting to find a active team by player id: " + _playerId +
            " in league: " + leagueCategoryName, LogLevel.VERBOSE);

        foreach (Team team in leagueData.Teams.GetListOfTeams())
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

    public void PostChallengeToThisLeague(ulong _playerId)
    {
        Team? team = FindActiveTeamByPlayerIdInAPredefinedLeague(_playerId);

        if (team == null)
        {
            Log.WriteLine(nameof(team) +
                " was null! Could not find the team.", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Team found: " + team.GetTeamName() + " (" + team.GetTeamId() + ")" +
            " adding it to the challenge queue with count: " +
            leagueData.ChallengeStatus.GetListOfTeamsInTheQueue(),
            LogLevel.VERBOSE);

        leagueData.ChallengeStatus.AddToTeamsInTheQueue(team);

        Log.WriteLine(leagueData.ChallengeStatus.ReturnTeamsInTheQueueOfAChallenge(
            leaguePlayerCountPerTeam), LogLevel.VERBOSE);
    }
}