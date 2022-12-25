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

    public static async Task CreateLeaguesOnStartupIfNecessary()
    {
        Log.WriteLine("Starting to create leagues for the discord server", LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();
        Log.WriteLine("guild valid", LogLevel.VERBOSE);
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        // Get all of the league category names and loop through them to create the database entries
        var categoryEnumValues = Enum.GetValues(typeof(CategoryName));
        Log.WriteLine(nameof(categoryEnumValues) + " length: " + categoryEnumValues.Length, LogLevel.VERBOSE);
        foreach (CategoryName leagueCategoryName in categoryEnumValues)
        {
            Log.WriteLine("Looping on category name: " + leagueCategoryName.ToString(), LogLevel.VERBOSE);
            // Check here too if a category is missing channelNames
            //bool categoryExists = false;

            ILeague interfaceLeagueCategory = GetCategoryInstance(leagueCategoryName);
            Log.WriteLine("after setting interface", LogLevel.VERBOSE);
            if (interfaceLeagueCategory == null)
            {
                Log.WriteLine(nameof(interfaceLeagueCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            // Add the new newly from the interface implementations added units here
            if (Database.Instance.StoredLeagues.Any(
                x => x.LeagueCategoryName == interfaceLeagueCategory.LeagueCategoryName))
            {
                Log.WriteLine(nameof(Database.Instance.StoredLeagues) +
                    " already contains: " + interfaceLeagueCategory.ToString() +
                    " adding new units to the league", LogLevel.VERBOSE);

                // Update the units and to the database (before interfaceLeagueCategory is replaced by it)
                Database.Instance.StoredLeagues.First(
                    x => x.LeagueCategoryName == interfaceLeagueCategory.LeagueCategoryName).LeagueUnits = interfaceLeagueCategory.LeagueUnits;

                /*
                // Replace InterfaceLeagueCategoryCategory with a one that is from the database
                System.Collections.Generic.KeyValuePair<
                    ulong, InterfaceCategory> interfaceLeagueCategorykvp =
                    Database.Instance.StoredLeagues.First(
                        x => x.Value.LeagueCategoryName == leagueCategoryName);
                interfaceLeagueCategory = interfaceLeagueCategorykvp.Value;


                Log.WriteLine("Replaced with: " + interfaceLeagueCategory.LeagueCategoryName + " from db", LogLevel.DEBUG);
                */

                //categoryExists = true;

                /*
                categoryExists = await CategoryRestore.CheckIfCategoryHasBeenDeletedAndRestoreForCategory(
                    interfaceLeagueCategorykvp, guild);                */

                continue;
            }

            Log.WriteLine("Adding to the stored leagues...", LogLevel.VERBOSE);

            Database.Instance.StoredLeagues.Add(interfaceLeagueCategory);

            Log.WriteLine("Done adding " + nameof(interfaceLeagueCategory) + " to " +
                nameof(Database.Instance.StoredLeagues) +
                " count: " + Database.Instance.StoredLeagues.Count, LogLevel.DEBUG);


            /*
            interfaceLeagueCategory.LeagueCategoryName = leagueCategoryName;

            string? leagueCategoryNameString = EnumExtensions.GetEnumMemberAttrValue(leagueCategoryName);
            if (leagueCategoryNameString == null)
            {
                Log.WriteLine(nameof(leagueCategoryName).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Creating a category named: " + leagueCategoryNameString, LogLevel.VERBOSE);

            BaseLeague baseLeagueCategory = interfaceLeagueCategory as BaseLeague;
            if (baseLeagueCategory == null)
            {
                Log.WriteLine(nameof(baseLeagueCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            // Get the role and create it if it already doesn't exist
            SocketRole role = RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
                guild, leagueCategoryNameString).Result;

            Log.WriteLine("Role is named: " + role.Name + " with ID: " + role.Id, LogLevel.VERBOSE);

            interfaceLeagueCategory.DiscordLeagueReferences.leagueRoleId = role.Id;

            SocketCategoryChannel? socketCategoryChannel = null;
            */
            // If the category doesn't exist at all, create it and add it to the database
            //if (!categoryExists)
            //{
            /*
            socketCategoryChannel =
                await CategoryManager.CreateANewSocketCategoryChannelAndReturnIt(
                    guild, leagueCategoryNameString, baseLeagueCategory.GetGuildPermissions(guild, role));
            if (socketCategoryChannel == null)
            {
                Log.WriteLine(nameof(socketCategoryChannel) + " was null!", LogLevel.CRITICAL);
                return;
            }

            interfaceLeagueCategory.DiscordLeagueReferences.leagueCategoryId = socketCategoryChannel.Id;

            Log.WriteLine("Created a " + nameof(socketCategoryChannel) + " with id: " + socketCategoryChannel.Id +
                " that's named: " + socketCategoryChannel.Name, LogLevel.VERBOSE);

            Log.WriteLine("Adding " + nameof(interfaceLeagueCategory) + " to " +
                nameof(Database.Instance.StoredLeagues), LogLevel.VERBOSE); */


        }
        /*
        // The category exists, just find it from the database and then get the id of the socketchannel
        else
        {
            var dbCategory = Database.Instance.StoredLeagues.First(
                x => x.LeagueCategoryName == interfaceLeagueCategory.LeagueCategoryName);

            ILeague databaseInterfaceLeagueCategory = GetCategoryInstance(leagueCategoryName);
            if (databaseInterfaceLeagueCategory == null)
            {
                Log.WriteLine(nameof(databaseInterfaceLeagueCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Found " + nameof(databaseInterfaceLeagueCategory) + " with id: " +
                dbCategory + " named: " +
                databaseInterfaceLeagueCategory.LeagueCategoryName, LogLevel.VERBOSE);

            //socketCategoryChannel = guild.GetCategoryChannel(dbCategory);

            /*
            Log.WriteLine("Found " + nameof(socketCategoryChannel) + " that's named: " +
                socketCategoryChannel.Name, LogLevel.DEBUG); */
        //}

        //Log.WriteLine("FINAL " + nameof(interfaceLeagueCategory) + " for " + leagueCategoryName.ToString() +
        //      "::" + interfaceLeagueCategory.LeagueCategoryName + " beforing creating channels", LogLevel.DEBUG);

        //await CreateChannelsForTheLeagueCategory(interfaceLeagueCategory, socketCategoryChannel, guild);
        //}
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

        if (CheckIfALeagueCategoryNameExistsInDatabase(_leagueInterface.LeagueCategoryName))
        {
            Log.WriteLine(_leagueInterface.LeagueCategoryName +
                " exists in the database!", LogLevel.DEBUG);

            var newInterfaceLeagueCategory =
                Database.Instance.StoredLeagues.First(
                    l => l.LeagueCategoryName == _leagueInterface.LeagueCategoryName);

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

    private static bool CheckIfALeagueCategoryNameExistsInDatabase(CategoryName _leagueName)
    {
        return Database.Instance.StoredLeagues.Any(l => l.LeagueCategoryName == _leagueName);
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