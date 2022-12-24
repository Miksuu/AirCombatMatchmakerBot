using Discord;
using Discord.WebSocket;
using System;


public static class LeagueCategoryAndChannelInitiator
{
    /*
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

            List<InterfaceLeagueChannel> channelListForCategory = Database.Instance.StoredLeagues.First(
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

                ulong leagueCategoryId = Database.Instance.StoredLeagues.First(
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
    }*/


}