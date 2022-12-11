using Discord;
using Discord.WebSocket;
using System;

public static class LeagueManager
{
    public static ulong leagueRegistrationChannelId;

    /*
    public static async Task CreateLeaguesOnStartup(SocketGuild _guild, ITextChannel _leagueRegistrationChannel)
    {
        Log.WriteLine("Starting to create leagues on the startup", LogLevel.VERBOSE);
        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        // Hardcoded channel id for now
        var channel = guild.GetTextChannel(leagueRegistrationChannelId) as ITextChannel;

        if (channel == null)
        {
            Log.WriteLine("Channel was null with id: " +
                leagueRegistrationChannelId, LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Channel found: " + channel.Name +
            "(" + channel.Id + ")", LogLevel.VERBOSE);

        foreach (LeagueCategoryName leagueName in Enum.GetValues(typeof(LeagueCategoryName)))
        {
            CreateALeague(channel, leagueName);
        }

        Log.WriteLine("Done creating leagues.", LogLevel.VERBOSE);

        //await SerializationManager.SerializeDB();
    }*/

    public static InterfaceLeagueCategory? GetInterfaceLeagueCategoryFromTheDatabase(InterfaceLeagueCategory _leagueInterface)
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
                Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.First(
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
        return Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.Values.Any(l => l.LeagueCategoryName == _leagueName);
    }

    private static bool CheckIfLeagueRegisterationMessageExists(InterfaceLeagueCategory _leagueInterface)
    {
        Log.WriteLine("Checking if _leagueInterface registration message exists", LogLevel.VERBOSE);

        if (_leagueInterface.DiscordLeagueReferences.leagueRegistrationChannelMessageId == 0)
        {
            Log.WriteLine("leagueData.leagueChannelMessageId was 0!", LogLevel.CRITICAL);
            return false;
        }

        Log.WriteLine("Found: " + _leagueInterface.DiscordLeagueReferences.leagueRegistrationChannelMessageId, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return false;
        }

        var channel = guild.GetTextChannel(1049555859656671232) as ITextChannel;

        var message = channel.GetMessageAsync(_leagueInterface.DiscordLeagueReferences.leagueRegistrationChannelMessageId);

        if (message.Result != null)
        {
            Log.WriteLine("message: " + message.Result.ToString() + " found.", LogLevel.DEBUG);
            return true;
        }
        // Returns false otherwise
        Log.WriteLine("returning false", LogLevel.DEBUG);
        return false;
    }

    public static void CreateALeague(ITextChannel _channel, LeagueCategoryName _leagueName)
    {
        Log.WriteLine("Looping on leagueName: " + _leagueName.ToString(), LogLevel.VERBOSE);

        InterfaceLeagueCategory? leagueInterface = GetLeagueInstanceWithLeagueCategoryName(_leagueName);
        InterfaceLeagueCategory? databaseLeagueInterface = GetLeagueInstanceWithLeagueCategoryName(_leagueName);

        Log.WriteLine("Made a " + nameof(leagueInterface) + " named: " +
             leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

        if (!CheckIfALeagueCategoryNameExistsInDatabase(_leagueName))
        {
            Log.WriteLine(_leagueName.ToString() + " not found in db! ", LogLevel.CRITICAL);
        }

        Log.WriteLine("name: " + _leagueName.ToString() +
            " was already in the database list", LogLevel.VERBOSE);

        databaseLeagueInterface = GetInterfaceLeagueCategoryFromTheDatabase(leagueInterface);

        if (databaseLeagueInterface == null)
        {
            Log.WriteLine("databaseLeagueInterface with " + _leagueName.ToString() +
                " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Starting to store league: " + leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);
        if (Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.Values == null)
        {
            Log.WriteLine(nameof(Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.Values) +
                " was null!", LogLevel.CRITICAL);
            return;
        }

        if (leagueInterface == null)
        {
            Log.WriteLine("leagueInterface with " + _leagueName.ToString() +
                " was null!", LogLevel.CRITICAL);
            return;
        }

        /*
        if (!alreadyInDatabase)
        {
            Log.WriteLine("League " + leagueInterface.LeagueCategoryName +
                " not in database, adding.", LogLevel.DEBUG);

            Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.Values.Add(leagueInterface);
        }
        else
        {

        }

        Log.WriteLine("League " + leagueInterface.LeagueCategoryName +
    " found in the database, updating.", LogLevel.DEBUG);

        if (databaseLeagueInterface == null)
        {
            Log.WriteLine("league with " + leagueInterface.LeagueCategoryName +
                " was null!", LogLevel.CRITICAL);
            return;
        }*/

        databaseLeagueInterface.LeagueUnits = leagueInterface.LeagueUnits; // Update the units/


        Log.WriteLine("Done storing a league: " + leagueInterface.LeagueCategoryName, LogLevel.DEBUG);
    }


    /*
    public static InterfaceLeagueCategory GetLeagueInstanceWithString(string _leagueCategoryName)
    {
        Log.WriteLine("Getting a league instance with string: " + _leagueCategoryName, LogLevel.VERBOSE);

        var leagueInstance = (InterfaceLeagueCategory)EnumExtensions.GetInstance(_leagueCategoryName);
        leagueInstance.LeagueCategoryName = _leagueCategoryName;
        Log.WriteLine(nameof(leagueInstance) + ": " + leagueInstance.ToString(), LogLevel.VERBOSE);
        return leagueInstance;
    }*/

    public static InterfaceLeagueCategory GetLeagueInstanceWithLeagueCategoryName(LeagueCategoryName _leagueCategoryName)
    {
        Log.WriteLine("Getting a league instance with LeagueCategoryName: " + _leagueCategoryName, LogLevel.VERBOSE);

        var leagueInstance = (InterfaceLeagueCategory)EnumExtensions.GetInstance(_leagueCategoryName.ToString());
        leagueInstance.LeagueCategoryName = _leagueCategoryName;
        Log.WriteLine(nameof(leagueInstance) + ": " + leagueInstance.ToString(),LogLevel.VERBOSE);
        return leagueInstance;
    }

    public static InterfaceLeagueCategory FindLeagueAndReturnInterfaceFromDatabase(InterfaceLeagueCategory _interfaceToSearchFor)
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



    /*
    public static void StoreTheLeague(InterfaceLeagueCategory _leagueInterface)
    {
        Log.WriteLine("Starting to store league: " + _leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

        /*
        if (_leagueInterface.DiscordLeagueReferences.leagueRegistrationChannelMessageId == 0)
        {
            Log.WriteLine(nameof(_leagueInterface.DiscordLeagueReferences.leagueRegistrationChannelMessageId) +
            " was 0, channel not created succesfully?", LogLevel.CRITICAL);
            return;
        }

        if (Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.Values == null)
        {
            Log.WriteLine(nameof(Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.Values) + " was null!", LogLevel.CRITICAL);
            return;
        }

        Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels[].Add(_leagueInterface);
        Log.WriteLine("Done storing a league: " + _leagueInterface.LeagueCategoryName, LogLevel.DEBUG);
    }*/

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