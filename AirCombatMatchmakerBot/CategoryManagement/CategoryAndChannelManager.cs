using Discord;
using Discord.WebSocket;
using System;
using System.Diagnostics.Metrics;
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
        BaseCategory? baseCategory = null;

        Log.WriteLine("Generating category named: " + _categoryName, LogLevel.VERBOSE);

        // For league category generating
        if (Database.Instance.Leagues.CheckIfILeagueExistsByCategoryName(_categoryName))
        {
            Log.WriteLine("This is a league category", LogLevel.DEBUG);

            isLeague = true;

            ILeague leagueInterface = GetLeagueInstance(_categoryName);

            Log.WriteLine("Got " + nameof(leagueInterface) +
                leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

            baseCategory = new LEAGUETEMPLATE();
            baseCategory.categoryName = leagueInterface.LeagueCategoryName;

            // Cached for later use (inserting the category ID)
            leagueCategoryName = leagueInterface.LeagueCategoryName;

            //finalCategoryName = baseCategory.categoryName.ToString();
            finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(baseCategory.categoryName);

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

            baseCategory = interfaceCategory as BaseCategory;
            if (baseCategory == null)
            {
                Log.WriteLine(nameof(baseCategory) + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine(nameof(baseCategory.categoryName) +
                ": " + baseCategory.categoryName, LogLevel.VERBOSE);

            finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(baseCategory.categoryName);
            Log.WriteLine("Category name is: " + baseCategory.categoryName, LogLevel.VERBOSE);
        }

        List<Overwrite> permissionsList = baseCategory.GetGuildPermissions(_guild);
        SocketCategoryChannel? socketCategoryChannel = null;

        bool contains = false;
        Log.WriteLine("searching for categoryname: " + baseCategory.categoryName, LogLevel.VERBOSE);
        foreach (var ct in Database.Instance.Categories.GetDictionaryOfCreatedCategoriesWithChannels())
        {
            Log.WriteLine("categoryname:" + ct.Value.CategoryName, LogLevel.VERBOSE);
            if (ct.Value.CategoryName == baseCategory.categoryName)
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


            // param: baseCategory.categoryName
            var dbCategory =
                Database.Instance.Categories.GetCreatedCategoryWithChannelKvpByCategoryName(
                    baseCategory.categoryName);

            //InterfaceCategory databaseInterfaceCategory = GetCategoryInstance(categoryName);
            if (dbCategory.Key == 0 || dbCategory.Value == null)
            {
                Log.WriteLine(nameof(dbCategory) + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Found " + nameof(dbCategory) + " with id: " +
                dbCategory.Key + " named: " +
                dbCategory.Value.CategoryName, LogLevel.VERBOSE);

            // Needs to be inserted in to the basecategory,
            // otherwise on channel generation some data won't show
            baseCategory = dbCategory.Value as BaseCategory;

            socketCategoryChannel = _guild.GetCategoryChannel(dbCategory.Key);

            // Insert a fix here if the category is still in DB but does not exist

            Log.WriteLine("Found " + nameof(socketCategoryChannel) + " that's named: " +
                socketCategoryChannel.Name, LogLevel.DEBUG);
        }
        // If the category doesn't exist at all, create it and add it to the database
        else
        {
            socketCategoryChannel =
                await CategoryManager.CreateANewSocketCategoryChannelAndReturnIt(
                    _guild, finalCategoryName, permissionsList);
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
                        DiscordLeagueReferences.SetLeagueCategoryId(socketCategoryChannel.Id);
            }

            Database.Instance.Categories.AddToCreatedCategoryWithChannelWithUlongAndBaseCategory(
                socketCategoryChannel.Id, baseCategory);
        }

        if (baseCategory == null)
        {
            Log.WriteLine(nameof(baseCategory) + " was null!", LogLevel.CRITICAL);
            return;
        }

        // Handle channel checking/creation
        await CreateChannelsForTheCategory(baseCategory, socketCategoryChannel, _guild);
    }

    private static async Task CreateChannelsForTheCategory(
        BaseCategory _baseCategory,
        SocketCategoryChannel _socketCategoryChannel,
        SocketGuild _guild)
    {
        Log.WriteLine("Starting to create channels for: " + _socketCategoryChannel.Name +
            " ( " + _socketCategoryChannel.Id + ")" + " Channel count: " +
            _baseCategory.channelNames.Count, LogLevel.VERBOSE);

        Log.WriteLine("Found " + nameof(_baseCategory.interfaceChannels)
            + " channel counts: " + _baseCategory.interfaceChannels.Count + " and: " +
            _baseCategory.channelNames.Count, LogLevel.VERBOSE);

        foreach (ChannelName channelName in _baseCategory.channelNames)
        {
            Log.WriteLine("Looping with channel name: " + channelName, LogLevel.DEBUG);

            bool channelExists = false;
            InterfaceChannel? interfaceChannel = null;
            BaseChannel? baseChannel = null;

            interfaceChannel = GetChannelInstance(channelName.ToString());
            Log.WriteLine("interfaceChanneltest: " +
                interfaceChannel.ChannelName.ToString(), LogLevel.DEBUG);
            
            baseChannel = interfaceChannel as BaseChannel;

            if (baseChannel == null)
            {
                Log.WriteLine(nameof(baseChannel) + " was null!", LogLevel.CRITICAL);
                return;
            }

            // Channel found from the basecategory (it exists)
            if (_baseCategory.interfaceChannels.Any(x => x.ChannelName == interfaceChannel.ChannelName))
            {
                Log.WriteLine(nameof(_baseCategory.interfaceChannels) + " already contains channel: " +
                    channelName.ToString(), LogLevel.VERBOSE);

                // Replace interfaceChannel with a one that is from the database
                interfaceChannel = _baseCategory.interfaceChannels.First(
                    x => x.ChannelName == channelName);

                Log.WriteLine("Replaced with: " +
                    interfaceChannel.ChannelName + " from db", LogLevel.DEBUG);

                channelExists = 
                   await ChannelRestore.CheckIfChannelHasBeenDeletedAndRestoreForCategory(
                  _socketCategoryChannel.Id, interfaceChannel, _guild);
            }
            else
            {
                Log.WriteLine(nameof(_baseCategory.interfaceChannels) +
                    " does not contain channel: " + channelName.ToString() +
                    ", getting instance of it", LogLevel.VERBOSE);
                interfaceChannel = GetChannelInstance(channelName.ToString());
            }

            baseChannel = interfaceChannel as BaseChannel;

            if (baseChannel == null)
            {
                Log.WriteLine(nameof(baseChannel) + " was null!", LogLevel.CRITICAL);
                return;
            }

            // Insert the category's ID for easier access for the channels later on
            // (for channel specific features for example)
            interfaceChannel.ChannelsCategoryId = _socketCategoryChannel.Id;

            string? channelNameString = EnumExtensions.GetEnumMemberAttrValue(channelName);
            if (channelNameString == null)
            {
                Log.WriteLine(nameof(channelNameString).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            if (!channelExists)
            {
                List<Overwrite> permissionsList = baseChannel.GetGuildPermissions(_guild);

                Log.WriteLine("Creating a channel named: " + channelNameString + " for category: "
                             + _baseCategory.categoryName + " (" +
                             _socketCategoryChannel.Id + ")", LogLevel.VERBOSE);

                ulong categoryId =
                    Database.Instance.Categories.GetCreatedCategoryWithChannelKvpByCategoryName(
                        _baseCategory.categoryName).Key;

                interfaceChannel.ChannelId = await ChannelManager.CreateAChannelForTheCategory(
                    _guild, channelNameString, _socketCategoryChannel.Id, permissionsList);

                Log.WriteLine("Done creating the channel with id: " + interfaceChannel.ChannelId +
                    " named:" + channelNameString + " adding it to the db.", LogLevel.DEBUG);

                _baseCategory.interfaceChannels.Add(baseChannel);

                Log.WriteLine("Done adding to the db. Count is now: " +
                    _baseCategory.interfaceChannels.Count +
                    " for the list of category: " + _baseCategory.categoryName.ToString() +
                    " (" + _socketCategoryChannel.Id + ")", LogLevel.VERBOSE);
            }

            await baseChannel.ActivateChannelFeatures();

            Log.WriteLine("Done looping through: " + channelNameString, LogLevel.VERBOSE);
        }
    }

    // Maybe add inside the classes itself
    private static InterfaceCategory GetCategoryInstance(CategoryName _categoryName)
    {
        return (InterfaceCategory)EnumExtensions.GetInstance(_categoryName.ToString());
    }

    private static ILeague GetLeagueInstance(CategoryName _leagueCategoryName)
    {
        return (ILeague)EnumExtensions.GetInstance(_leagueCategoryName.ToString());
    }

    private static InterfaceChannel GetChannelInstance(string _channelName)
    {
        return (InterfaceChannel)EnumExtensions.GetInstance(_channelName);
    }
}