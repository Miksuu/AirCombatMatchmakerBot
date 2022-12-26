using Discord;
using Discord.WebSocket;
using System;

public static class LeagueManager
{
    public static ulong leagueRegistrationChannelId;

    public static ILeague GetCategoryInstance(CategoryName _leagueCategoryName)
    {
        return (ILeague)EnumExtensions.GetInstance(_leagueCategoryName.ToString());
    }

    public static Task CreateLeaguesOnStartupIfNecessary()
    {
        Log.WriteLine("Starting to create leagues for the discord server", LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();
        Log.WriteLine("guild valid", LogLevel.VERBOSE);
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return Task.CompletedTask;
        }

        // Get all of the league category names and loop through them to create the database entries
        var categoryEnumValues = Enum.GetValues(typeof(CategoryName));
        Log.WriteLine(nameof(categoryEnumValues) + " length: " + categoryEnumValues.Length, LogLevel.VERBOSE);
        foreach (CategoryName leagueCategoryName in categoryEnumValues)
        {
            Log.WriteLine("Looping on category name: " + leagueCategoryName.ToString(), LogLevel.DEBUG);
            // Check here too if a category is missing channelNames
            //bool categoryExists = false;

            // Skip all the non-leagues
            int enumValue = (int)leagueCategoryName;
            if (enumValue < 101) continue;

            ILeague interfaceLeagueCategory = GetCategoryInstance(leagueCategoryName);
            Log.WriteLine("after setting interface", LogLevel.VERBOSE);
            if (interfaceLeagueCategory == null)
            {
                Log.WriteLine(nameof(interfaceLeagueCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return Task.CompletedTask;
            }

            // Add the new newly from the interface implementations added units here
            if (Database.Instance.Leagues.CheckIfILeagueExistsByCategoryName(
                interfaceLeagueCategory.LeagueCategoryName))
            {
                Log.WriteLine(nameof(Database.Instance.Leagues) +
                    " already contains: " + interfaceLeagueCategory.ToString() +
                    " adding new units to the league", LogLevel.VERBOSE);

                // Update the units and to the database (before interfaceLeagueCategory is replaced by it)
                Database.Instance.Leagues.GetILeagueByCategoryName(
                    interfaceLeagueCategory.LeagueCategoryName).
                        LeagueUnits = interfaceLeagueCategory.LeagueUnits;

                continue;
            }

            string? leagueCategoryNameString = EnumExtensions.GetEnumMemberAttrValue(leagueCategoryName);

            // Get the role and create it if it already doesn't exist
            SocketRole role = RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
                guild, leagueCategoryNameString).Result;

            Log.WriteLine("Role is named: " + role.Name + " with ID: " + role.Id, LogLevel.VERBOSE);

            interfaceLeagueCategory.DiscordLeagueReferences.leagueRoleId = role.Id;

            Database.Instance.Leagues.AddToStoredLeagues(interfaceLeagueCategory);
        }

        return Task.CompletedTask;
    }

    public static ILeague? GetInterfaceLeagueCategoryFromTheDatabase(ILeague _leagueInterface)
    {
        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Checking if " + _leagueInterface.LeagueCategoryName +
            " has _leagueInterface in the database", LogLevel.VERBOSE);

        if (Database.Instance.Leagues.CheckIfILeagueExistsByCategoryName(_leagueInterface.LeagueCategoryName))
        {
            Log.WriteLine(_leagueInterface.LeagueCategoryName +
                " exists in the database!", LogLevel.DEBUG);

            var newInterfaceLeagueCategory =
                Database.Instance.Leagues.GetILeagueByCategoryName(_leagueInterface.LeagueCategoryName);

            if (newInterfaceLeagueCategory == null)
            {
                Log.WriteLine(nameof(newInterfaceLeagueCategory) + " was null!", LogLevel.CRITICAL);
                return null;
            }

            Log.WriteLine("found result: " + newInterfaceLeagueCategory.LeagueCategoryName, LogLevel.DEBUG);
            return newInterfaceLeagueCategory;
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

    public static ILeague GetLeagueInstanceWithLeagueCategoryName(CategoryName _leagueCategoryName)
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