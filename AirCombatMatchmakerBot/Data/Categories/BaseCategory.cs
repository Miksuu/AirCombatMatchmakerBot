using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using Discord.Rest;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

[DataContract]
public abstract class BaseCategory : InterfaceCategory
{
    CategoryType InterfaceCategory.CategoryType
    {
        get => categoryType.GetValue();
        set => categoryType.SetValue(value);
    }

    ConcurrentBag<ChannelType> InterfaceCategory.ChannelTypes
    {
        get => channelTypes.GetValue();
        set => channelTypes.SetValue(value);
    }

    [IgnoreDataMember]
    public ConcurrentDictionary<ulong, InterfaceChannel> InterfaceChannels
    {
        get => interfaceChannels.GetValue();
        set => interfaceChannels.SetValue(value);
    }

    ulong InterfaceCategory.SocketCategoryChannelId
    {
        get => socketCategoryChannelId.GetValue();
        set => socketCategoryChannelId.SetValue(value);
    }

    [DataMember] protected logClass<CategoryType> categoryType = new logClass<CategoryType>(new CategoryType());
    protected logConcurrentBag<ChannelType> channelTypes = new logConcurrentBag<ChannelType>();
    [DataMember] protected logConcurrentDictionary<ulong, InterfaceChannel> interfaceChannels = new logConcurrentDictionary<ulong, InterfaceChannel>();
    [DataMember] protected logVar<ulong> socketCategoryChannelId = new logVar<ulong>();

    protected InterfaceCategory thisInterfaceCategory;

    public BaseCategory()
    {
        thisInterfaceCategory = this;
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);

    public async Task<SocketCategoryChannel> CreateANewSocketCategoryChannelAndReturnIt(
        SocketGuild _guild, string _categoryName, SocketRole _role)
    {
        Log.WriteLine("Starting to create a new category with name: " +
            _categoryName);

        RestCategoryChannel newCategory = await _guild.CreateCategoryChannelAsync(
            _categoryName, x => x.PermissionOverwrites = GetGuildPermissions(_guild, _role));
        if (newCategory == null)
        {
            Log.WriteLine(nameof(newCategory) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(newCategory) + " was null!");
        }

        Log.WriteLine("Created a new RestCategoryChannel with ID: " +
            newCategory.Id);

        SocketCategoryChannel socketCategoryChannel =
            _guild.GetCategoryChannel(newCategory.Id);
        if (socketCategoryChannel == null)
        {
            Log.WriteLine(nameof(socketCategoryChannel) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(newCategory) + " was null!");
        }

        Log.WriteLine("Created a new socketCategoryChannel :" +
            socketCategoryChannel.Id.ToString() +" named: " +
            socketCategoryChannel.Name, LogLevel.DEBUG);

        return socketCategoryChannel;
    }

    public async Task CreateChannelsForTheCategory(
        ulong _socketCategoryChannelId, DiscordSocketClient _client, SocketRole _role)
    {
        Log.WriteLine("Starting to create channels for: " + _socketCategoryChannelId + ")" + 
            " Channel count: " + thisInterfaceCategory.ChannelTypes +
            " and setting the references", LogLevel.DEBUG);

        thisInterfaceCategory.SocketCategoryChannelId = _socketCategoryChannelId;

        Log.WriteLine(_socketCategoryChannelId.ToString(), LogLevel.DEBUG);

        foreach (ChannelType channelType in thisInterfaceCategory.ChannelTypes)
        {
            // Checks for missing match channels from the league category
            if (channelType == ChannelType.MATCHCHANNEL)
            {
                //await CreateTheMissingMatchChannels(_client, thisInterfaceCategory.SocketCategoryChannelId);
                continue;
            }

            try
            {
                InterfaceChannel interfaceChannel =
                    await CreateSpecificChannelFromChannelType(channelType, _socketCategoryChannelId, _role);

                await interfaceChannel.PostChannelMessages(_client);
            }
            catch(Exception ex) 
            { 
                Log.WriteLine(ex.Message, LogLevel.ERROR);
                continue;
            }
        }
    }

