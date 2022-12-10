using Discord;
using System;

public static class LeagueManager
{
    public static async Task CreateLeaguesOnStartup()
    {
        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        // Hardcoded channel id for now
        var channel = guild.GetTextChannel(1049555859656671232) as ITextChannel;

        if (channel == null)
        {
            Log.WriteLine("Channel was null, wrong id in the code?", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Channel found: " + channel.Name +
            "(" + channel.Id + ")", LogLevel.VERBOSE);

        foreach (LeagueName leagueName in Enum.GetValues(typeof(LeagueName)))
        {
            CreateALeague(channel, leagueName);
        }

        await SerializationManager.SerializeDB();
    }

    private static ILeague? GetILeagueFromTheDatabase(ILeague _leagueInterface)
    {
        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Checking if " + _leagueInterface.LeagueName +
            " has _leagueInterface in the database", LogLevel.VERBOSE);

        if (CheckIfALeagueNameExistsInDatabase(_leagueInterface.LeagueName))
        {
            Log.WriteLine(_leagueInterface.LeagueName +
                " exists in the database!", LogLevel.DEBUG);

            var newILeague = Database.Instance.StoredLeagues.Find(
                l => l.LeagueName == _leagueInterface.LeagueName);

            if (newILeague == null)
            {
                Log.WriteLine(nameof(newILeague) + " was null!", LogLevel.CRITICAL);
                return null;
            }

            Log.WriteLine("found result: " + newILeague.LeagueName, LogLevel.DEBUG);
            return newILeague;
        }
        else
        {
            Log.WriteLine(_leagueInterface.LeagueName + " does not exist in the database," +
                " creating a new LeagueData for it", LogLevel.DEBUG);

            _leagueInterface.LeagueData = new LeagueData();
            _leagueInterface.DiscordLeagueReferences = new DiscordLeagueReferences();

            return _leagueInterface;
        }
    }

    private static bool CheckIfALeagueNameExistsInDatabase(LeagueName _leagueName)
    {
        return Database.Instance.StoredLeagues.Any(l => l.LeagueName == _leagueName);
    }

    private static bool CheckIfLeagueRegisterationMessageExists(ILeague _leagueInterface)
    {
        Log.WriteLine("Checking if _leagueInterface registration message exists", LogLevel.VERBOSE);

        if (_leagueInterface.DiscordLeagueReferences.leagueRegisterationChannelMessageId == 0)
        {
            Log.WriteLine("leagueData.leagueChannelMessageId was 0!", LogLevel.CRITICAL);
            return false;
        }

        Log.WriteLine("Found: " + _leagueInterface.DiscordLeagueReferences.leagueRegisterationChannelMessageId, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return false;
        }

        var channel = guild.GetTextChannel(1049555859656671232) as ITextChannel;

        var message = channel.GetMessageAsync(_leagueInterface.DiscordLeagueReferences.leagueRegisterationChannelMessageId);

        if (message.Result != null)
        {
            Log.WriteLine("message: " + message.Result.ToString() + " found.", LogLevel.DEBUG);
            return true;
        }
        // Returns false otherwise
        Log.WriteLine("returning false", LogLevel.DEBUG);
        return false;
    }

