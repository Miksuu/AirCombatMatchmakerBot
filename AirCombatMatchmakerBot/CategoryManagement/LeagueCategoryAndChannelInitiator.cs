using Discord;
using Discord.WebSocket;
using System;

/*
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

            ILeague interfaceLeagueCategory = GetCategoryInstance(leagueCategoryName);
            Log.WriteLine("after setting interface", LogLevel.VERBOSE);
            if (interfaceLeagueCategory == null)
            {
                Log.WriteLine(nameof(interfaceLeagueCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            if (Database.Instance.StoredLeagueCategoriesWithChannels.Any(
                x => x.Value.LeagueCategoryName == leagueCategoryName))
            {
                Log.WriteLine("after alreadycontains", LogLevel.VERBOSE);
                Log.WriteLine(nameof(Database.Instance.StoredLeagueCategoriesWithChannels) +
                    " already contains: " + leagueCategoryName.ToString(), LogLevel.VERBOSE);

                // Update the units and to the database (before interfaceLeagueCategory is replaced by it)
                Database.Instance.StoredLeagueCategoriesWithChannels.First(
                    x => x.Value.LeagueCategoryName == leagueCategoryName).Value.LeagueUnits = interfaceLeagueCategory.LeagueUnits;

                // Replace InterfaceLeagueCategoryCategory with a one that is from the database
                var interfaceLeagueCategorykvp =
                    Database.Instance.StoredLeagueCategoriesWithChannels.First(
                        x => x.Value.LeagueCategoryName == leagueCategoryName);
                interfaceLeagueCategory = interfaceLeagueCategorykvp.Value;

                Log.WriteLine("Replaced with: " + interfaceLeagueCategory.LeagueCategoryName + " from db", LogLevel.DEBUG);

                categoryExists = await CategoryRestore.CheckIfCategoryHasBeenDeletedAndRestoreForLeagueCategory(
                    interfaceLeagueCategorykvp, guild);
            }

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

            // If the category doesn't exist at all, create it and add it to the database
            if (!categoryExists)
            {
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
                    nameof(Database.Instance.StoredLeagueCategoriesWithChannels), LogLevel.VERBOSE);

                Database.Instance.StoredLeagueCategoriesWithChannels.Add(socketCategoryChannel.Id, interfaceLeagueCategory);

                Log.WriteLine("Done adding " + nameof(interfaceLeagueCategory) + " to " +
                    nameof(Database.Instance.StoredLeagueCategoriesWithChannels), LogLevel.DEBUG);
            }
            // The category exists, just find it from the database and then get the id of the socketchannel
            else
            {
                var dbCategory = Database.Instance.StoredLeagueCategoriesWithChannels.First(
                    x => x.Value.LeagueCategoryName == interfaceLeagueCategory.LeagueCategoryName);

                ILeague databaseInterfaceLeagueCategory = GetCategoryInstance(leagueCategoryName);
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
        ILeague _InterfaceLeagueCategory,
        SocketCategoryChannel _socketCategoryChannel,
        SocketGuild _guild)
    {
        Log.WriteLine("Starting to create league channels for: " + _socketCategoryChannel.Name +
            " ( " + _socketCategoryChannel.Id + ")" + " Channel count: " +
            _InterfaceLeagueCategory.LeagueChannelNames.Count, LogLevel.VERBOSE) ;

        foreach (ChannelName leagueChannelName in Enum.GetValues(typeof(LeagueChannelName)))
        {
            bool channelExists = false;

            List<InterfaceLeagueChannel> channelListForCategory = Database.Instance.StoredLeagueCategoriesWithChannels.First(
                x => x.Key == _socketCategoryChannel.Id).Value.InterfaceLeagueChannels;

            if (channelListForCategory == null)
            {
                Log.WriteLine(nameof(channelListForCategory) + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Found " + nameof(channelListForCategory)
                + " channel count: " + channelListForCategory.Count, LogLevel.VERBOSE);

            InterfaceLeagueChannel interfaceLeagueChannel = GetLeagueChannelInstance(leagueChannelName);
            if (interfaceLeagueChannel == null)
            {
                Log.WriteLine(nameof(interfaceLeagueChannel).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            if (channelListForCategory.Any(x => x.LeagueChannelName == leagueChannelName))
            {
                Log.WriteLine(nameof(channelListForCategory) + " already contains channel: " +
                    leagueChannelName.ToString(), LogLevel.VERBOSE);

                // Replace interfaceLeagueChannel with a one that is from the database
                interfaceLeagueChannel = channelListForCategory.First(x => x.LeagueChannelName == leagueChannelName);

                Log.WriteLine("Replaced with: " + interfaceLeagueChannel.LeagueChannelName + " from db", LogLevel.DEBUG);

                channelExists = await ChannelRestore.CheckIfChannelHasBeenDeletedAndRestoreForeagueCategory(
                    _socketCategoryChannel.Id, interfaceLeagueChannel, _guild);
            }

            interfaceLeagueChannel.LeagueChannelName = leagueChannelName;

            if (!channelExists)
            Log.WriteLine("Does not contain: " + leagueChannelName.ToString() + " adding it", LogLevel.DEBUG);


            BaseLeagueChannel baseLeagueChannel = interfaceLeagueChannel as BaseLeagueChannel;
            if (baseLeagueChannel == null)
            {
                Log.WriteLine(nameof(baseLeagueChannel).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            string? leagueChannelString = EnumExtensions.GetEnumMemberAttrValue(leagueChannelName);
            if (leagueChannelString == null)
            {
                Log.WriteLine(nameof(leagueChannelString).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            };

            if (!channelExists)
            {
                List<Overwrite> permissionsList = baseLeagueChannel.GetGuildPermissions(_guild);

                ulong leagueCategoryId = Database.Instance.StoredLeagueCategoriesWithChannels.First(
                     x => x.Value.LeagueCategoryName == _InterfaceLeagueCategory.LeagueCategoryName).Key;

                Log.WriteLine("Creating a channel named: " + leagueChannelString + " for category: "
                    + _InterfaceLeagueCategory.LeagueCategoryName + " (" + leagueCategoryId + ")", LogLevel.VERBOSE);

                interfaceLeagueChannel.LeagueChannelId = await ChannelManager.CreateAChannelForTheCategory(
                    _guild, leagueChannelString, _socketCategoryChannel.Id, permissionsList);

                Log.WriteLine("Done creating the channel with id: " + interfaceLeagueChannel.LeagueChannelId +
                    " named:" + leagueChannelString + " adding it to the db.", LogLevel.DEBUG);

                channelListForCategory.Add(interfaceLeagueChannel);

                Log.WriteLine("Done adding to the db. Count is now: " + channelListForCategory.Count +
                    " for the list of category: " + _InterfaceLeagueCategory.LeagueCategoryName.ToString() +
                    " (" + leagueCategoryId + ")", LogLevel.VERBOSE);
            }

            await baseLeagueChannel.ActivateChannelFeatures();

            Log.WriteLine("Done looping through: " + leagueChannelString, LogLevel.VERBOSE);
        }
    }

    public static ILeague GetCategoryInstance(LeagueCategoryName _leagueCategoryName)
    {
        return (ILeague)EnumExtensions.GetInstance(_leagueCategoryName.ToString());
    }
}*/