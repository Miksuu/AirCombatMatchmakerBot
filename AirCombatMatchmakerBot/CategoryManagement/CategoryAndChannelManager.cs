using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;

public static class CategoryAndChannelManager
{
    // Do not create these categories,
    // as they are used as template (such as generating from a league template)
    private static List<CategoryType> categoriesThatWontGetGenerated = new List<CategoryType> {
        CategoryType.LEAGUETEMPLATE };

    public static async Task CreateCategoriesAndChannelsForTheDiscordServer()
    {
        Log.WriteLine("Starting to create categories and channels for" +
            " the discord server", LogLevel.VERBOSE);

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return;
        }

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        foreach (LeagueName categoryName in Enum.GetValues(typeof(LeagueName)))
        {
            Log.WriteLine("Looping on league category name: " + categoryName, LogLevel.VERBOSE);
            await GenerateLeagueCategoryFromName(client, guild, categoryName);
        }

        foreach (CategoryType categoryName in Enum.GetValues(typeof(CategoryType)))
        {
            Log.WriteLine("Looping on category name: " + categoryName, LogLevel.VERBOSE);

            // Skip creating from the default LeagueTemplate
            if (categoriesThatWontGetGenerated.Contains(categoryName))
            {
                Log.WriteLine("category name is: " + categoryName +
                    " skipping creation for this type of category", LogLevel.VERBOSE);
                continue;
            }

            await GenerateARegularCategoryFromName(client, guild, categoryName);
        }