    public async Task<InterfaceChannel> CreateSpecificChannelFromChannelType(
        ChannelType _channelType, ulong _socketCategoryChannelId, SocketRole _role,
        string _overrideChannelName = "",// Keeps the functionality, but overrides the channel name
                                         // It is used for creating matches with correct name ID right now.
        params ulong[] _allowedUsersIdsArray)
    {
        bool channelExists = false;

        Log.WriteLine("Creating channel name: " + _channelType, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            throw new InvalidOperationException(Exceptions.BotGuildRefNull());
        }

        InterfaceChannel interfaceChannel = GetChannelInstance(_channelType.ToString());

        Log.WriteLine("interfaceChannel initialsetup: " +
            interfaceChannel.ChannelType.ToString(), LogLevel.DEBUG);

        interfaceChannel.ChannelName =
            GetChannelNameFromOverridenString(_overrideChannelName, _channelType);

        foreach (var item in InterfaceChannels)
        {
            Log.WriteLine(item.Value.ChannelName.ToString(), LogLevel.DEBUG);
        }

        // Channel found from the basecategory (it exists)
        if (InterfaceChannels.Any(
            x => x.Value.ChannelName == interfaceChannel.ChannelName))
        {
            Log.WriteLine(nameof(InterfaceChannels) + " with count: " + InterfaceChannels.Count +
                " already contains channel: " + interfaceChannel.ChannelName, LogLevel.DEBUG);

            foreach ( var channel in InterfaceChannels ) 
            {
                Log.WriteLine(channel.Value.ChannelType + " when searching for: " + _channelType +
                    " with id: " + channel.Value.ChannelId, LogLevel.DEBUG);
            }

            // Replace interfaceChannel with a one that is from the database
            interfaceChannel = InterfaceChannels.FirstOrDefault(
                x => x.Value.ChannelType == _channelType).Value;

            Log.WriteLine("Replaced with: " +
                interfaceChannel.ChannelType + " from db. with id: " + interfaceChannel.ChannelId, LogLevel.DEBUG);

            channelExists = ChannelRestore.CheckIfChannelHasBeenDeletedAndRestoreForCategory(
                _socketCategoryChannelId, interfaceChannel, guild);
        }

        interfaceChannel.ChannelsCategoryId = _socketCategoryChannelId;

        if (!channelExists)
        {
            Log.WriteLine("Creating a channel named: " + interfaceChannel.ChannelType +
                " for category: " + thisInterfaceCategory.CategoryType + " (" +
                _socketCategoryChannelId + ")" + " with name: " +
                interfaceChannel.ChannelName, LogLevel.DEBUG);

            await interfaceChannel.CreateAChannelForTheCategory(guild, _role, _allowedUsersIdsArray);

            interfaceChannel.InterfaceMessagesWithIds.Clear();

            InterfaceChannels.TryAdd(interfaceChannel.ChannelId, interfaceChannel);

            Log.WriteLine("Done adding to the db. Count is now: " +
                InterfaceChannels.Count +
                " for the ConcurrentBag of category: " + thisInterfaceCategory.CategoryType.ToString() +
                " (" + _socketCategoryChannelId + ")");
        }

        Log.WriteLine("Done creating channel: " + interfaceChannel.ChannelId + " with name: " 
            + interfaceChannel.ChannelName);

        return interfaceChannel;
    }

    public async Task<InterfaceChannel> CreateSpecificChannelFromChannelTypeWithoutRole(
    ChannelType _channelType, ulong _socketCategoryChannelId,
    string _overrideChannelName = "",// Keeps the functionality, but overrides the channel name
                                     // It is used for creating matches with correct name ID right now.
    params ulong[] _allowedUsersIdsArray)
    {
        bool channelExists = false;

        Log.WriteLine("Creating channel name: " + _channelType, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            throw new InvalidOperationException(Exceptions.BotGuildRefNull());
        }

        InterfaceChannel interfaceChannel = GetChannelInstance(_channelType.ToString());

        Log.WriteLine("interfaceChannel initialsetup: " +
            interfaceChannel.ChannelType.ToString(), LogLevel.DEBUG);

        interfaceChannel.ChannelName =
            GetChannelNameFromOverridenString(_overrideChannelName, _channelType);

        // Channel found from the basecategory (it exists)
        if (InterfaceChannels.Any(
            x => x.Value.ChannelName == interfaceChannel.ChannelName))
        {
            Log.WriteLine(nameof(InterfaceChannels) + " with count: " + InterfaceChannels.Count +
                " already contains channel: " + interfaceChannel.ChannelName, LogLevel.DEBUG);

            foreach (var channel in InterfaceChannels)
            {
                Log.WriteLine(channel.Value.ChannelType + " when searching for: " + _channelType +
                    " with id: " + channel.Value.ChannelId, LogLevel.DEBUG);
            }

            // Replace interfaceChannel with a one that is from the database
            interfaceChannel = InterfaceChannels.FirstOrDefault(
                x => x.Value.ChannelType == _channelType).Value;

            Log.WriteLine("Replaced with: " +
                interfaceChannel.ChannelType + " from db. with id: " + interfaceChannel.ChannelId, LogLevel.DEBUG);

            channelExists = ChannelRestore.CheckIfChannelHasBeenDeletedAndRestoreForCategory(
                _socketCategoryChannelId, interfaceChannel, guild);
        }

        interfaceChannel.ChannelsCategoryId = _socketCategoryChannelId;

        if (!channelExists)
        {
            Log.WriteLine("Creating a channel named: " + interfaceChannel.ChannelType +
                " for category: " + thisInterfaceCategory.CategoryType + " (" +
                _socketCategoryChannelId + ")" + " with name: " +
                interfaceChannel.ChannelName, LogLevel.DEBUG);

            await interfaceChannel.CreateAChannelForTheCategoryWithoutRole(guild, _allowedUsersIdsArray);

            interfaceChannel.InterfaceMessagesWithIds.Clear();

            InterfaceChannels.TryAdd(interfaceChannel.ChannelId, interfaceChannel);

            Log.WriteLine("Done adding to the db. Count is now: " +
                InterfaceChannels.Count +
                " for the ConcurrentBag of category: " + thisInterfaceCategory.CategoryType.ToString() +
                " (" + _socketCategoryChannelId + ")");
        }

        Log.WriteLine("Done creating channel: " + interfaceChannel.ChannelId + " with name: "
            + interfaceChannel.ChannelName);

        return interfaceChannel;
    }

