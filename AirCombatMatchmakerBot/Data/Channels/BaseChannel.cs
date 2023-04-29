using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Threading.Channels;

[DataContract]
public abstract class BaseChannel : InterfaceChannel
{
    ChannelType InterfaceChannel.ChannelType
    {
        get 
        {
            Log.WriteLine("Getting " + nameof(ChannelType) + ": " + channelType, LogLevel.VERBOSE);
            return channelType;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(channelType) + channelType
                + " to: " + value, LogLevel.VERBOSE);
            channelType = value;
        }
    }

    string? InterfaceChannel.ChannelName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(channelName) + ": " + channelName, LogLevel.VERBOSE);
            return channelName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(channelName) + channelName
                + " to: " + value, LogLevel.VERBOSE);
            channelName = value;
        }
    }

    ulong InterfaceChannel.ChannelId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(channelId) + ": " + channelId, LogLevel.VERBOSE);
            return channelId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(channelId) + channelId
                + " to: " + value, LogLevel.VERBOSE);
            channelId = value;
        }
    }

    ulong InterfaceChannel.ChannelsCategoryId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(channelsCategoryId) + ": " + channelsCategoryId, LogLevel.VERBOSE);
            return channelsCategoryId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(channelsCategoryId) + channelsCategoryId
                + " to: " + value, LogLevel.VERBOSE);
            channelsCategoryId = value;
        }
    }

    ConcurrentDictionary<MessageName, bool> InterfaceChannel.ChannelMessages
    {
        get
        {
            Log.WriteLine("Getting " + nameof(channelMessages) + " with count of: " +
                channelMessages.Count, LogLevel.VERBOSE);
            return channelMessages;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(channelMessages)
                + " to: " + value, LogLevel.VERBOSE);
            channelMessages = value;
        }
    }

    ConcurrentDictionary<ulong, InterfaceMessage> InterfaceChannel.InterfaceMessagesWithIds
    {
        get
        {
            Log.WriteLine("Getting " + nameof(interfaceMessagesWithIds) + " with count of: " +
                interfaceMessagesWithIds.Count, LogLevel.VERBOSE);
            return interfaceMessagesWithIds;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(interfaceMessagesWithIds)
                + " to: " + value, LogLevel.VERBOSE);
            interfaceMessagesWithIds = value;
        }
    }

    [DataMember] protected ChannelType channelType { get; set; }
    [DataMember] protected string? channelName { get; set; }
    [DataMember] protected ulong channelId { get; set; }
    [DataMember] protected ulong channelsCategoryId { get; set; }
    [DataMember] protected ConcurrentDictionary<MessageName, bool> channelMessages { get; set; }
    [DataMember] protected ConcurrentDictionary<ulong, InterfaceMessage> interfaceMessagesWithIds { get; set; }
    public BaseChannel()
    {
        channelMessages = new ConcurrentDictionary<MessageName, bool>();
        interfaceMessagesWithIds = new ConcurrentDictionary<ulong, InterfaceMessage>();
    }

    public abstract List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, SocketRole _role, params ulong[] _allowedUsersIdsArray);

    public async Task CreateAChannelForTheCategory(SocketGuild _guild, SocketRole _role,
         params ulong[] _allowedUsersIdsArray)
    {
        Log.WriteLine("Creating a channel named: " + channelType +
            " for category: " + channelsCategoryId, LogLevel.VERBOSE);

        string channelTypeString = EnumExtensions.GetEnumMemberAttrValue(channelType);

        if (channelName == null)
        {
            Log.WriteLine("channelName was null!", LogLevel.CRITICAL);
            return;
        }

        // Temp fix perhaps unnecessary after the name has been set more properly 
        // for non-match channels
        if (channelName.Contains("match-"))
        {
            channelTypeString = channelName;
        }

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return;
        }

        var channel = await _guild.CreateTextChannelAsync(channelTypeString, x =>
        {
            x.PermissionOverwrites = GetGuildPermissions(_guild, _role, _allowedUsersIdsArray);
            x.CategoryId = channelsCategoryId;
        });

        channelId = channel.Id;

        Log.WriteLine("Done creating a channel named: " + channelType + " with ID: " + channel.Id +
            " for category: " + channelsCategoryId, LogLevel.DEBUG);
    }

    public async Task<InterfaceMessage?> CreateAMessageForTheChannelFromMessageName(
        MessageName _MessageName, bool _displayMessage = true,
        SocketMessageComponent? _component = null, bool _ephemeral = true)
    {
        Log.WriteLine("Creating a message named: " + _MessageName.ToString(), LogLevel.DEBUG);

        InterfaceMessage interfaceMessage =
            (InterfaceMessage)EnumExtensions.GetInstance(_MessageName.ToString());

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return interfaceMessage;
        }

        var newMessageTuple = await interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClass(
            client, this, true, _displayMessage, _component, _ephemeral);

        return newMessageTuple;
    }

    public async Task<Discord.IUserMessage?>CreateARawMessageForTheChannelFromMessageName(
        string _input, string _embedTitle, bool _displayMessage,
        SocketMessageComponent? _component, bool _ephemeral, params string[] _files)
    {
        Log.WriteLine("Creating a raw message: " + _input, LogLevel.DEBUG);

        InterfaceMessage interfaceMessage =
            (InterfaceMessage)EnumExtensions.GetInstance(MessageName.RAWMESSAGEINPUT.ToString());

        var rawMessageInput = interfaceMessage as RAWMESSAGEINPUT;
        if (rawMessageInput == null)
        {
            Log.WriteLine(nameof(rawMessageInput) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        rawMessageInput.GenerateRawMessage(_input, _embedTitle);

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return null;
        }

        var createdInterfaceMessage = await rawMessageInput.CreateTheMessageAndItsButtonsOnTheBaseClass(
            client, this, true, _displayMessage, _component, _ephemeral, _files);
        if (createdInterfaceMessage == null)
        {
            Log.WriteLine(nameof(createdInterfaceMessage) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        return createdInterfaceMessage.CachedUserMessage;
    }

    public async Task<InterfaceMessage?> CreateARawMessageForTheChannelFromMessageNameWithAttachmentData(
    string _input, AttachmentData[] _attachmentDatas, string _embedTitle = "", bool _displayMessage = true,
    SocketMessageComponent? _component = null, bool _ephemeral = true)
    {
        Log.WriteLine("Creating a raw message with attachmentdata: " + _input +
            " count: " + _attachmentDatas.Length, LogLevel.DEBUG);

        InterfaceMessage interfaceMessage =
            (InterfaceMessage)EnumExtensions.GetInstance(MessageName.RAWMESSAGEINPUT.ToString());

        var rawMessageInput = interfaceMessage as RAWMESSAGEINPUT;
        if (rawMessageInput == null)
        {
            Log.WriteLine(nameof(rawMessageInput) + " was null!", LogLevel.CRITICAL);
            return interfaceMessage;
        }

        rawMessageInput.GenerateRawMessage(_input, _embedTitle);

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return interfaceMessage;
        }

        interfaceMessage = rawMessageInput;

        var newMessageTuple = await interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClassWithAttachmentData(
            client, this, _attachmentDatas, _displayMessage, 0, _component, _ephemeral);

        return newMessageTuple;
    }


    public virtual async Task PostChannelMessages(DiscordSocketClient _client)
    {
        //Log.WriteLine("Starting to post channel messages on: " + channelType, LogLevel.VERBOSE);

        Log.WriteLine("Finding channel: " + channelType + " (" + channelId +
            ") parent category with id: " + channelsCategoryId, LogLevel.VERBOSE);

        // If the messageDescription doesn't exist, set it ID to 0 to regenerate it

        var channel = _client.GetChannelAsync(channelId).Result as ITextChannel;

        if (channel == null)
        {
            Log.WriteLine(nameof(channel) + " was null!", LogLevel.CRITICAL);
            return;
        }

        var channelMessagesFromDb = await channel.GetMessagesAsync(50, CacheMode.AllowDownload).FirstOrDefaultAsync();
        if (channelMessagesFromDb == null)
        {
            Log.WriteLine(nameof(channelMessagesFromDb) + " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine(nameof(interfaceMessagesWithIds) + " count: " +
            interfaceMessagesWithIds.Count + " | " + nameof(channelMessagesFromDb) +
            " count: " + channelMessagesFromDb.Count, LogLevel.VERBOSE);

        if (channelType == ChannelType.LEAGUEREGISTRATION)
        {
            Log.WriteLine("Starting to to prepare channel messages on " + channelType +
                " count: " + Enum.GetValues(typeof(CategoryType)).Length, LogLevel.VERBOSE);

            foreach (CategoryType leagueName in Enum.GetValues(typeof(CategoryType)))
            {
                Log.WriteLine("Looping on to find leagueName: " + leagueName.ToString(), LogLevel.VERBOSE);

                // Skip all the non-leagues
                int enumValue = (int)leagueName;
                if (enumValue > 100) continue;

                string? leagueNameString = EnumExtensions.GetEnumMemberAttrValue(leagueName);
                Log.WriteLine("leagueNameString after enumValueCheck: " + leagueNameString, LogLevel.VERBOSE);
                if (leagueNameString == null)
                {
                    Log.WriteLine(nameof(leagueNameString) + " was null!", LogLevel.CRITICAL);
                    return;
                }

                var leagueInterface = LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueName);
                if (leagueInterface == null)
                {
                    Log.WriteLine("leagueInterface was null!", LogLevel.CRITICAL);
                    return;
                }

                var leagueInterfaceFromDatabase =
                    Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(leagueInterface);

                Log.WriteLine("Starting to create a league join button for: " + leagueNameString, LogLevel.VERBOSE);

                if (leagueInterfaceFromDatabase == null)
                {
                    Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
                    return;
                }

                Log.WriteLine(nameof(leagueInterfaceFromDatabase) + " before creating leagueButtonRegisterationCustomId: "
                    + leagueInterfaceFromDatabase.ToString(), LogLevel.VERBOSE);

                if (leagueInterfaceFromDatabase.DiscordLeagueReferences.LeagueRegistrationMessageId != 0) continue;

                InterfaceMessage interfaceMessage =
                    (InterfaceMessage)EnumExtensions.GetInstance(MessageName.LEAGUEREGISTRATIONMESSAGE.ToString());
                
                await interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClass(
                        _client, this, true, true);

                leagueInterfaceFromDatabase.DiscordLeagueReferences.LeagueRegistrationMessageId = interfaceMessage.MessageId;

                interfaceMessagesWithIds.TryAdd(
                    leagueInterfaceFromDatabase.DiscordLeagueReferences.LeagueCategoryId,
                        (InterfaceMessage)EnumExtensions.GetInstance(MessageName.LEAGUEREGISTRATIONMESSAGE.ToString()));

                Log.WriteLine("Added to the ConcurrentDictionary, count is now: " +
                    interfaceMessagesWithIds.Count, LogLevel.VERBOSE);

                Log.WriteLine("Done looping on: " + leagueNameString, LogLevel.VERBOSE);
            }
        }
        else
        {
            for (int m = 0; m < channelMessages.Count; ++m)
            {
                // Skip the messages that have been generated already
                if (!channelMessages.ElementAt(m).Value)
                {
                    InterfaceMessage interfaceMessage =
                        (InterfaceMessage)EnumExtensions.GetInstance(channelMessages.ElementAt(m).Key.ToString());

                    await interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClass(_client, this, true, true);
                    channelMessages[channelMessages.ElementAt(m).Key] = true;
                }
            }

            if (channelType == ChannelType.BOTLOG)
            {
                BotMessageLogging.loggingChannelId = channelId;
            }
        }
    }

    // Finds ANY messageDescription with that messageDescription name (there can be multiple of same messages now)
    public InterfaceMessage? FindInterfaceMessageWithNameInTheChannel(
        MessageName _messageName)
    {
        Log.WriteLine("Getting MessageName with name: " + _messageName, LogLevel.VERBOSE);

        var foundInterfaceMessage = interfaceMessagesWithIds.FirstOrDefault(
            x => x.Value.MessageName == _messageName);
        if (foundInterfaceMessage.Value == null)
        {
            Log.WriteLine(nameof(foundInterfaceMessage) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found: " + foundInterfaceMessage.Value.MessageName, LogLevel.VERBOSE);
        return foundInterfaceMessage.Value;
    }

    // Finds all messages with that messageName
    public List<InterfaceMessage>? FindAllInterfaceMessagesWithNameInTheChannel(
        MessageName _messageName)
    {
        List<InterfaceMessage> interfaceMessageValues = new();

        Log.WriteLine("Getting CategoryKvp with name: " + _messageName, LogLevel.VERBOSE);

        var foundInterfaceMessages = interfaceMessagesWithIds.Where(
            x => x.Value.MessageName == _messageName);
        if (foundInterfaceMessages == null)
        {
            Log.WriteLine(nameof(foundInterfaceMessages) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        foreach (var message in foundInterfaceMessages)
        {
            Log.WriteLine("Found: " + message.Value.MessageName, LogLevel.VERBOSE);
            interfaceMessageValues.Add(message.Value);
        }

        Log.WriteLine("returning messages with count: " + interfaceMessageValues.Count, LogLevel.VERBOSE);

        return interfaceMessageValues;
    }

    public async Task<IMessageChannel?> GetMessageChannelById(DiscordSocketClient _client)
    {
        Log.WriteLine("Getting IMessageChannel with id: " + channelId, LogLevel.VERBOSE);

        var channel = await _client.GetChannelAsync(channelId) as IMessageChannel;
        if (channel == null)
        {
            Log.WriteLine(nameof(channel) + " was null!", LogLevel.ERROR);
            return null;
        }

        Log.WriteLine("Found: " + channel.Id, LogLevel.VERBOSE);
        return channel;
    }

    // Deletes all messages in a channel defined by enum MessageName
    public async Task<string> DeleteMessagesInAChannelWithMessageName(
        MessageName _messageNameToDelete)
    {
        var client = BotReference.GetClientRef();
        if (client == null)
        {
            return Exceptions.BotClientRefNull();
        }

        List<InterfaceMessage>? interfaceMessages =
            FindAllInterfaceMessagesWithNameInTheChannel(_messageNameToDelete);
        if (interfaceMessages == null)
        {
            Log.WriteLine(nameof(interfaceMessages) + " was null, with: " +
                _messageNameToDelete, LogLevel.CRITICAL);
            return nameof(interfaceMessages) + " was null";
        }

        var iMessageChannel = await GetMessageChannelById(client);
        if (iMessageChannel == null)
        {
            Log.WriteLine(nameof(iMessageChannel) + " was null!", LogLevel.CRITICAL);
            return nameof(iMessageChannel) + " was null";
        }

        foreach (var interfaceMessage in interfaceMessages)
        {
            Log.WriteLine("Looping on: " + interfaceMessage.MessageId, LogLevel.VERBOSE);

            var message = await interfaceMessage.GetMessageById(iMessageChannel);
            if (message == null)
            {
                Log.WriteLine(nameof(message) + " was null!", LogLevel.CRITICAL);
                continue;
            }

            await message.DeleteAsync();
            Log.WriteLine("Deleted the message: " + message.Id +
                " deleting it from DB count: " + interfaceMessagesWithIds.Count, LogLevel.VERBOSE);

            if (!interfaceMessagesWithIds.Any(msg => msg.Value.MessageId == message.Id))
            {
                Log.WriteLine("Did not contain: " + message.Id, LogLevel.WARNING);
                continue;
            }

            interfaceMessagesWithIds.TryRemove(message.Id, out InterfaceMessage? im);
            Log.WriteLine("Deleted the message: " + message.Id + " from DB. count now:" +
                interfaceMessagesWithIds.Count, LogLevel.VERBOSE);
        }

        return "";
    }
}