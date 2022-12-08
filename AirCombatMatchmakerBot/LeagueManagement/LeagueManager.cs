using Discord;

public static class LeagueManager
{
    public static Task CreateLeaguesOnStartup()
    {
        var guild = BotReference.GetGuildRef();

        if (guild != null)
        {
            // Hardcoded channel id for now
            var channel = guild.GetTextChannel(1049555859656671232) as ITextChannel;

            if (channel != null)
            {
                Log.WriteLine("Channel found: " + channel.Name +
                    "(" + channel.Id + ")", LogLevel.VERBOSE);

                foreach (LeagueName leagueName in Enum.GetValues(typeof(LeagueName)))
                {
                    CreateALeague(channel, leagueName);
                }
            }
            else Log.WriteLine("Channel was null, wrong id in the code?", LogLevel.CRITICAL);

        } else Exceptions.BotGuildRefNull();

        return Task.CompletedTask;
    }

    private static ILeague? GetILeagueFromTheDatabase(ILeague _leagueInterface)
    {
        Log.WriteLine("Checking if " + _leagueInterface.LeagueName +
            " has _leagueInterface in the database", LogLevel.VERBOSE);

        if (_leagueInterface != null)
        {
            if (CheckIfALeagueNameExistsInDatabase(_leagueInterface.LeagueName))
            {
                Log.WriteLine(_leagueInterface.LeagueName +
                    " exists in the database!", LogLevel.DEBUG);

                var newILeague = Database.Instance.StoredLeagues.Find(
                    l => l.LeagueName == _leagueInterface.LeagueName);

                if (newILeague != null)
                {
                    Log.WriteLine("found result: " + newILeague.LeagueName, LogLevel.DEBUG);
                    return newILeague;
                }
            }
            else
            {
                Log.WriteLine(_leagueInterface.LeagueName + " does not exist in the database," +
                    " creating a new LeagueData for it", LogLevel.DEBUG);

                _leagueInterface.LeagueData = new LeagueData();

                return _leagueInterface;
            }
        }
        else Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);

        return _leagueInterface;
    }

    private static bool CheckIfALeagueNameExistsInDatabase(LeagueName _leagueName)
    {
        return Database.Instance.StoredLeagues.Any(l => l.LeagueName == _leagueName);
    }

    private static bool CheckIfLeagueRegisterationMessageExists(ILeague _leagueInterface)
    {
        Log.WriteLine("Checking if _leagueInterface registeration message exists", LogLevel.VERBOSE);

        if (_leagueInterface.LeagueData.leagueChannelMessageId != 0)
        {
            Log.WriteLine("Found: " + _leagueInterface.LeagueData.leagueChannelMessageId, LogLevel.DEBUG);

            var guild = BotReference.GetGuildRef();

            if (guild != null)
            {
                var channel = guild.GetTextChannel(1049555859656671232) as ITextChannel;

                var message = channel.GetMessageAsync(_leagueInterface.LeagueData.leagueChannelMessageId);

                if (message.Result != null)
                {
                    Log.WriteLine("message: " + message.Result.ToString() + " found.", LogLevel.DEBUG);
                    return true;
                }
                // Returns false otherwise
            }
            else { Exceptions.BotGuildRefNull(); }
        } else Log.WriteLine("leagueData.leagueChannelMessageId was 0!", LogLevel.CRITICAL);

        Log.WriteLine("returning false", LogLevel.DEBUG);

        return false;
    }

    public static void CreateALeague(ITextChannel _channel, LeagueName _leagueName)
    {
        Log.WriteLine("Looping on leagueName: " + _leagueName.ToString(), LogLevel.VERBOSE);

        ILeague leagueInterface = GetLeagueInstance(_leagueName.ToString());

        Log.WriteLine("Made a " + nameof(leagueInterface) + " named: " +
             leagueInterface.LeagueName, LogLevel.VERBOSE);

        if (Database.Instance.StoredLeagues != null)
        {
            if (CheckIfALeagueNameExistsInDatabase(_leagueName))
            {
                Log.WriteLine("name: " + _leagueName.ToString() +
                    " was already in the list", LogLevel.VERBOSE);

                leagueInterface = GetILeagueFromTheDatabase(leagueInterface);

                if (leagueInterface == null)
                {
                    Log.WriteLine("leagueInterface with " + _leagueName.ToString() +
                        " was null!", LogLevel.CRITICAL);
                    return;
                }

                if (CheckIfLeagueRegisterationMessageExists(leagueInterface))
                {
                    return;
                }
            }

            Log.WriteLine("name: " + _leagueName.ToString() +
                " was not found, creating a League button for it", LogLevel.DEBUG);

            leagueInterface = CreateALeagueJoinButton(_channel, leagueInterface).Result;

            StoreTheLeague(leagueInterface);
        }
        else Log.WriteLine(nameof(Database.Instance.StoredLeagues) +
            " was null!", LogLevel.CRITICAL);
    }

    public static async Task<ILeague> CreateALeagueJoinButton(
        ITextChannel _channel, ILeague _leagueInterface)
    {
        string leagueButtonRegisterationCustomId =
           "leagueRegisteration_" + _leagueInterface.LeagueName.ToString();

        Log.WriteLine(nameof(leagueButtonRegisterationCustomId) + ": " +
            leagueButtonRegisterationCustomId, LogLevel.VERBOSE);

        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return _leagueInterface;
        }

        _leagueInterface = GetILeagueFromTheDatabase(_leagueInterface);

        _leagueInterface.LeagueData.leagueChannelMessageId =
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

        return "\\" + "\n" + leagueEnumAttrValue + "\n" +
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

        if (dbLeagueInstance != null)
        {
            Log.WriteLine("Found " + nameof(dbLeagueInstance) + ": " + dbLeagueInstance.LeagueName, LogLevel.VERBOSE);

            return dbLeagueInstance;
        }
        else Log.WriteLine(nameof(dbLeagueInstance) + " was null! Could not find the league.", LogLevel.CRITICAL);

        return _interfaceToSearchFor;
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
        if (_leagueInterface.LeagueData.leagueChannelMessageId != 0)
        {
            if (Database.Instance.StoredLeagues != null)
            {
                Database.Instance.StoredLeagues.Add(_leagueInterface);
            }
            else Log.WriteLine(nameof(Database.Instance.StoredLeagues) +
                " was null!", LogLevel.CRITICAL);
        }
        else Log.WriteLine(nameof(_leagueInterface.LeagueData.leagueChannelMessageId) +
            " was 0, channel not created succesfully?", LogLevel.CRITICAL);
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