        Log.WriteLine("Done looping through the category names.", LogLevel.VERBOSE);
    }

    private static async Task GenerateLeagueCategoryFromName(
        DiscordSocketClient _client, SocketGuild _guild, LeagueName _leagueName)
    {
        try
        {
            Log.WriteLine("This is a league category", LogLevel.DEBUG);

            InterfaceCategory interfaceCategory = GetCategoryInstance(CategoryType.LEAGUETEMPLATE);
            //interfaceCategory.CategoryType = _categoryName;

            // Cached for later use (inserting the category ID)
            //leagueCategoryName = leagueInterface.LeagueCategoryName;

            InterfaceLeague leagueInterface = GetLeagueInstance(_leagueName);

            Log.WriteLine("Got " + nameof(leagueInterface) +
                leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

            string finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(_leagueName);
            Log.WriteLine("Category name is: " + interfaceCategory.CategoryType, LogLevel.VERBOSE);

            if (interfaceCategory == null)
            {
                Log.WriteLine(nameof(interfaceCategory).ToString() +
                    " was null!", LogLevel.CRITICAL);
                return;
            }

            //BaseLeague baseLeague = new();
            //InterfaceLeague interfaceLeague = (InterfaceLeague)baseLeague;
            InterfaceLeague interfaceLeague = null;

            // Add the new newly from the interface implementations added units here
            if (Database.Instance.Leagues.CheckIfILeagueExistsByCategoryName(
                leagueInterface.LeagueCategoryName))
            {
                Log.WriteLine(nameof(Database.Instance.Leagues) +
                    " already contains: " + leagueInterface.ToString() +
                    " adding new units to the league", LogLevel.VERBOSE);

                // Update the units and to the database (before interfaceLeagueCategory is replaced by it)
                interfaceLeague = Database.Instance.Leagues.GetILeagueByCategoryName(
                    leagueInterface.LeagueCategoryName);

                // Clears the queue on the startup
                if (interfaceLeague.LeagueData.ChallengeStatus.TeamsInTheQueue.Count > 0)
                {
                    interfaceLeague.LeagueData.ChallengeStatus.TeamsInTheQueue.Clear();
                    var message = Database.Instance.Categories.FindInterfaceCategoryWithId(
                        interfaceLeague.LeagueCategoryId).FindInterfaceChannelWithNameInTheCategory(
                            ChannelType.CHALLENGE).FindInterfaceMessageWithNameInTheChannel(MessageName.CHALLENGEMESSAGE);

                    await message.GenerateAndModifyTheMessage();
                }

                interfaceLeague.LeagueUnits = leagueInterface.LeagueUnits;
            }
            else
            {
                Database.Instance.Leagues.AddToStoredLeagues(leagueInterface);
                interfaceLeague = leagueInterface;
            }

            SocketCategoryChannel? socketCategoryChannel = null;

            bool contains = false;
            Log.WriteLine("searching for categoryname: " + interfaceCategory.CategoryType, LogLevel.VERBOSE);
            foreach (var storedLeague in Database.Instance.Leagues.StoredLeagues)
            {
                Log.WriteLine("categoryname:" + storedLeague.LeagueCategoryName, LogLevel.VERBOSE);
                if (storedLeague.LeagueCategoryName == leagueInterface.LeagueCategoryName)
                {
                    // Checks if the channel is also in the discord server itself too, not only database
                    contains = CategoryRestore.CheckIfCategoryHasBeenDeletedAndRestoreForCategory(storedLeague.LeagueCategoryId, _guild);
                    break;
                }
            }

            SocketRole? role = RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
                        _guild, finalCategoryName).Result;

            Log.WriteLine("Role is named: " + role.Name + " with ID: " + role.Id, LogLevel.VERBOSE);



            // The category exists,
            // just find it from the database and then get the id of the socketchannel
            if (contains)
            {
                Log.WriteLine("Category: " + finalCategoryName +
                    " found, checking it's channels", LogLevel.VERBOSE);

                InterfaceCategory? dbCategory =
                    Database.Instance.Categories.FindInterfaceCategoryByCategoryName(
                        interfaceCategory.CategoryType);

                Log.WriteLine("Found " + nameof(dbCategory) + " named: " +
                    dbCategory.CategoryType, LogLevel.VERBOSE);

                interfaceCategory = dbCategory as InterfaceCategory;

                socketCategoryChannel = _guild.GetCategoryChannel(dbCategory.SocketCategoryChannelId);

                // Insert a fix here if the category is still in DB but does not exist

                Log.WriteLine("Found " + nameof(socketCategoryChannel) + " that's named: " +
                    socketCategoryChannel.Name, LogLevel.DEBUG);

            }
            // If the category doesn't exist at all, create it and add it to the database
            else
            {
                socketCategoryChannel =
                    await interfaceCategory.CreateANewSocketCategoryChannelAndReturnIt(
                        _guild, finalCategoryName, role);

                Log.WriteLine("Created a " + nameof(socketCategoryChannel) +
                    " with id: " + socketCategoryChannel.Id +
                    " that's named: " + socketCategoryChannel.Name, LogLevel.VERBOSE);

                Log.WriteLine("Is league, inserting " + socketCategoryChannel.Id +
                    " to " + _leagueName, LogLevel.DEBUG);

                LeagueName categoryTypeNonNullable = (LeagueName)_leagueName;

                // TODO: refactor this mess
                interfaceLeague.LeagueRoleId = role.Id;
                interfaceLeague.LeagueCategoryId = socketCategoryChannel.Id;
                //interfaceLeague.LeagueData.InterfaceLeagueCategoryId = socketCategoryChannel.Id;
                //interfaceLeague.LeagueData.interfaceLeagueRef = interfaceLeague;
                interfaceLeague.LeagueData.SetReferences(interfaceLeague);

                Database.Instance.Categories.AddToCreatedCategoryWithChannelWithUlongAndInterfaceCategory(
                    socketCategoryChannel.Id, interfaceCategory);
            }

            // Handle channel checking/creation
            await interfaceCategory.CreateChannelsForTheCategory(socketCategoryChannel.Id, _client, role);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }
    }

    private static async Task GenerateARegularCategoryFromName(
        DiscordSocketClient _client, SocketGuild _guild, CategoryType _categoryName)
    {
        try
        {
            Log.WriteLine("Generating category named: " + _categoryName, LogLevel.VERBOSE);

            InterfaceCategory interfaceCategory = GetCategoryInstance(_categoryName);

            if (interfaceCategory == null)
            {
                Log.WriteLine(nameof(interfaceCategory).ToString() +
                    " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("interfaceCategory name: " +
                interfaceCategory.CategoryType, LogLevel.DEBUG);

            Log.WriteLine(nameof(interfaceCategory.CategoryType) +
                ": " + interfaceCategory.CategoryType, LogLevel.VERBOSE);

            string finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(interfaceCategory.CategoryType);
            Log.WriteLine("Category name is: " + interfaceCategory.CategoryType, LogLevel.VERBOSE);

            if (interfaceCategory == null)
            {
                Log.WriteLine(nameof(interfaceCategory).ToString() +
                    " was null!", LogLevel.CRITICAL);
                return;
            }

            SocketCategoryChannel? socketCategoryChannel = null;

            bool contains = false;
            Log.WriteLine("searching for categoryname: " + interfaceCategory.CategoryType, LogLevel.VERBOSE);
            foreach (var ct in Database.Instance.Categories.CreatedCategoriesWithChannels)
            {
                Log.WriteLine("categoryname:" + ct.Value.CategoryType, LogLevel.VERBOSE);
                if (ct.Value.CategoryType == interfaceCategory.CategoryType)
                {
                    // Checks if the channel is also in the discord server itself too, not only database
                    contains = CategoryRestore.CheckIfCategoryHasBeenDeletedAndRestoreForCategory(ct.Key, _guild);
                    break;
                }
            }

            SocketRole? role = RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
                        _guild, finalCategoryName).Result;

            // The category exists,
            // just find it from the database and then get the id of the socketchannel
            if (contains)
            {
                Log.WriteLine("Category: " + finalCategoryName +
                    " found, checking it's channels", LogLevel.VERBOSE);


                InterfaceCategory? dbCategory =
                    Database.Instance.Categories.FindInterfaceCategoryByCategoryName(
                        interfaceCategory.CategoryType);

                Log.WriteLine("Found " + nameof(dbCategory) + " named: " +
                    dbCategory.CategoryType, LogLevel.VERBOSE);

                interfaceCategory = dbCategory as InterfaceCategory;

                socketCategoryChannel = _guild.GetCategoryChannel(dbCategory.SocketCategoryChannelId);

                // Insert a fix here if the category is still in DB but does not exist

                Log.WriteLine("Found " + nameof(socketCategoryChannel) + " that's named: " +
                    socketCategoryChannel.Name, LogLevel.DEBUG);

            }
            // If the category doesn't exist at all, create it and add it to the database
            else
            {

                socketCategoryChannel =
                    await interfaceCategory.CreateANewSocketCategoryChannelAndReturnIt(
                        _guild, finalCategoryName, role);

                Log.WriteLine("Created a " + nameof(socketCategoryChannel) +
                    " with id: " + socketCategoryChannel.Id +
                    " that's named: " + socketCategoryChannel.Name, LogLevel.VERBOSE);

                Database.Instance.Categories.AddToCreatedCategoryWithChannelWithUlongAndInterfaceCategory(
                    socketCategoryChannel.Id, interfaceCategory);
            }

            // Handle channel checking/creation
            await interfaceCategory.CreateChannelsForTheCategory(socketCategoryChannel.Id, _client, role);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }
    }

    // Maybe add inside the classes itself
    private static InterfaceCategory GetCategoryInstance(CategoryType _categoryName)
    {
        try
        {
            return (InterfaceCategory)EnumExtensions.GetInstance(_categoryName.ToString());
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            throw new InvalidOperationException(ex.Message);
        }
    }

    private static InterfaceLeague GetLeagueInstance(LeagueName _leagueCategoryName)
    {
        try
        {
            return (InterfaceLeague)EnumExtensions.GetInstance(_leagueCategoryName.ToString());
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            throw new InvalidOperationException(ex.Message);
        }
    }
}