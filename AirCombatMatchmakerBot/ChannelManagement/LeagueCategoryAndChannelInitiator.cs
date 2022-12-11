using Discord;
using Discord.WebSocket;
using System;

public static class LeagueCategoryAndChannelInitiator
{
    public static async Task CreateLeagueCategoriesAndChannelsForTheDiscordServer()
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

        var categoryEnumValues = Enum.GetValues(typeof(LeagueCategoryName));

        Log.WriteLine(nameof(categoryEnumValues) + " length: " + categoryEnumValues.Length, LogLevel.VERBOSE);

        // Loop through every category names creating them and the channelNames for them
        foreach (LeagueCategoryName leagueCategoryName in Enum.GetValues(typeof(LeagueCategoryName)))
        {
            Log.WriteLine("Looping on category name: " + leagueCategoryName.ToString(), LogLevel.VERBOSE);
            // Check here too if a category is missing channelNames
            bool categoryExists = false;

            InterfaceLeagueCategory interfaceLeagueCategory = GetCategoryInstance(leagueCategoryName);
            Log.WriteLine("after setting interface", LogLevel.VERBOSE);
            if (interfaceLeagueCategory == null)
            {
                Log.WriteLine(nameof(interfaceLeagueCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("after nullcheck " + nameof(Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels) +
                " count: " + Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.Count, LogLevel.VERBOSE);

            if (Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.Any(x => x.Value.LeagueCategoryName == leagueCategoryName))
            {
                Log.WriteLine("after alreadycontains", LogLevel.VERBOSE);
                Log.WriteLine(nameof(Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels) + " already contains: " +
                    leagueCategoryName.ToString(), LogLevel.VERBOSE);

                // Replace InterfaceLeagueCategoryCategory with a one that is from the database
                interfaceLeagueCategory =
                    Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.First(
                        x => x.Value.LeagueCategoryName == leagueCategoryName).Value;

                Log.WriteLine("Replaced with: " + interfaceLeagueCategory.LeagueCategoryName + " from db", LogLevel.DEBUG);

                categoryExists = true;
            }

            interfaceLeagueCategory.LeagueCategoryName = leagueCategoryName;

            Log.WriteLine("after contains check", LogLevel.VERBOSE);

            string? leagueCategoryNameString = EnumExtensions.GetEnumMemberAttrValue(leagueCategoryName);
            if (leagueCategoryNameString == null)
            {
                Log.WriteLine(nameof(leagueCategoryName).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Creating a category named: " + leagueCategoryNameString, LogLevel.VERBOSE);

            BaseLeagueCategory baseLeagueCategory = interfaceLeagueCategory as BaseLeagueCategory;
            if (baseLeagueCategory == null)
            {
                Log.WriteLine(nameof(baseLeagueCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            List<Overwrite> permissionsList = baseLeagueCategory.GetLeagueGuildPermissions(guild);

            SocketCategoryChannel? socketCategoryChannel = null;

            // If the category doesn't exist at all, create it and add it to the database
            if (!categoryExists)
            {
                socketCategoryChannel =
                    await CategoryManager.CreateANewSocketCategoryChannelAndReturnIt(
                        guild, leagueCategoryNameString, permissionsList);
                if (socketCategoryChannel == null)
                {
                    Log.WriteLine(nameof(socketCategoryChannel) + " was null!", LogLevel.CRITICAL);
                    return;
                }

                Log.WriteLine("Created a " + nameof(socketCategoryChannel) + " with id: " + socketCategoryChannel.Id +
                    " that's named: " + socketCategoryChannel.Name, LogLevel.VERBOSE);

                Log.WriteLine("Adding " + nameof(interfaceLeagueCategory) + " to " +
                    nameof(Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels), LogLevel.VERBOSE);

                Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.Add(socketCategoryChannel.Id, interfaceLeagueCategory);

                Log.WriteLine("Done adding " + nameof(interfaceLeagueCategory) + " to " +
                    nameof(Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels), LogLevel.DEBUG);
            }
            // The category exists, just find it from the database and then get the id of the socketchannel
            else
            {
                var dbCategory = Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.First(
                    x => x.Value.LeagueCategoryName == interfaceLeagueCategory.LeagueCategoryName);

                InterfaceLeagueCategory databaseInterfaceLeagueCategory = GetCategoryInstance(leagueCategoryName);
                if (databaseInterfaceLeagueCategory == null)
                {
                    Log.WriteLine(nameof(databaseInterfaceLeagueCategory).ToString() + " was null!", LogLevel.CRITICAL);
                    return;
                }

                Log.WriteLine("Found " + nameof(databaseInterfaceLeagueCategory) + " with id: " +
                    dbCategory.Key + " named: " +
                    databaseInterfaceLeagueCategory.LeagueCategoryName, LogLevel.VERBOSE);

                socketCategoryChannel = guild.GetCategoryChannel(dbCategory.Key);

                Log.WriteLine("Found " + nameof(socketCategoryChannel) + " that's named: " +
                    socketCategoryChannel.Name, LogLevel.DEBUG);
            }

            Log.WriteLine("FINAL " + nameof(interfaceLeagueCategory) + " for " + leagueCategoryName.ToString() +
                  "::" + interfaceLeagueCategory.LeagueCategoryName + " beforing creating channels", LogLevel.DEBUG);

            await CreateChannelsForTheLeagueCategory(interfaceLeagueCategory, socketCategoryChannel, guild);
        }
        await SerializationManager.SerializeDB();
    }

    public static async Task CreateChannelsForTheLeagueCategory(
        InterfaceLeagueCategory _InterfaceLeagueCategory,
        SocketCategoryChannel _socketCategoryChannel,
        SocketGuild _guild)
    {
        Log.WriteLine("Starting to create league channels for: " + _socketCategoryChannel.Name +
            " ( " + _socketCategoryChannel.Id + ")" + " Channel count: " +
            _InterfaceLeagueCategory.LeagueChannelNames.Count, LogLevel.VERBOSE) ;

        foreach (LeagueChannelName LeagueChannelName in Enum.GetValues(typeof(LeagueChannelName)))
        {
            bool channelExists = false;

            List<InterfaceLeagueChannel> channelListForCategory = Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.First(
                x => x.Key == _socketCategoryChannel.Id).Value.InterfaceLeagueChannels;

            if (channelListForCategory == null)
            {
                Log.WriteLine(nameof(channelListForCategory) + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Found " + nameof(channelListForCategory)
                + " channel count: " + channelListForCategory.Count, LogLevel.VERBOSE);

            InterfaceLeagueChannel InterfaceLeagueChannel = GetLeagueChannelInstance(LeagueChannelName);
            if (InterfaceLeagueChannel == null)
            {
                Log.WriteLine(nameof(InterfaceLeagueChannel).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            if (channelListForCategory.Any(x => x.LeagueChannelName == LeagueChannelName))
            {
                Log.WriteLine(nameof(channelListForCategory) + " already contains channel: " +
                    LeagueChannelName.ToString(), LogLevel.VERBOSE);

                // Replace InterfaceLeagueChannel with a one that is from the database
                InterfaceLeagueChannel = channelListForCategory.First(x => x.LeagueChannelName == LeagueChannelName);

                Log.WriteLine("Replaced with: " + InterfaceLeagueChannel.LeagueChannelName + " from db", LogLevel.DEBUG);

                channelExists = true;
            }

            if (!channelExists)
            Log.WriteLine("Does not contain: " + LeagueChannelName.ToString() + " adding it", LogLevel.DEBUG);


            BaseLeagueChannel baseLeagueChannel = InterfaceLeagueChannel as BaseLeagueChannel;
            if (baseLeagueChannel == null)
            {
                Log.WriteLine(nameof(baseLeagueChannel).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            string? channelNameString = EnumExtensions.GetEnumMemberAttrValue(LeagueChannelName);
            if (channelNameString == null)
            {
                Log.WriteLine(nameof(channelNameString).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            if (!channelExists)
            {
                List<Overwrite> permissionsList = baseLeagueChannel.GetGuildLeaguePermissions(_guild);

                ulong leagueCategoryId = Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.First(
                     x => x.Value.LeagueCategoryName == _InterfaceLeagueCategory.LeagueCategoryName).Key;

                Log.WriteLine("Creating a channel named: " + channelNameString + " for category: "
                    + _InterfaceLeagueCategory.LeagueCategoryName + " (" + leagueCategoryId + ")", LogLevel.VERBOSE);

                InterfaceLeagueChannel.LeagueChannelId = await ChannelManager.CreateAChannelForTheCategory(
                    _guild, channelNameString, _socketCategoryChannel.Id, permissionsList);

                Log.WriteLine("Done creating the channel with id: " + InterfaceLeagueChannel.LeagueChannelId +
                    " named:" + channelNameString + " adding it to the db.", LogLevel.DEBUG);

                channelListForCategory.Add(InterfaceLeagueChannel);

                Log.WriteLine("Done adding to the db. Count is now: " + channelListForCategory.Count +
                    " for the list of category: " + _InterfaceLeagueCategory.LeagueCategoryName.ToString() +
                    " (" + leagueCategoryId + ")", LogLevel.VERBOSE);
            }

            await baseLeagueChannel.ActivateLeagueChannelFeatures();

            Log.WriteLine("Done looping through: " + channelNameString, LogLevel.VERBOSE);
        }
    }

    public static InterfaceLeagueCategory GetCategoryInstance(LeagueCategoryName _leagueCategoryName)
    {
        return (InterfaceLeagueCategory)EnumExtensions.GetInstance(_leagueCategoryName.ToString());
    }

    public static InterfaceLeagueChannel GetLeagueChannelInstance(LeagueChannelName _leagueChannelName)
    {
        return (InterfaceLeagueChannel)EnumExtensions.GetInstance(_leagueChannelName.ToString());
    }
}