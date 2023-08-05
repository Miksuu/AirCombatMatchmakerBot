using Discord;
using Discord.WebSocket;
using System;

public static class LeagueManager
{
    public static InterfaceLeague GetLeagueCategoryInstance(LeagueName _leagueCategoryName)
    {
        Log.WriteLine(_leagueCategoryName.ToString());
        return (InterfaceLeague)EnumExtensions.GetInstance(_leagueCategoryName.ToString());
    }

    public async static Task CreateLeaguesOnStartupIfNecessary()
    {
        // Get all of the league category names and loop through them to create the database entries
        var categoryEnumValues = Enum.GetValues(typeof(LeagueName));
        Log.WriteLine(nameof(categoryEnumValues) +
            " length: " + categoryEnumValues.Length, LogLevel.VERBOSE);
        foreach (LeagueName leagueCategoryName in categoryEnumValues)
        {
            Log.WriteLine("Looping on category name: " + leagueCategoryName.ToString(), LogLevel.DEBUG);

            InterfaceLeague interfaceLeagueCategory = GetLeagueCategoryInstance(leagueCategoryName);
            Log.WriteLine(nameof(LeagueManager) + " with: " + Database.Instance.PlayerData, LogLevel.VERBOSE);

            if (Database.Instance.Leagues.StoredLeagues.Any(x => x.LeagueCategoryName == leagueCategoryName))
            {
                Log.WriteLine(leagueCategoryName + " already exists, continuing");
                continue;
            }

            Log.WriteLine(leagueCategoryName + " does not exists, creating it", LogLevel.DEBUG);

            Database.Instance.Leagues.AddToStoredLeagues(interfaceLeagueCategory);
        }
    }

    //public static InterfaceLeague GetLeagueInstanceWithLeagueCategoryName(LeagueName _leagueCategoryType)
    //{
    //    Log.WriteLine("Getting a league instance with LeagueCategoryName: " +
    //        _leagueCategoryType, LogLevel.VERBOSE);

    //    var leagueInstance = (InterfaceLeague)EnumExtensions.GetInstance(_leagueCategoryType.ToString());
    //    leagueInstance.LeagueCategoryName = _leagueCategoryType;
    //    Log.WriteLine(nameof(leagueInstance) + ": " + leagueInstance.ToString(),LogLevel.VERBOSE);
    //    return leagueInstance;
    //}

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