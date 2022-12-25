using Discord;
using Discord.WebSocket;
using System;
using System.Diagnostics.Metrics;

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
        bool categoryExists = false;
        //string leagueNameCached = "";
        string finalCategoryName = "";

        Log.WriteLine("Generating: " + _categoryName.ToString(), LogLevel.DEBUG);


        /*
        // Make a LeagueTemplate if the _type == typeof(LeagueCategoryName)
        if (_isLeagueCategory)
        {
            leagueNameCached = EnumExtensions.GetEnumMemberAttrValue(GetLeagueInstance(_categoryName).LeagueCategoryName);
            Log.WriteLine("leagueNameCached: " + leagueNameCached, LogLevel.VERBOSE);

            _categoryName = "LEAGUETEMPLATE";
        } */

        InterfaceCategory interfaceCategory = null;

        /*
        if (Database.Instance.StoredLeagues.Any(x=>x.LeagueCategoryName == interfaceCategory.CategoryName))
        {
            var searchingWith = Database.Instance.StoredLeagues.First(
                x => x.LeagueCategoryName.ToString() == interfaceCategory.CategoryName.ToString()).LeagueCategoryName;

            Log.WriteLine("searching with: " + searchingWith, LogLevel.WARNING);

            finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(searchingWith);
        }
        else
        {
            finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(interfaceCategory.CategoryName);
        }*/

        BaseCategory baseCategory = null;
        if (Database.Instance.StoredLeagues.Any(x => x.LeagueCategoryName.ToString() == _categoryName))
        {
            Log.WriteLine("This is a league category", LogLevel.DEBUG);

            ILeague leagueInterface = GetLeagueInstance(_categoryName);

            baseCategory = new LEAGUETEMPLATE();
            baseCategory.categoryName = leagueInterface.LeagueCategoryName;

            finalCategoryName = baseCategory.categoryName.ToString();
            finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(baseCategory.categoryName);

            Log.WriteLine("League's category name is: " + finalCategoryName, LogLevel.VERBOSE);
        }
        else
        {
            interfaceCategory = GetCategoryInstance(_categoryName);

            baseCategory = interfaceCategory as BaseCategory;

            if (interfaceCategory == null)
            {
                Log.WriteLine(nameof(interfaceCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("interfaceCategory name: " + interfaceCategory.CategoryName, LogLevel.DEBUG);

            finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(baseCategory.categoryName);
            Log.WriteLine("Category name is: " + baseCategory.categoryName, LogLevel.VERBOSE);
        }

        /*
        if (_isLeagueCategory)
        {
            finalCategoryName = leagueNameCached;
        }
        else
        {
            finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(interfaceCategory.CategoryName);
        }*/


        //finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(interfaceCategory.CategoryName);

        /*
        if (Database.Instance.CreatedCategoriesWithChannels.Any(
            x => x.Value.CategoryName.ToString() == _categoryName))
        {
            // Replace InterfaceLeagueCategoryCategory with a one that is from the database
            var interfaceCategoryKvp = Database.Instance.CreatedCategoriesWithChannels.First(
                x => x.Value.CategoryName == interfaceCategory.CategoryName);
            interfaceCategory = interfaceCategoryKvp.Value;

            Log.WriteLine("Replaced with: " + interfaceCategory.CategoryName + " from db", LogLevel.DEBUG);

            categoryExists = await CategoryRestore.CheckIfCategoryHasBeenDeletedAndRestoreForCategory(
                interfaceCategoryKvp, _guild);
        }*/


        /*
     if (categoryNameString == null)
     {
         Log.WriteLine("categoryNameString was null!", LogLevel.CRITICAL);
         return;
     }

     Log.WriteLine("Creating a category named: " + categoryNameString, LogLevel.VERBOSE);*/

        //BaseCategory baseCategory = interfaceCategory as BaseCategory;


        List<Overwrite> permissionsList = baseCategory.GetGuildPermissions(_guild);

        SocketCategoryChannel? socketCategoryChannel = null;

        // If the category doesn't exist at all, create it and add it to the database
        if (!categoryExists)
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

            Log.WriteLine("Adding " + nameof(interfaceCategory) + " to " +
                nameof(Database.Instance.CreatedCategoriesWithChannels), LogLevel.VERBOSE);

            Database.Instance.CreatedCategoriesWithChannels.Add(socketCategoryChannel.Id, baseCategory);

            Log.WriteLine("Done adding " + nameof(interfaceCategory) + " to " +
                nameof(Database.Instance.CreatedCategoriesWithChannels), LogLevel.DEBUG);
        }
        // The category exists, just find it from the database and then get the id of the socketchannel
        else
        {
            var dbCategory = Database.Instance.CreatedCategoriesWithChannels.First(
                x => x.Value.CategoryName == interfaceCategory.CategoryName);

            //InterfaceCategory databaseInterfaceCategory = GetCategoryInstance(categoryName);
            if (dbCategory.Key == 0 || dbCategory.Value == null)
            {
                Log.WriteLine(nameof(dbCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Found " + nameof(dbCategory) + " with id: " +
                dbCategory.Key + " named: " +
                dbCategory.Value.CategoryName, LogLevel.VERBOSE);

            socketCategoryChannel = _guild.GetCategoryChannel(dbCategory.Key);

            Log.WriteLine("Found " + nameof(socketCategoryChannel) + " that's named: " +
                socketCategoryChannel.Name, LogLevel.DEBUG);
        }

        //await CreateChannelsForTheCategory(interfaceCategory, socketCategoryChannel, _guild);

        /*
        if (Database.Instance.CreatedCategoriesWithChannels.Any(x => x.Value.CategoryName == interfaceCategory.CategoryName))
        {


        }*/
    }



    public static async Task CreateChannelsForTheCategory(
        InterfaceCategory _interfaceCategory,
        SocketCategoryChannel _socketCategoryChannel,
        SocketGuild _guild)
    {
        Log.WriteLine("Starting to create channels for: " + _socketCategoryChannel.Name +
            " ( " + _socketCategoryChannel.Id + ")" + " Channel count: " +
            _interfaceCategory.ChannelNames.Count, LogLevel.VERBOSE);

        List<InterfaceChannel> channelListForCategory = Database.Instance.CreatedCategoriesWithChannels.First(
            x => x.Value.CategoryName == _interfaceCategory.CategoryName).Value.InterfaceChannels;
        if (channelListForCategory == null)
        {
            Log.WriteLine(nameof(channelListForCategory) + " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Found " + nameof(channelListForCategory)
            + " channel count: " + channelListForCategory.Count, LogLevel.VERBOSE);

        foreach (ChannelName channelName in _interfaceCategory.ChannelNames)
        {
            bool channelExists = false;

            if (channelName == null)
            {
                Log.WriteLine("channelName was null1", LogLevel.CRITICAL);
                return;
            }

            InterfaceChannel interfaceChannel = GetChannelInstance(channelName.ToString());
            if (interfaceChannel == null)
            {
                Log.WriteLine(nameof(interfaceChannel).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }
            else { }

            if (channelListForCategory.Any(x => x.ChannelName == channelName))
            {
                Log.WriteLine(nameof(channelListForCategory) + " already contains channel: " +
                    channelName.ToString(), LogLevel.VERBOSE);

                // Replace interfaceChannel with a one that is from the database
                interfaceChannel = channelListForCategory.First(x => x.ChannelName == channelName);

                Log.WriteLine("Replaced with: " + interfaceChannel.ChannelName + " from db", LogLevel.DEBUG);

                channelExists = await ChannelRestore.CheckIfChannelHasBeenDeletedAndRestoreForCategory(
                    _socketCategoryChannel.Id, interfaceChannel, _guild);
            }

            if (!channelExists)
                Log.WriteLine("Does not contain: " + channelName.ToString() + " adding it", LogLevel.DEBUG);

            BaseChannel baseChannel = interfaceChannel as BaseChannel;
            if (baseChannel == null)
            {
                Log.WriteLine(nameof(baseChannel).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            string? channelNameString = EnumExtensions.GetEnumMemberAttrValue(channelName);
            if (channelNameString == null)
            {
                Log.WriteLine(nameof(channelNameString).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            if (!channelExists)
            {
                List<Overwrite> permissionsList = baseChannel.GetGuildPermissions(_guild);

                /*
                ulong categoryId = Database.Instance.CreatedCategoriesWithChannels.First(
                     x => x.Value.CategoryName == _interfaceCategory.CategoryName).Key; */

                Log.WriteLine("Creating a channel named: " + channelNameString + " for category: "
                            + _interfaceCategory.CategoryName + " (" + _socketCategoryChannel.Id + ")", LogLevel.VERBOSE);

                interfaceChannel.ChannelId = await ChannelManager.CreateAChannelForTheCategory(
                    _guild, channelNameString, _socketCategoryChannel.Id, permissionsList);

                Log.WriteLine("Done creating the channel with id: " + interfaceChannel.ChannelId +
                    " named:" + channelNameString + " adding it to the db.", LogLevel.DEBUG);

                channelListForCategory.Add(interfaceChannel);

                Log.WriteLine("Done adding to the db. Count is now: " + channelListForCategory.Count +
                    " for the list of category: " + _interfaceCategory.CategoryName.ToString() +
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