    public static void CreateALeague(ITextChannel _channel, LeagueName _leagueName)
    {
        bool leagueExistsInTheDatabase = false;
        bool leagueRegisterationMessageExists = false;
        bool leagueChannelCategoryExists = false;
        bool newTypesOfLeagueChannels = false;

        Log.WriteLine("Looping on leagueName: " + _leagueName.ToString(), LogLevel.VERBOSE);

        ILeague? leagueInterface = GetLeagueInstance(_leagueName.ToString());

        Log.WriteLine("Made a " + nameof(leagueInterface) + " named: " +
             leagueInterface.LeagueName, LogLevel.VERBOSE);

        if (Database.Instance.StoredLeagues == null)
        {
            Log.WriteLine(nameof(Database.Instance.StoredLeagues) + " was null!", LogLevel.CRITICAL);
            return;
        }

        if (CheckIfALeagueNameExistsInDatabase(_leagueName))
        {
            Log.WriteLine("name: " + _leagueName.ToString() +
                " was already in the database list", LogLevel.VERBOSE);

            leagueExistsInTheDatabase = true;

            leagueInterface = GetILeagueFromTheDatabase(leagueInterface);

            if (leagueInterface == null)
            {
                Log.WriteLine("leagueInterface with " + _leagueName.ToString() +
                    " was null!", LogLevel.CRITICAL);
                return;
            }

            leagueRegisterationMessageExists = CheckIfLeagueRegisterationMessageExists(leagueInterface);

            leagueChannelCategoryExists = CategoryManager.CheckIfLeagueCategoryExists(
                leagueInterface.DiscordLeagueReferences.leagueCategoryId).Result;

            newTypesOfLeagueChannels = Enum.GetValues(typeof(LeagueCategoryChannelType)).Length >
                leagueInterface.DiscordLeagueReferences.leagueChannels.Count ? true : false;

            Log.WriteLine(nameof(newTypesOfLeagueChannels) + ": " + newTypesOfLeagueChannels, LogLevel.VERBOSE);

            if (leagueRegisterationMessageExists && leagueChannelCategoryExists && !newTypesOfLeagueChannels)
            {
                Log.WriteLine(nameof(leagueRegisterationMessageExists) + " and " +
                    nameof(leagueChannelCategoryExists) + " true, returning.", LogLevel.DEBUG);
                return;
            }
        }

        Log.WriteLine(nameof(leagueRegisterationMessageExists) + ": " + leagueRegisterationMessageExists + 
            " | " + nameof(leagueChannelCategoryExists) + ": " + leagueChannelCategoryExists, LogLevel.DEBUG);

        if (!leagueRegisterationMessageExists)
        {
            Log.WriteLine("name: " + _leagueName.ToString() +
                " was not found, creating a button for it", LogLevel.DEBUG);
            leagueInterface = CreateALeagueJoinButton(_channel, leagueInterface).Result;
        }

        if (!leagueChannelCategoryExists || newTypesOfLeagueChannels)
        {
            // Category was deleted or does not exist, clear channelNames in the db and generate a new category
            if (!newTypesOfLeagueChannels)
            {
                Log.WriteLine("name: " + _leagueName.ToString() +
                    " was not found, creating a category for it", LogLevel.DEBUG);

                leagueInterface.DiscordLeagueReferences.leagueChannels.Clear();
                LeagueChannelManager.CreateCategoryAndChannelsForALeague(leagueInterface);
            }
            // New channelNames detected in the code, create them
            else
            {
                Log.WriteLine("New types of league channels detected in the code," +
                    " fixing that for league: " + _leagueName.ToString(), LogLevel.DEBUG);

                var guild = BotReference.GetGuildRef();
                if (guild == null)
                {
                    Exceptions.BotGuildRefNull();
                    return;
                }

                //LeagueChannelManager.CreateChannelsForTheCategory(leagueInterface, guild);
            }
        }

        // To avoid duplicates in the db
        if (!leagueExistsInTheDatabase)
        {
            Log.WriteLine("The league doesn't exist in the database, storing it", LogLevel.DEBUG);
            if (leagueInterface == null)
            {
                Log.WriteLine("leagueInterface for channel: " + _channel.Id + " and _leagueName: " +
                    _leagueName.ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            StoreTheLeague(leagueInterface);
        }
    }

    public static async Task<ILeague?> CreateALeagueJoinButton(
        ITextChannel _channel, ILeague? _leagueInterface)
    {
        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return null;
        }

        string leagueButtonRegisterationCustomId =
           "leagueRegisteration_" + _leagueInterface.LeagueName.ToString();

        Log.WriteLine(nameof(leagueButtonRegisterationCustomId) + ": " +
            leagueButtonRegisterationCustomId, LogLevel.VERBOSE);

        _leagueInterface = GetILeagueFromTheDatabase(_leagueInterface);

        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return null;
        }

        _leagueInterface.DiscordLeagueReferences.leagueRegisterationChannelMessageId =
            await ButtonComponents.CreateButtonMessage(
                _channel,
                GenerateALeagueJoinButtonMessage(_leagueInterface),
                "Join",
                leagueButtonRegisterationCustomId); // Maybe replace this with some other system

        return _leagueInterface;
    }

