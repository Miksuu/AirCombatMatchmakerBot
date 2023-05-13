using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

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

    string InterfaceChannel.ChannelName
    {
        get => channelName.GetValue();
        set => channelName.SetValue(value);
    }

    ulong InterfaceChannel.ChannelId
    {
        get => channelId.GetValue();
        set => channelId.SetValue(value);
    }

    ulong InterfaceChannel.ChannelsCategoryId
    {
        get => channelsCategoryId.GetValue();
        set => channelsCategoryId.SetValue(value);
    }

    public ConcurrentDictionary<MessageName, bool> ChannelMessages
    {
        get => channelMessages.GetValue();
        set => channelMessages.SetValue(value);
    }

    public ConcurrentDictionary<ulong, InterfaceMessage> InterfaceMessagesWithIds
    {
        get => interfaceMessagesWithIds.GetValue();
        set => interfaceMessagesWithIds.SetValue(value);
    }

    [DataMember] protected ChannelType channelType { get; set; }
    [DataMember] protected logString channelName = new logString();
    [DataMember] protected logClass<ulong> channelId = new logClass<ulong>();
    [DataMember] protected logClass<ulong> channelsCategoryId = new logClass<ulong>();
    [DataMember] protected logConcurrentDictionary<MessageName, bool> channelMessages = new logConcurrentDictionary<MessageName, bool>();
    [DataMember] protected logConcurrentDictionary<ulong, InterfaceMessage> interfaceMessagesWithIds = new logConcurrentDictionary<ulong, InterfaceMessage>();
    protected InterfaceChannel thisInterfaceChannel;

    public BaseChannel()
    {
        thisInterfaceChannel = this;
    }

    public abstract List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, SocketRole _role, params ulong[] _allowedUsersIdsArray);

    public async Task CreateAChannelForTheCategory(SocketGuild _guild, SocketRole _role,
         params ulong[] _allowedUsersIdsArray)
    {
        Log.WriteLine("Creating a channel named: " + channelType +
            " for category: " + thisInterfaceChannel.ChannelsCategoryId, LogLevel.VERBOSE);

        string channelTypeString = EnumExtensions.GetEnumMemberAttrValue(channelType);

        if (thisInterfaceChannel.ChannelName == null)
        {
            Log.WriteLine("thisInterfaceChannel.ChannelName was null!", LogLevel.CRITICAL);
            return;
        }

        // Temp fix perhaps unnecessary after the name has been set more properly 
        // for non-match channels
        if (thisInterfaceChannel.ChannelName.Contains("match-"))
        {
            channelTypeString = thisInterfaceChannel.ChannelName;
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
            x.CategoryId = thisInterfaceChannel.ChannelsCategoryId;
        });

        thisInterfaceChannel.ChannelId = channel.Id;

        Log.WriteLine("Done creating a channel named: " + channelType + " with ID: " + channel.Id +
            " for category: " + thisInterfaceChannel.ChannelsCategoryId, LogLevel.DEBUG);
    }

    public async Task CreateAChannelForTheCategoryWithoutRole(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray)
    {
        Log.WriteLine("Creating a channel named: " + channelType +
            " for category: " + thisInterfaceChannel.ChannelsCategoryId, LogLevel.VERBOSE);

        string channelTypeString = EnumExtensions.GetEnumMemberAttrValue(channelType);

        if (thisInterfaceChannel.ChannelName == null)
        {
            Log.WriteLine("thisInterfaceChannel.ChannelName was null!", LogLevel.CRITICAL);
            return;
        }

        // Temp fix perhaps unnecessary after the name has been set more properly 
        // for non-match channels
        if (thisInterfaceChannel.ChannelName.Contains("match-"))
        {
            channelTypeString = thisInterfaceChannel.ChannelName;
        }

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return;
        }

        var channel = await _guild.CreateTextChannelAsync(channelTypeString, x =>
        {
            //x.PermissionOverwrites = GetGuildPermissions(_guild, _allowedUsersIdsArray);
            x.CategoryId = thisInterfaceChannel.ChannelsCategoryId;
        });

        thisInterfaceChannel.ChannelId = channel.Id;

        Log.WriteLine("Done creating a channel named: " + channelType + " with ID: " + channel.Id +
            " for category: " + thisInterfaceChannel.ChannelsCategoryId, LogLevel.DEBUG);
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
            client, this, true, _displayMessage, 0, _component, _ephemeral);

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
            client, this, true, _displayMessage, 0, _component, _ephemeral, _files);
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

        Log.WriteLine("Finding channel: " + channelType + " (" + thisInterfaceChannel.ChannelId +
            ") parent category with id: " + thisInterfaceChannel.ChannelsCategoryId, LogLevel.VERBOSE);

        // If the MessageDescription doesn't exist, set it ID to 0 to regenerate it

        var channel = _client.GetChannelAsync(thisInterfaceChannel.ChannelId).Result as ITextChannel;

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

        Log.WriteLine(nameof(InterfaceMessagesWithIds) + " count: " +
            InterfaceMessagesWithIds.Count + " | " + nameof(channelMessagesFromDb) +
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

                if (leagueInterfaceFromDatabase.LeagueRegistrationMessageId != 0) continue;

                InterfaceMessage interfaceMessage =
                    (InterfaceMessage)EnumExtensions.GetInstance(MessageName.LEAGUEREGISTRATIONMESSAGE.ToString());
                
                var newInterfaceMessage = await interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClass(
                        _client, this, true, true, leagueInterfaceFromDatabase.LeagueCategoryId);

                leagueInterfaceFromDatabase.LeagueRegistrationMessageId = interfaceMessage.MessageId;

                InterfaceMessagesWithIds.TryAdd(
                    leagueInterfaceFromDatabase.LeagueCategoryId,
                        (InterfaceMessage)EnumExtensions.GetInstance(MessageName.LEAGUEREGISTRATIONMESSAGE.ToString()));

                Log.WriteLine("Added to the ConcurrentDictionary, count is now: " +
                    InterfaceMessagesWithIds.Count, LogLevel.VERBOSE);

                Log.WriteLine("Done looping on: " + leagueNameString, LogLevel.VERBOSE);
            }
        }
        else
        {
            for (int m = 0; m < ChannelMessages.Count; ++m)
            {
                // Skip the messages that have been generated already
                if (!ChannelMessages.ElementAt(m).Value)
                {
                    InterfaceMessage interfaceMessage =
                        (InterfaceMessage)EnumExtensions.GetInstance(ChannelMessages.ElementAt(m).Key.ToString());

                    await interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClass(_client, this, true, true);
                    ChannelMessages[ChannelMessages.ElementAt(m).Key] = true;
                }
            }

            if (channelType == ChannelType.BOTLOG)
            {
                BotMessageLogging.loggingChannelId = thisInterfaceChannel.ChannelId;
            }
        }
    }

    // Finds ANY MessageDescription with that MessageDescription name (there can be multiple of same messages now)
    public InterfaceMessage? FindInterfaceMessageWithNameInTheChannel(
        MessageName _messageName)
    {
        Log.WriteLine("Getting MessageName with name: " + _messageName, LogLevel.VERBOSE);

        var foundInterfaceMessage = InterfaceMessagesWithIds.FirstOrDefault(
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

        var foundInterfaceMessages = InterfaceMessagesWithIds.Where(
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
        Log.WriteLine("Getting IMessageChannel with id: " + thisInterfaceChannel.ChannelId, LogLevel.VERBOSE);

        var channel = await _client.GetChannelAsync(thisInterfaceChannel.ChannelId) as IMessageChannel;
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
                " deleting it from DB count: " + InterfaceMessagesWithIds.Count, LogLevel.VERBOSE);

            if (!InterfaceMessagesWithIds.Any(msg => msg.Value.MessageId == message.Id))
            {
                Log.WriteLine("Did not contain: " + message.Id, LogLevel.WARNING);
                continue;
            }

            InterfaceMessagesWithIds.TryRemove(message.Id, out InterfaceMessage? im);
            Log.WriteLine("Deleted the message: " + message.Id + " from DB. count now:" +
                InterfaceMessagesWithIds.Count, LogLevel.VERBOSE);
        }

        return "";
    }
}