    private static InterfaceChannel GetChannelInstance(string _channelType)
    {
        return (InterfaceChannel)EnumExtensions.GetInstance(_channelType);
    }

    private static Task CreateTheMissingMatchChannels(
        DiscordSocketClient _client, ulong _socketCategoryChannelId)
    {
        InterfaceLeague interfaceLeague;

        Log.WriteLine("Checking for missing matches in: " + _socketCategoryChannelId);

        try
        {
            interfaceLeague =
                Database.Instance.Leagues.GetILeagueByCategoryId(_socketCategoryChannelId);
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return Task.CompletedTask;
        }

        Log.WriteLine("Found InterfaceLeague: " + interfaceLeague.LeagueCategoryName);

        Matches matches = interfaceLeague.LeagueData.Matches;

        /*
        foreach (LeagueMatch match in matches. atchesConcurrentBag)
        {
            Log.WriteLine("Looping on match id: " + match.MatchId +
                " with channelId: " + match.MatchChannelId);

            var matchChannel = _client.GetChannelAsync(match.MatchChannelId).Result as ITextChannel;

            if (matchChannel != null)
            {
                Log.WriteLine("Found " + nameof(matchChannel) + matchChannel.Name);
                continue;
            }

            Log.WriteLine(nameof(matchChannel) + " was not found!" +
                " Expected to find a channel with match id: " + match.MatchId, LogLevel.WARNING);

            matches.CreateAMatchChannel(match, interfaceLeague, _client);
        }*/

        return Task.CompletedTask;
    }

    private static string GetChannelNameFromOverridenString(
        string _overrideChannelName, ChannelType _channelType)
    {
        if (_overrideChannelName == "")
        {
            Log.WriteLine("Settings regular channel name to: " +
                _channelType.ToString(), LogLevel.DEBUG);
            // Maybe insert the name more properly here if needed later
            return _channelType.ToString();
        }
        // Channels such as the match channel, that have the same type,
        // but different names
        else
        {
            Log.WriteLine("Setting overriden channel name to: " +
                _overrideChannelName, LogLevel.DEBUG);
            return _overrideChannelName;
        }
    }

    public bool FindIfInterfaceChannelExistsWithIdInTheCategory(
        ulong _idToSearchWith)
    {
        Log.WriteLine("Getting to see if CategoryKvp exists with id: " + _idToSearchWith);

        var foundInterfaceChannel = InterfaceChannels.FirstOrDefault(x => x.Key == _idToSearchWith);
        if (foundInterfaceChannel.Value == null)
        {
            return false;
        }

        Log.WriteLine("Found: " + foundInterfaceChannel.Value.ChannelName);
        return true;
    }

    public InterfaceChannel FindInterfaceChannelWithIdInTheCategory(
        ulong _idToSearchWith)
    {
        Log.WriteLine("Getting CategoryKvp with id: " + _idToSearchWith);

        InterfaceChannel interfaceChannel = InterfaceChannels.FirstOrDefault(x => x.Key == _idToSearchWith).Value;
        if (interfaceChannel == null)
        {
            string errorMsg = nameof(interfaceChannel) + " was null! with id: " + _idToSearchWith;
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            throw new InvalidOperationException(errorMsg);
        }

        Log.WriteLine("Found: " + interfaceChannel.ChannelName);

        return interfaceChannel;
    }

    public InterfaceChannel FindInterfaceChannelWithNameInTheCategory(
        ChannelType _nameToSearchWith)
    {
        Log.WriteLine("Getting CategoryKvp with name: " + _nameToSearchWith);

        InterfaceChannel interfaceChannel = InterfaceChannels.FirstOrDefault(x => x.Value.ChannelType == _nameToSearchWith).Value;
        if (interfaceChannel == null)
        {
            string errorMsg = nameof(interfaceChannel) + " was null! with name: " + _nameToSearchWith;
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            throw new InvalidOperationException(errorMsg);
        }
        Log.WriteLine("Found: " + interfaceChannel.ChannelName);
        return interfaceChannel;
    }
}