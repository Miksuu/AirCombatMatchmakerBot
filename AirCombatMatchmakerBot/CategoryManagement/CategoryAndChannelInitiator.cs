using Discord;
using Discord.WebSocket;
using System;
using System.Diagnostics.Metrics;
using System.Threading.Channels;

public static class CategoryAndChannelInitiator
{
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

        await GenerateACategoryType(guild);

        await SerializationManager.SerializeDB();
    }

    public static async Task GenerateACategoryType(SocketGuild _guild)
    {
        // Get the names of the members of the specific enum type and loop through the names of the categories
        var names = Enum.GetNames(typeof(CategoryName));
        foreach (string categoryName in names)
        {
            // Skip creating from the default LeagueTemplate
            if (categoryName == "LEAGUETEMPLATE") continue;

            Log.WriteLine("Generating category named: " + categoryName, LogLevel.VERBOSE);
            await GenerateACategoryFromName(_guild, categoryName);
        }
    }

    public static async Task GenerateACategoryFromName(
        SocketGuild _guild, string _categoryName)
    {
        string finalCategoryName = "";

        bool isLeague = false;
        CategoryName? leagueCategoryName = null;

        Log.WriteLine("Generating: " + _categoryName.ToString(), LogLevel.DEBUG);

        InterfaceCategory? interfaceCategory = null;
        BaseCategory? baseCategory = null;

        // For league category generating
        if (Database.Instance.StoredLeagues.Any(x => x.LeagueCategoryName.ToString() == _categoryName))
        {
            Log.WriteLine("This is a league category", LogLevel.DEBUG);

            isLeague = true;

            ILeague leagueInterface = GetLeagueInstance(_categoryName);

            baseCategory = new LEAGUETEMPLATE();
            baseCategory.categoryName = leagueInterface.LeagueCategoryName;

            // Cached for later use (inserting the category ID)
            leagueCategoryName = leagueInterface.LeagueCategoryName;

            finalCategoryName = baseCategory.categoryName.ToString();
            finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(baseCategory.categoryName);

            Log.WriteLine("League's category name is: " + finalCategoryName, LogLevel.VERBOSE);
        }
        // For normal category generating
        else
        {
            interfaceCategory = GetCategoryInstance(_categoryName);

            if (interfaceCategory == null)
            {
                Log.WriteLine(nameof(interfaceCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("interfaceCategory name: " + interfaceCategory.CategoryName, LogLevel.DEBUG);

            baseCategory = interfaceCategory as BaseCategory;
            if (baseCategory == null)
            {
                Log.WriteLine(nameof(baseCategory) + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine(nameof(baseCategory.categoryName) + ": " + baseCategory.categoryName, LogLevel.VERBOSE);

            finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(baseCategory.categoryName);
            Log.WriteLine("Category name is: " + baseCategory.categoryName, LogLevel.VERBOSE);
        }

        List<Overwrite> permissionsList = baseCategory.GetGuildPermissions(_guild);
        SocketCategoryChannel? socketCategoryChannel = null;

        bool contains = false;
        Log.WriteLine("searching for categoryname: " + baseCategory.categoryName, LogLevel.VERBOSE);
        foreach (var ct in Database.Instance.CreatedCategoriesWithChannels)
        {
            Log.WriteLine("categoryname:" + ct.Value.CategoryName, LogLevel.VERBOSE);
            if (ct.Value.CategoryName == baseCategory.categoryName)
            {
                // Checks if the channel is also in the discord server itself too, not only database
                contains = CategoryRestore.CheckIfCategoryHasBeenDeletedAndRestoreForCategory(ct, _guild).Result;
                break;
            }
        }

        // The category exists, just find it from the database and then get the id of the socketchannel
        if (contains)
        {
            Log.WriteLine("Category: " + finalCategoryName + " found, checking it's channels", LogLevel.VERBOSE);

            var dbCategory = Database.Instance.CreatedCategoriesWithChannels.First(
                x => x.Value.CategoryName == baseCategory.categoryName);

            //InterfaceCategory databaseInterfaceCategory = GetCategoryInstance(categoryName);
            if (dbCategory.Key == 0 || dbCategory.Value == null)
            {
                Log.WriteLine(nameof(dbCategory) + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Found " + nameof(dbCategory) + " with id: " +
                dbCategory.Key + " named: " +
                dbCategory.Value.CategoryName, LogLevel.VERBOSE);

            // Needs to be inserted in to the basecategory, otherwise on channel generation some data won't show
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

            Log.WriteLine("Created a " + nameof(socketCategoryChannel) + " with id: " + socketCategoryChannel.Id +
                " that's named: " + socketCategoryChannel.Name, LogLevel.VERBOSE);

            // Inserts the id to the discord league references to be cached for later use
            if (isLeague)
            {
                Log.WriteLine("Is league, inserting " + socketCategoryChannel.Id +
                    " to " + leagueCategoryName, LogLevel.DEBUG);
                Database.Instance.StoredLeagues.First(
                    x => x.LeagueCategoryName == leagueCategoryName).
                        DiscordLeagueReferences.leagueCategoryId = socketCategoryChannel.Id;
            }

            Log.WriteLine("Adding " + nameof(baseCategory) + " to " +
                nameof(Database.Instance.CreatedCategoriesWithChannels), LogLevel.VERBOSE);

            Database.Instance.CreatedCategoriesWithChannels.Add(socketCategoryChannel.Id, baseCategory);

            Log.WriteLine("Done adding " + nameof(baseCategory) + " to " +
                nameof(Database.Instance.CreatedCategoriesWithChannels), LogLevel.DEBUG);
        }

        if (baseCategory == null)
        {
            Log.WriteLine(nameof(baseCategory) + " was null!", LogLevel.CRITICAL);
            return;
        }

        // Handle channel checking/creation
        await CreateChannelsForTheCategory(baseCategory, socketCategoryChannel, _guild);
    }

    public static async Task CreateChannelsForTheCategory(
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
            Log.WriteLine("interfaceChanneltest: " + interfaceChannel.ChannelName.ToString(), LogLevel.DEBUG);
            
            baseChannel = interfaceChannel as BaseChannel;

            if (baseChannel == null)
            {
                Log.WriteLine(nameof(baseChannel) + " was null!", LogLevel.CRITICAL);
                return;
            }

            // Channel found from the basecategory (it exists)
            if (_baseCategory.interfaceChannels.Any(x => x.ChannelName == baseChannel.channelName))
            {
                Log.WriteLine(nameof(_baseCategory.interfaceChannels) + " already contains channel: " +
                    channelName.ToString(), LogLevel.VERBOSE);

                // Replace interfaceChannel with a one that is from the database
                interfaceChannel = _baseCategory.interfaceChannels.First(x => x.ChannelName == channelName);

                Log.WriteLine("Replaced with: " + interfaceChannel.ChannelName + " from db", LogLevel.DEBUG);

                channelExists = await ChannelRestore.CheckIfChannelHasBeenDeletedAndRestoreForCategory(
                  _socketCategoryChannel.Id, interfaceChannel, _guild);
            }
            else
            {
                Log.WriteLine(nameof(_baseCategory.interfaceChannels) + " does not contain channel: " +
                    channelName.ToString() + ", getting instance of it", LogLevel.VERBOSE);
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
            baseChannel.channelsCategoryId = _socketCategoryChannel.Id;

            string? channelNameString = EnumExtensions.GetEnumMemberAttrValue(channelName);
            if (channelNameString == null)
            {
                Log.WriteLine(nameof(channelNameString).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            if (!channelExists)
            {
                List<Overwrite> permissionsList = baseChannel.GetGuildPermissions(_guild);

                ulong categoryId = Database.Instance.CreatedCategoriesWithChannels.First(
                     x => x.Value.CategoryName == _baseCategory.categoryName).Key;

                Log.WriteLine("Creating a channel named: " + channelNameString + " for category: "
                            + _baseCategory.categoryName + " (" + _socketCategoryChannel.Id + ")", LogLevel.VERBOSE);

                baseChannel.channelId = await ChannelManager.CreateAChannelForTheCategory(
                    _guild, channelNameString, _socketCategoryChannel.Id, permissionsList);

                Log.WriteLine("Done creating the channel with id: " + interfaceChannel.ChannelId +
                    " named:" + channelNameString + " adding it to the db.", LogLevel.DEBUG);

                _baseCategory.interfaceChannels.Add(baseChannel);

                Log.WriteLine("Done adding to the db. Count is now: " + _baseCategory.interfaceChannels.Count +
                    " for the list of category: " + _baseCategory.categoryName.ToString() +
                    " (" + _socketCategoryChannel.Id + ")", LogLevel.VERBOSE);
            }

            await baseChannel.ActivateChannelFeatures();

            Log.WriteLine("Done looping through: " + channelNameString, LogLevel.VERBOSE);
        }
    }

    public static InterfaceCategory GetCategoryInstance(string _categoryName)
    {
        return (InterfaceCategory)EnumExtensions.GetInstance(_categoryName);
    }

    public static ILeague GetLeagueInstance(string _leagueName)
    {
        return (ILeague)EnumExtensions.GetInstance(_leagueName);
    }

    public static InterfaceChannel GetChannelInstance(string _channelName)
    {
        return (InterfaceChannel)EnumExtensions.GetInstance(_channelName);
    }
}