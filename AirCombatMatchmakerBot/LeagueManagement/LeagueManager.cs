using Discord;
using Discord.WebSocket;
using System;

public static class LeagueManager
{
    public static ulong leagueRegistrationChannelId;

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

        foreach (LeagueName leagueName in Enum.GetValues(typeof(LeagueName)))
        {
            CreateALeague(channel, leagueName);
        }

        Log.WriteLine("Done creating leagues.", LogLevel.VERBOSE);

        //await SerializationManager.SerializeDB();
    }

    public static ILeague? GetILeagueFromTheDatabase(ILeague _leagueInterface)
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
        bool alreadyInDatabase = false;

        Log.WriteLine("Looping on leagueName: " + _leagueName.ToString(), LogLevel.VERBOSE);

        ILeague? leagueInterface = GetLeagueInstance(_leagueName.ToString());
        ILeague? databaseLeagueInterface = GetLeagueInstance(_leagueName.ToString());

        Log.WriteLine("Made a " + nameof(leagueInterface) + " named: " +
             leagueInterface.LeagueName, LogLevel.VERBOSE);

        if (CheckIfALeagueNameExistsInDatabase(_leagueName))
        {
            Log.WriteLine("name: " + _leagueName.ToString() +
                " was already in the database list", LogLevel.VERBOSE);

            databaseLeagueInterface = GetILeagueFromTheDatabase(leagueInterface);

            alreadyInDatabase = true;
        }

        Log.WriteLine("Starting to store league: " + leagueInterface.LeagueName, LogLevel.VERBOSE);
        if (Database.Instance.StoredLeagues == null)
        {
            Log.WriteLine(nameof(Database.Instance.StoredLeagues) + " was null!", LogLevel.CRITICAL);
            return;
        }

        if (leagueInterface == null)
        {
            Log.WriteLine("leagueInterface with " + _leagueName.ToString() +
                " was null!", LogLevel.CRITICAL);
            return;
        }

        if (!alreadyInDatabase)
        {
            Log.WriteLine("League " + leagueInterface.LeagueName +
                " not in database, adding.", LogLevel.DEBUG);

            Database.Instance.StoredLeagues.Add(leagueInterface);
        }
        else
        {
            Log.WriteLine("League " + leagueInterface.LeagueName +
                " found in the database, updating.", LogLevel.DEBUG);

            if (databaseLeagueInterface == null)
            {
                Log.WriteLine("league with " + leagueInterface.LeagueName +
                    " was null!", LogLevel.CRITICAL);
                return;
            }

            databaseLeagueInterface.LeagueUnits = leagueInterface.LeagueUnits; // Update the units/
        }

        
        Log.WriteLine("Done storing a league: " + leagueInterface.LeagueName, LogLevel.DEBUG);
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



    public static void StoreTheLeague(ILeague _leagueInterface)
    {
        Log.WriteLine("Starting to store league: " + _leagueInterface.LeagueName, LogLevel.VERBOSE);

        /*
        if (_leagueInterface.DiscordLeagueReferences.leagueRegisterationChannelMessageId == 0)
        {
            Log.WriteLine(nameof(_leagueInterface.DiscordLeagueReferences.leagueRegisterationChannelMessageId) +
            " was 0, channel not created succesfully?", LogLevel.CRITICAL);
            return;
        }*/

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