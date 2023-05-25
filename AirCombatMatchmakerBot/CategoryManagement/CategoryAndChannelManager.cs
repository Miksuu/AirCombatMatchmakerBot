using Discord.WebSocket;
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

        // Get the values of the members of the specific enum type and
        // loop through the values of the categories
        var values = Enum.GetValues(typeof(CategoryType));
        Log.WriteLine(nameof(values) + " count: " + values.Length, LogLevel.VERBOSE);
        foreach (CategoryType categoryName in values)
        {
            Log.WriteLine("Looping on category name: " + categoryName, LogLevel.VERBOSE);

            // Skip creating from the default LeagueTemplate
            if (categoriesThatWontGetGenerated.Contains(categoryName))
            {
                Log.WriteLine("category name is: " + categoryName +
                    " skipping creation for this type of category", LogLevel.VERBOSE);
                continue;
            }

            await GenerateACategoryFromName(client, categoryName);
        }

        Log.WriteLine("Done looping through the category names, serialiazing.",
            LogLevel.VERBOSE);
        //await SerializationManager.SerializeDB();
    }

    // Sort out this spaghetti
    private static async Task GenerateACategoryFromName(
        DiscordSocketClient _client, CategoryType _categoryName)
    {
        string finalCategoryName = "";
        bool isLeague = false;
        CategoryType? leagueCategoryName = null;
        InterfaceCategory? interfaceCategory = null;

        Log.WriteLine("Generating category named: " + _categoryName, LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        // For league category generating
        if (Database.Instance.Leagues.CheckIfILeagueExistsByCategoryName(_categoryName))
        {
            Log.WriteLine("This is a league category", LogLevel.DEBUG);

            isLeague = true;

            InterfaceLeague leagueInterface = GetLeagueInstance(_categoryName);

            Log.WriteLine("Got " + nameof(leagueInterface) +
                leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

            interfaceCategory = GetCategoryInstance(CategoryType.LEAGUETEMPLATE);
            interfaceCategory.CategoryType = _categoryName;

            // Cached for later use (inserting the category ID)
            leagueCategoryName = leagueInterface.LeagueCategoryName;

            finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(
                leagueInterface.LeagueCategoryName);

            Log.WriteLine("League's category name is: " + finalCategoryName, LogLevel.VERBOSE);
        }
        // For normal category generating
        else
        {
            interfaceCategory = GetCategoryInstance(_categoryName);

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

            finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(interfaceCategory.CategoryType);
            Log.WriteLine("Category name is: " + interfaceCategory.CategoryType, LogLevel.VERBOSE);
        }

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
                contains = CategoryRestore.CheckIfCategoryHasBeenDeletedAndRestoreForCategory(
                    ct, guild);
                break;
            }
        }

        SocketRole? role = RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
                    guild, finalCategoryName).Result;

        // The category exists,
        // just find it from the database and then get the id of the socketchannel
        if (contains)
        {
            Log.WriteLine("Category: " + finalCategoryName +
                " found, checking it's channels", LogLevel.VERBOSE);

            try
            {
                InterfaceCategory? dbCategory =
                    Database.Instance.Categories.FindInterfaceCategoryByCategoryName(
                        interfaceCategory.CategoryType);

                Log.WriteLine("Found " + nameof(dbCategory) + " named: " +
                    dbCategory.CategoryType, LogLevel.VERBOSE);

                socketCategoryChannel = guild.GetCategoryChannel(dbCategory.SocketCategoryChannelId);

                // Insert a fix here if the category is still in DB but does not exist

                Log.WriteLine("Found " + nameof(socketCategoryChannel) + " that's named: " +
                    socketCategoryChannel.Name, LogLevel.DEBUG);
            }
            catch (Exception ex) 
            {
                Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                return;
            }
        }
        // If the category doesn't exist at all, create it and add it to the database
        else
        {
            try
            {
                socketCategoryChannel =
                    await interfaceCategory.CreateANewSocketCategoryChannelAndReturnIt(
                        guild, finalCategoryName, role);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Created a " + nameof(socketCategoryChannel) +
                " with id: " + socketCategoryChannel.Id +
                " that's named: " + socketCategoryChannel.Name, LogLevel.VERBOSE);

            // Inserts the id to the discord league references to be cached for later use
            if (isLeague)
            {
                Log.WriteLine("Is league, inserting " + socketCategoryChannel.Id +
                    " to " + leagueCategoryName, LogLevel.DEBUG);

                CategoryType categoryTypeNonNullable = (CategoryType)leagueCategoryName;

                InterfaceLeague interfaceLeague;
                try
                {
                    interfaceLeague = Database.Instance.Leagues.GetILeagueByCategoryName(
                        categoryTypeNonNullable);
                    interfaceLeague.LeagueCategoryId = socketCategoryChannel.Id;
                }
                catch (Exception ex) 
                {
                    Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                    return;
                }
            }

            Database.Instance.Categories.AddToCreatedCategoryWithChannelWithUlongAndInterfaceCategory(
                socketCategoryChannel.Id, interfaceCategory);
        }

        /*
        if (role == null)
        {
            Log.WriteLine(nameof(role) + " was null!", LogLevel.CRITICAL);
            return;
        }*/

        // Handle channel checking/creation
        await interfaceCategory.CreateChannelsForTheCategory(socketCategoryChannel.Id, _client, role);
    }

    // Maybe add inside the classes itself
    private static InterfaceCategory GetCategoryInstance(CategoryType _categoryName)
    {
        return (InterfaceCategory)EnumExtensions.GetInstance(_categoryName.ToString());
    }

    private static InterfaceLeague GetLeagueInstance(CategoryType _leagueCategoryName)
    {
        return (InterfaceLeague)EnumExtensions.GetInstance(_leagueCategoryName.ToString());
    }
}