    public static string GenerateALeagueJoinButtonMessage(ILeague _leagueInterface)
    {
        string? leagueEnumAttrValue =
            EnumExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueName);

        Log.WriteLine(nameof(leagueEnumAttrValue) + ": " +
            leagueEnumAttrValue, LogLevel.VERBOSE);

        return "." + "\n" + leagueEnumAttrValue + "\n" +
            GetAllowedUnitsAsString(_leagueInterface) + "\n" +
            GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(_leagueInterface);
             
    }

    private static string GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(ILeague _leagueInterface)
    {
        int count = 0;

        foreach (Team team in _leagueInterface.LeagueData.Teams)
        {
            if (team.active)
            {
                count++;
                Log.WriteLine("team: " + team.teamName +
                    " is active, increased count to: " + count, LogLevel.VERBOSE);
            }
            else
            {
                Log.WriteLine("team: " + team.teamName + " is not active", LogLevel.VERBOSE);
            }
        }

        Log.WriteLine("Total count: " + count, LogLevel.VERBOSE);

        if (_leagueInterface.LeaguePlayerCountPerTeam > 1)
        {
            return "Teams: " + count;
        }
        else
        {
            return "Players: " + count;
        }
    }

    public static ILeague GetLeagueInstance(string _leagueName)
    {
        return (ILeague)EnumExtensions.GetInstance(_leagueName);
    }

    public static ILeague FindLeagueAndReturnInterfaceFromDatabase(ILeague _interfaceToSearchFor)
    {
        var dbLeagueInstance = Database.Instance.StoredLeagues.Find(l => l.LeagueName == _interfaceToSearchFor.LeagueName);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) + " was null! Could not find the league.", LogLevel.CRITICAL);
            return _interfaceToSearchFor;
        }

        Log.WriteLine("Found " + nameof(dbLeagueInstance) + ": " + dbLeagueInstance.LeagueName, LogLevel.DEBUG);

        return dbLeagueInstance;
    }

    private static string GetAllowedUnitsAsString(ILeague _leagueInterface)
    {
        string allowedUnits = string.Empty;

        for (int u = 0; u < _leagueInterface.LeagueUnits.Count; ++u)
        {
            allowedUnits += EnumExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueUnits[u]);

            // Is not the last index
            if (u != _leagueInterface.LeagueUnits.Count - 1)
            {
                allowedUnits += ", ";
            }
        }

        return allowedUnits;
    }

    public static void StoreTheLeague(ILeague _leagueInterface)
    {
        Log.WriteLine("Starting to store league: " + _leagueInterface.LeagueName, LogLevel.VERBOSE);

        if (_leagueInterface.DiscordLeagueReferences.leagueRegisterationChannelMessageId == 0)
        {
            Log.WriteLine(nameof(_leagueInterface.DiscordLeagueReferences.leagueRegisterationChannelMessageId) +
            " was 0, channel not created succesfully?", LogLevel.CRITICAL);
            return;
        }

        if (Database.Instance.StoredLeagues == null)
        {
            Log.WriteLine(nameof(Database.Instance.StoredLeagues) + " was null!", LogLevel.CRITICAL);
            return;
        }

        Database.Instance.StoredLeagues.Add(_leagueInterface);
        Log.WriteLine("Done storing a league: " + _leagueInterface.LeagueName, LogLevel.DEBUG);
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