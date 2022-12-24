using Discord;
using Discord.WebSocket;
using System;

public static class LeagueManager
{
    public static ulong leagueRegistrationChannelId;

    public static ILeague? GetInterfaceLeagueCategoryFromTheDatabase(ILeague _leagueInterface)
    {
        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Checking if " + _leagueInterface.LeagueCategoryName +
            " has _leagueInterface in the database", LogLevel.VERBOSE);

        if (CheckIfALeagueCategoryNameExistsInDatabase(_leagueInterface.LeagueCategoryName))
        {
            Log.WriteLine(_leagueInterface.LeagueCategoryName +
                " exists in the database!", LogLevel.DEBUG);

            var newInterfaceLeagueCategory =
                Database.Instance.StoredLeagueCategoriesWithChannels.First(
                    l => l.Value.LeagueCategoryName == _leagueInterface.LeagueCategoryName);

            if (newInterfaceLeagueCategory.Value == null)
            {
                Log.WriteLine(nameof(newInterfaceLeagueCategory.Value) + " was null!", LogLevel.CRITICAL);
                return null;
            }

            Log.WriteLine("found result: " + newInterfaceLeagueCategory.Value.LeagueCategoryName, LogLevel.DEBUG);
            return newInterfaceLeagueCategory.Value;
        }
        else
        {
            Log.WriteLine(_leagueInterface.LeagueCategoryName + " does not exist in the database," +
                " creating a new LeagueData for it", LogLevel.DEBUG);

            _leagueInterface.LeagueData = new LeagueData();
            _leagueInterface.DiscordLeagueReferences = new DiscordLeagueReferences();

            return _leagueInterface;
        }
    }

    private static bool CheckIfALeagueCategoryNameExistsInDatabase(LeagueCategoryName _leagueName)
    {
        return Database.Instance.StoredLeagueCategoriesWithChannels.Values.Any(l => l.LeagueCategoryName == _leagueName);
    }

    public static ILeague GetLeagueInstanceWithLeagueCategoryName(LeagueCategoryName _leagueCategoryName)
    {
        Log.WriteLine("Getting a league instance with LeagueCategoryName: " + _leagueCategoryName, LogLevel.VERBOSE);

        var leagueInstance = (ILeague)EnumExtensions.GetInstance(_leagueCategoryName.ToString());
        leagueInstance.LeagueCategoryName = _leagueCategoryName;
        Log.WriteLine(nameof(leagueInstance) + ": " + leagueInstance.ToString(),LogLevel.VERBOSE);
        return leagueInstance;
    }

    public static ILeague FindLeagueAndReturnInterfaceFromDatabase(ILeague _interfaceToSearchFor)
    {
        var dbLeagueInstance = GetInterfaceLeagueCategoryFromTheDatabase(_interfaceToSearchFor);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) + " was null! Could not find the league.", LogLevel.CRITICAL);
            return _interfaceToSearchFor;
        }

        Log.WriteLine("Found " + nameof(dbLeagueInstance) + ": " + dbLeagueInstance.LeagueCategoryName, LogLevel.DEBUG);

        return dbLeagueInstance;
    }

    public static bool CheckIfPlayerIsAlreadyInATeamById(List<Team> _leagueTeams, ulong _idToSearchFor)
    {
        foreach (Team team in _leagueTeams)
        {
            Log.WriteLine("Searching team: " + team.teamName +
                " with " + team.players.Count, LogLevel.VERBOSE);

            foreach (Player teamPlayer in team.players)
            {
                Log.WriteLine("Checking player: " + teamPlayer.playerNickName +
                    " (" + teamPlayer.playerDiscordId + ")", LogLevel.VERBOSE);

                if (teamPlayer.playerDiscordId == _idToSearchFor)
                {
                    return true;
                }
            }
        }

        Log.WriteLine("Did not find any teams that the player was in the league", LogLevel.VERBOSE);

        return false;
    }

    // Always run CheckIfPlayerIsAlreadyInATeamById() before!
    public static Team ReturnTeamThatThePlayerIsIn(List<Team> _leagueTeams, ulong _idToSearchFor)
    {
        foreach (Team team in _leagueTeams)
        {
            Log.WriteLine("Searching team: " + team.teamName +
                " with " + team.players.Count, LogLevel.VERBOSE);

            foreach (Player teamPlayer in team.players)
            {
                Log.WriteLine("Checking player: " + teamPlayer.playerNickName +
                    " (" + teamPlayer.playerDiscordId + ")", LogLevel.VERBOSE);

                if (teamPlayer.playerDiscordId == _idToSearchFor)
                {
                    return team;
                }
            }
        }

        Log.WriteLine("Did not find any teams that the player was in the league", LogLevel.CRITICAL);

        return new Team();
    }
}