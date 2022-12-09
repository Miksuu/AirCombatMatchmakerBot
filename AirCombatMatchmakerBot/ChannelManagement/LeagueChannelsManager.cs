using Discord;
using Discord.WebSocket;

public static class LeagueChannelsManager
{
    public static void CreateCategoryAndChannelsForALeague(ILeague _leagueInterface)
    {
        Log.WriteLine("Starting to create a category channel for league: " +
            _leagueInterface.LeagueName.ToString(), LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        List<Overwrite> permissions = new List<Overwrite>
        {
            new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),
        };

        SocketCategoryChannel? socketCategoryChannel =
            CategoryManager.CreateANewSocketCategoryChannelAndReturnIt(guild,
                EnumExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueName),
                permissions).Result;

        if (socketCategoryChannel == null)
        {
            Log.WriteLine(nameof(socketCategoryChannel) + " was null!", LogLevel.CRITICAL);
            return;
        }

        _leagueInterface.DiscordLeagueReferences.leagueCategoryId = socketCategoryChannel.Id;
        CreateChannelsForTheCategory(_leagueInterface, guild);

        Log.WriteLine("End of creating a category channel for league: " +
            _leagueInterface.LeagueName.ToString(), LogLevel.DEBUG);
    }

    public static void CreateChannelsForTheCategory(
        ILeague _leagueInterface, SocketGuild _guild)
    {
        Log.WriteLine("Starting to create categories for: " +
            _leagueInterface.LeagueName.ToString(), LogLevel.VERBOSE);

        foreach (LeagueCategoryChannelType channelType in
            Enum.GetValues(typeof(LeagueCategoryChannelType)))
        {
            Log.WriteLine("Looping through: " + channelType.ToString(), LogLevel.VERBOSE);

            if (!_leagueInterface.DiscordLeagueReferences.leagueChannels.ContainsKey(channelType))
            {
                Log.WriteLine("Does not contain: " + channelType.ToString() +
                    " adding it", LogLevel.DEBUG);

                _leagueInterface.DiscordLeagueReferences.leagueChannels.Add(
                    channelType, CreateAChannelForTheCategory(
                        _guild,
                        EnumExtensions.GetEnumMemberAttrValue(channelType),
                        _leagueInterface.DiscordLeagueReferences.leagueCategoryId).Result);
            }
            else
            {
                Log.WriteLine("Contains " + channelType.ToString() +
                    " disregarding it", LogLevel.DEBUG);
            }

            Log.WriteLine("Done looping through: " + channelType.ToString(), LogLevel.VERBOSE);
        }
    }

    private static async Task<ulong> CreateAChannelForTheCategory(
        SocketGuild _guild, string _name, ulong _forCategory)
    {
        Log.WriteLine("Create a channel named: " + _name +
            " for category: " + _forCategory, LogLevel.VERBOSE);

        var channel = await _guild.CreateTextChannelAsync(_name, tcp => tcp.CategoryId = _forCategory);

        Log.WriteLine("Done creating a channel named: " + _name + " with ID: " + channel.Id +
            " for category: " + _forCategory, LogLevel.DEBUG);

        return channel.Id;
    }
}