using Discord;
using Discord.WebSocket;
using System;

public static class CategoryAndChannelInitiator
{
    public static async Task CreateCategoriesAndChannelsForTheDiscordServer()
    {
        Log.WriteLine("Starting to create categories and channels for" +
            " the discord server", LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();
        Log.WriteLine("guild valid", LogLevel.VERBOSE);
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        var categoryEnumValues = Enum.GetValues(typeof(CategoryName));

        Log.WriteLine(nameof(categoryEnumValues) + " length: " + categoryEnumValues.Length, LogLevel.VERBOSE);

        // Loop through every category names creating them and the channelNames for them
        foreach (CategoryName categoryName in Enum.GetValues(typeof(CategoryName)))
        {
            Log.WriteLine("Looping on category name: " + categoryName.ToString(), LogLevel.VERBOSE);
            // Check here too if a category is missing channelNames
            bool categoryExists = false;

            InterfaceCategory interfaceCategory = GetCategoryInstance(categoryName);
            if (interfaceCategory == null)
            {
                Log.WriteLine(nameof(interfaceCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            if (Database.Instance.CreatedCategoriesWithChannels.Any(x => x.Value.CategoryName == categoryName))
            {
                Log.WriteLine(nameof(Database.Instance.CreatedCategoriesWithChannels) + " already contains: " +
                    categoryName.ToString(), LogLevel.VERBOSE);

                // Replace InterfaceLeagueCategoryCategory with a one that is from the database
                interfaceCategory = Database.Instance.CreatedCategoriesWithChannels.First(x => x.Value.CategoryName == categoryName).Value;

                Log.WriteLine("Replaced with: " + interfaceCategory.CategoryName + " from db", LogLevel.DEBUG);

                categoryExists = true;
            }

            string? categoryNameString = EnumExtensions.GetEnumMemberAttrValue(categoryName);
            if (categoryNameString == null)
            {
                Log.WriteLine(nameof(categoryName).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Creating a category named: " + categoryNameString, LogLevel.VERBOSE);

            BaseCategory baseCategory = interfaceCategory as BaseCategory;
            if (baseCategory == null)
            {
                Log.WriteLine(nameof(baseCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            List<Overwrite> permissionsList = baseCategory.GetGuildPermissions(guild);

            SocketCategoryChannel? socketCategoryChannel = null;

            // If the category doesn't exist at all, create it and add it to the database
            if (!categoryExists)
            {
                socketCategoryChannel =
                    await CategoryManager.CreateANewSocketCategoryChannelAndReturnIt(
                        guild, categoryNameString, permissionsList);
                if (socketCategoryChannel == null)
                {
                    Log.WriteLine(nameof(socketCategoryChannel) + " was null!", LogLevel.CRITICAL);
                    return;
                }

                Log.WriteLine("Created a " + nameof(socketCategoryChannel) + " with id: " + socketCategoryChannel.Id +
                    " that's named: " + socketCategoryChannel.Name, LogLevel.VERBOSE);

                Log.WriteLine("Adding " + nameof(interfaceCategory) + " to " +
                    nameof(Database.Instance.CreatedCategoriesWithChannels), LogLevel.VERBOSE);

                Database.Instance.CreatedCategoriesWithChannels.Add(socketCategoryChannel.Id, interfaceCategory);

                Log.WriteLine("Done adding " + nameof(interfaceCategory) + " to " +
                    nameof(Database.Instance.CreatedCategoriesWithChannels), LogLevel.DEBUG);
            }
            // The category exists, just find it from the database and then get the id of the socketchannel
            else
            {
                var dbCategory = Database.Instance.CreatedCategoriesWithChannels.First(
                    x => x.Value.CategoryName == interfaceCategory.CategoryName);

                InterfaceCategory databaseInterfaceCategory = GetCategoryInstance(categoryName);
                if (databaseInterfaceCategory == null)
                {
                    Log.WriteLine(nameof(databaseInterfaceCategory).ToString() + " was null!", LogLevel.CRITICAL);
                    return;
                }

                Log.WriteLine("Found " + nameof(databaseInterfaceCategory) + " with id: " +
                    dbCategory.Key + " named: " +
                    databaseInterfaceCategory.CategoryName, LogLevel.VERBOSE);

                socketCategoryChannel = guild.GetCategoryChannel(dbCategory.Key);

                Log.WriteLine("Found " + nameof(socketCategoryChannel) + " that's named: " +
                    socketCategoryChannel.Name, LogLevel.DEBUG);
            }

            await CreateChannelsForTheCategory(interfaceCategory, socketCategoryChannel, guild);
        }
        await SerializationManager.SerializeDB();
    }

    public static async Task CreateChannelsForTheCategory(
        InterfaceCategory _interfaceCategory,
        SocketCategoryChannel _socketCategoryChannel,
        SocketGuild _guild)
    {
        Log.WriteLine("Starting to create channels for: " + _socketCategoryChannel.Name +
            " ( " + _socketCategoryChannel.Id + ")" + " Channel count: " +
            _interfaceCategory.ChannelNames.Count, LogLevel.VERBOSE) ;

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

            if (channelName ==  null)
            {

                return;
            }

            InterfaceChannel interfaceChannel = GetChannelInstance(channelName);
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

                channelExists = await ChannelRestore.CheckIfChannelHasBeenDeletedAndRestore(
                    _socketCategoryChannel.Id, _guild, interfaceChannel);
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

                ulong categoryId = Database.Instance.CreatedCategoriesWithChannels.First(
                     x => x.Value.CategoryName == _interfaceCategory.CategoryName).Key;

                Log.WriteLine("Creating a channel named: " + channelNameString + " for category: "
                    + _interfaceCategory.CategoryName + " (" + categoryId + ")", LogLevel.VERBOSE);

                interfaceChannel.ChannelId = await ChannelManager.CreateAChannelForTheCategory(
                    _guild, channelNameString, categoryId, permissionsList);

                Log.WriteLine("Done creating the channel with id: " + interfaceChannel.ChannelId +
                    " named:" + channelNameString + " adding it to the db.", LogLevel.DEBUG);

                channelListForCategory.Add(interfaceChannel);

                Log.WriteLine("Done adding to the db. Count is now: " + channelListForCategory.Count +
                    " for the list of category: " + _interfaceCategory.CategoryName.ToString() +
                    " (" + categoryId + ")", LogLevel.VERBOSE);
            }

            await baseChannel.ActivateChannelFeatures();

            Log.WriteLine("Done looping through: " + channelNameString, LogLevel.VERBOSE);
        }
    }

    public static InterfaceCategory GetCategoryInstance(CategoryName _categoryName)
    {
        return (InterfaceCategory)EnumExtensions.GetInstance(_categoryName.ToString());
    }

    public static InterfaceChannel GetChannelInstance(ChannelName _channelName)
    {
        return (InterfaceChannel)EnumExtensions.GetInstance(_channelName.ToString());
    }


}