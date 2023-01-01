using Discord;
using Discord.WebSocket;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Channels;

public static class CategoryAndChannelManager
{
    // Do not create these categories,
    // as they are used as template (such as generating a baseline league)
    private static List<CategoryName> categoriesThatWontGetGenerated = new List<CategoryName> {
        CategoryName.LEAGUETEMPLATE };

    public static async Task CreateCategoriesAndChannelsForTheDiscordServer()
    {
        Log.WriteLine("Starting to create categories and channels for" +
            " the discord server", LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        // Get the values of the members of the specific enum type and
        // loop through the values of the categories
        var values = Enum.GetValues(typeof(CategoryName));
        Log.WriteLine(nameof(values) + " count: " + values.Length, LogLevel.VERBOSE);
        foreach (CategoryName categoryName in values)
        {
            Log.WriteLine("Looping on category name: " + categoryName, LogLevel.VERBOSE);

            // Skip creating from the default LeagueTemplate
            if (categoriesThatWontGetGenerated.Contains(categoryName))
            {
                Log.WriteLine("category name is: " + categoryName +
                    " skipping creation for this type of category", LogLevel.VERBOSE);
                continue;
            }

            await GenerateACategoryFromName(guild, categoryName);
        }

        Log.WriteLine("Done looping through the category names, serialiazing.", LogLevel.VERBOSE);
        await SerializationManager.SerializeDB();
    }

    private static async Task GenerateACategoryFromName(
        SocketGuild _guild, CategoryName _categoryName)
    {
        string finalCategoryName = "";
        bool isLeague = false;
        CategoryName? leagueCategoryName = null;
        InterfaceCategory? interfaceCategory = null;

        Log.WriteLine("Generating category named: " + _categoryName, LogLevel.VERBOSE);

        // For league category generating
        if (Database.Instance.Leagues.CheckIfILeagueExistsByCategoryName(_categoryName))
        {
            Log.WriteLine("This is a league category", LogLevel.DEBUG);

            isLeague = true;

            InterfaceLeague leagueInterface = GetLeagueInstance(_categoryName);

            Log.WriteLine("Got " + nameof(leagueInterface) +
                leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

            interfaceCategory = GetCategoryInstance(CategoryName.LEAGUETEMPLATE);
            interfaceCategory.CategoryName = _categoryName;

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
                interfaceCategory.CategoryName, LogLevel.DEBUG);

            Log.WriteLine(nameof(interfaceCategory.CategoryName) +
                ": " + interfaceCategory.CategoryName, LogLevel.VERBOSE);

            finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(interfaceCategory.CategoryName);
            Log.WriteLine("Category name is: " + interfaceCategory.CategoryName, LogLevel.VERBOSE);
        }


        if (interfaceCategory == null)
        {
            Log.WriteLine(nameof(interfaceCategory).ToString() +
                " was null!", LogLevel.CRITICAL);
            return;
        }


        SocketCategoryChannel? socketCategoryChannel = null;

        bool contains = false;
        Log.WriteLine("searching for categoryname: " + interfaceCategory.CategoryName, LogLevel.VERBOSE);
        foreach (var ct in Database.Instance.Categories.CreatedCategoriesWithChannels)
        {
            Log.WriteLine("categoryname:" + ct.Value.CategoryName, LogLevel.VERBOSE);
            if (ct.Value.CategoryName == interfaceCategory.CategoryName)
            {
                // Checks if the channel is also in the discord server itself too, not only database
                contains = CategoryRestore.CheckIfCategoryHasBeenDeletedAndRestoreForCategory(
                    ct, _guild).Result;
                break;
            }
        }

        // The category exists,
        // just find it from the database and then get the id of the socketchannel
        if (contains)
        {
            Log.WriteLine("Category: " + finalCategoryName +
                " found, checking it's channels", LogLevel.VERBOSE);

            var dbCategory =
                Database.Instance.Categories.GetCreatedCategoryWithChannelKvpByCategoryName(
                    interfaceCategory.CategoryName);

            interfaceCategory = dbCategory.Value;

            ////InterfaceCategory databaseInterfaceCategory = GetCategoryInstance(buttonName);
            if (dbCategory.Key == 0 || dbCategory.Value == null)
            {
                Log.WriteLine(nameof(dbCategory) + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Found " + nameof(dbCategory) + " with id: " +
                dbCategory.Key + " named: " +
                dbCategory.Value.CategoryName, LogLevel.VERBOSE);

            socketCategoryChannel = _guild.GetCategoryChannel(dbCategory.Key);

            // Insert a fix here if the category is still in DB but does not exist

            Log.WriteLine("Found " + nameof(socketCategoryChannel) + " that's named: " +
                socketCategoryChannel.Name, LogLevel.DEBUG);
        }
        // If the category doesn't exist at all, create it and add it to the database
        else
        {
            SocketRole role =
                RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
                    _guild, finalCategoryName).Result;

            socketCategoryChannel =
                await interfaceCategory.CreateANewSocketCategoryChannelAndReturnIt(
                    _guild, finalCategoryName, role);
            if (socketCategoryChannel == null)
            {
                Log.WriteLine(nameof(socketCategoryChannel) + " was null!", LogLevel.CRITICAL);
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
                Database.Instance.Leagues.GetILeagueByCategoryName(leagueCategoryName).
                        DiscordLeagueReferences.LeagueCategoryId = socketCategoryChannel.Id;
            }

            Database.Instance.Categories.AddToCreatedCategoryWithChannelWithUlongAndInterfaceCategory(
                socketCategoryChannel.Id, interfaceCategory);
        }

        // Handle channel checking/creation
        await interfaceCategory.CreateChannelsForTheCategory(
            interfaceCategory, socketCategoryChannel.Id, _guild);
    }

    // Maybe add inside the classes itself
    private static InterfaceCategory GetCategoryInstance(CategoryName _categoryName)
    {
        return (InterfaceCategory)EnumExtensions.GetInstance(_categoryName.ToString());
    }

    private static InterfaceLeague GetLeagueInstance(CategoryName _leagueCategoryName)
    {
        return (InterfaceLeague)EnumExtensions.GetInstance(_leagueCategoryName.ToString());
    }
}