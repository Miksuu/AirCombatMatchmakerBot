using Discord;
using Discord.WebSocket;
using System;

public static class LeagueManager
{
    //public static ulong leagueRegistrationChannelId;

    public static InterfaceLeague GetCategoryInstance(CategoryName _leagueCategoryName)
    {
        return (InterfaceLeague)EnumExtensions.GetInstance(_leagueCategoryName.ToString());
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
        Log.WriteLine(nameof(categoryEnumValues) +
            " length: " + categoryEnumValues.Length, LogLevel.VERBOSE);
        foreach (CategoryName leagueCategoryName in categoryEnumValues)
        {
            Log.WriteLine("Looping on category name: " + leagueCategoryName.ToString(), LogLevel.DEBUG);
            // Check here too if a category is missing channelNames
            //bool categoryExists = false;

            // Skip all the non-leagues
            int enumValue = (int)leagueCategoryName;
            if (enumValue > 100) continue;

            InterfaceLeague interfaceLeagueCategory = GetCategoryInstance(leagueCategoryName);
            Log.WriteLine("after setting interface", LogLevel.VERBOSE);
            if (interfaceLeagueCategory == null)
            {
                Log.WriteLine(nameof(interfaceLeagueCategory).ToString() +
                    " was null!", LogLevel.CRITICAL);
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

            interfaceLeagueCategory.DiscordLeagueReferences.LeagueRoleId = role.Id;

            Database.Instance.Leagues.AddToStoredLeagues(interfaceLeagueCategory);
        }

        return Task.CompletedTask;
    }

    public static InterfaceLeague GetLeagueInstanceWithLeagueCategoryName(CategoryName _leagueCategoryName)
    {
        Log.WriteLine("Getting a league instance with LeagueCategoryName: " +
            _leagueCategoryName, LogLevel.VERBOSE);

        var leagueInstance = (InterfaceLeague)EnumExtensions.GetInstance(_leagueCategoryName.ToString());
        leagueInstance.LeagueCategoryName = _leagueCategoryName;
        Log.WriteLine(nameof(leagueInstance) + ": " + leagueInstance.ToString(),LogLevel.VERBOSE);
        return leagueInstance;
    }

    /*
    public static InterfaceLeague FindLeagueAndReturnInterfaceFromDatabase(InterfaceLeague _interfaceToSearchFor)
    {
        var dbLeagueInstance = GetInterfaceLeagueCategoryFromTheDatabase(_interfaceToSearchFor);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return _interfaceToSearchFor;
        }

        Log.WriteLine("Found " + nameof(dbLeagueInstance) +
            ": " + dbLeagueInstance.LeagueCategoryName, LogLevel.DEBUG);

        return dbLeagueInstance;
    }*/
}