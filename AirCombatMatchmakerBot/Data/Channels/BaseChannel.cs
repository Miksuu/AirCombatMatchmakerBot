using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

[DataContract]
public abstract class BaseChannel : InterfaceChannel
{
    ChannelType InterfaceChannel.ChannelType
    {
        get => channelType.GetValue();
        set => channelType.SetValue(value);
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

    [IgnoreDataMember]
    ConcurrentDictionary<MessageName, bool> InterfaceChannel.ChannelMessages
    {
        get => channelMessages.GetValue();
        set => channelMessages.SetValue(value);
    }

    [IgnoreDataMember]
    ConcurrentDictionary<ulong, InterfaceMessage> InterfaceChannel.InterfaceMessagesWithIds
    {
        get => interfaceMessagesWithIds.GetValue();
        set => interfaceMessagesWithIds.SetValue(value);
    }

    [DataMember] protected logClass<ChannelType> channelType = new logClass<ChannelType>();
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
        Log.WriteLine("Creating a channel named: " + thisInterfaceChannel.ChannelType +
            " for category: " + thisInterfaceChannel.ChannelsCategoryId, LogLevel.VERBOSE);

        string channelTypeString = EnumExtensions.GetEnumMemberAttrValue(thisInterfaceChannel.ChannelType);

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

        Log.WriteLine("Done creating a channel named: " + thisInterfaceChannel.ChannelType + " with ID: " + channel.Id +
            " for category: " + thisInterfaceChannel.ChannelsCategoryId, LogLevel.DEBUG);
    }

    public async Task CreateAChannelForTheCategoryWithoutRole(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray)
    {
        Log.WriteLine("Creating a channel named: " + thisInterfaceChannel.ChannelType +
            " for category: " + thisInterfaceChannel.ChannelsCategoryId, LogLevel.VERBOSE);

        string channelTypeString = EnumExtensions.GetEnumMemberAttrValue(thisInterfaceChannel.ChannelType);

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

        Log.WriteLine("Done creating a channel named: " + thisInterfaceChannel.ChannelType + " with ID: " + channel.Id +
            " for category: " + thisInterfaceChannel.ChannelsCategoryId, LogLevel.DEBUG);
    }

    public async Task<InterfaceMessage> CreateAMessageForTheChannelFromMessageName(
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
            throw new InvalidOperationException("Client ref was not found!");
        }

        InterfaceMessage newInterfaceMessage = await interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClass(
            client, this, true, _displayMessage, 0, _component, _ephemeral);

        return newInterfaceMessage;
    }

    public async Task<Discord.IUserMessage> CreateARawMessageForTheChannelFromMessageName(
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
            throw new InvalidOperationException(nameof(rawMessageInput) + " was null!");
        }

        rawMessageInput.GenerateRawMessage(_input, _embedTitle);

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            throw new InvalidOperationException(Exceptions.BotClientRefNull());
        }

        try
        {
            var createdInterfaceMessage = await rawMessageInput.CreateTheMessageAndItsButtonsOnTheBaseClass(
                client, this, true, _displayMessage, 0, _component, _ephemeral, _files);

            return createdInterfaceMessage.CachedUserMessage;
        }

        catch (Exception ex)
        {
            throw new InvalidOperationException(nameof(InterfaceMessage) + " was null!");
        }

    }

    public async Task<InterfaceMessage> CreateARawMessageForTheChannelFromMessageNameWithAttachmentData(
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

        var newMessage = await interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClassWithAttachmentData(
            client, this, _attachmentDatas, _displayMessage, 0, _component, _ephemeral);

        return newMessage;
    }


    public virtual async Task PostChannelMessages(DiscordSocketClient _client)
    {
        //Log.WriteLine("Starting to post channel messages on: " + channelType, LogLevel.VERBOSE);

        Log.WriteLine("Finding channel: " + thisInterfaceChannel.ChannelType + " (" + thisInterfaceChannel.ChannelId +
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

        Log.WriteLine(nameof(thisInterfaceChannel.InterfaceMessagesWithIds) + " count: " +
            thisInterfaceChannel.InterfaceMessagesWithIds.Count + " | " + nameof(channelMessagesFromDb) +
            " count: " + channelMessagesFromDb.Count, LogLevel.VERBOSE);

        if (thisInterfaceChannel.ChannelType == ChannelType.LEAGUEREGISTRATION)
        {
            Log.WriteLine("Starting to to prepare channel messages on " + thisInterfaceChannel.ChannelType +
                " count: " + Enum.GetValues(typeof(CategoryType)).Length, LogLevel.VERBOSE);

            foreach (CategoryType leagueName in Enum.GetValues(typeof(CategoryType)))
            {
                Log.WriteLine("Looping on to find leagueName: " + leagueName.ToString(), LogLevel.VERBOSE);

                // Skip all the non-leagues
                int enumValue = (int)leagueName;
                if (enumValue > 100) continue;

                string leagueNameString = EnumExtensions.GetEnumMemberAttrValue(leagueName);
                Log.WriteLine("leagueNameString after enumValueCheck: " + leagueNameString, LogLevel.VERBOSE);
                if (leagueNameString == null)
                {
                    Log.WriteLine(nameof(leagueNameString) + " was null!", LogLevel.CRITICAL);
                    return;
                }

                try
                {
                    var leagueInterface = LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueName);
                    var leagueInterfaceFromDatabase =
                        Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(leagueInterface);

                    Log.WriteLine("Starting to create a league join button for: " + leagueNameString, LogLevel.VERBOSE);

                    Log.WriteLine(nameof(leagueInterfaceFromDatabase) + " before creating leagueButtonRegisterationCustomId: "
                        + leagueInterfaceFromDatabase.ToString(), LogLevel.VERBOSE);

                    if (leagueInterfaceFromDatabase.LeagueRegistrationMessageId != 0) continue;

                    InterfaceMessage interfaceMessage =
                        (InterfaceMessage)EnumExtensions.GetInstance(MessageName.LEAGUEREGISTRATIONMESSAGE.ToString());

                    var newInterfaceMessage = await interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClass(
                            _client, this, true, true, leagueInterfaceFromDatabase.LeagueCategoryId);

                    leagueInterfaceFromDatabase.LeagueRegistrationMessageId = interfaceMessage.MessageId;

                    thisInterfaceChannel.InterfaceMessagesWithIds.TryAdd(
                        leagueInterfaceFromDatabase.LeagueCategoryId,
                            (InterfaceMessage)EnumExtensions.GetInstance(MessageName.LEAGUEREGISTRATIONMESSAGE.ToString()));

                    Log.WriteLine("Added to the ConcurrentDictionary, count is now: " +
                        thisInterfaceChannel.InterfaceMessagesWithIds.Count, LogLevel.VERBOSE);

                    Log.WriteLine("Done looping on: " + leagueNameString, LogLevel.VERBOSE);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                    continue;
                }
            }
        }
        else
        {
            foreach (var item in thisInterfaceChannel.ChannelMessages)
            {
                Log.WriteLine(item.Key.ToString(), LogLevel.WARNING);
            }

            for (int m = 0; m < thisInterfaceChannel.ChannelMessages.Count; ++m)
            {
                // Skip the messages that have been generated already
                if (!thisInterfaceChannel.ChannelMessages.ElementAt(m).Value)
                {
                    InterfaceMessage interfaceMessage =
                        (InterfaceMessage)EnumExtensions.GetInstance(thisInterfaceChannel.ChannelMessages.ElementAt(m).Key.ToString());

                    await interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClass(_client, this, true, true);
                    thisInterfaceChannel.ChannelMessages[thisInterfaceChannel.ChannelMessages.ElementAt(m).Key] = true;
                }
            }

            if (thisInterfaceChannel.ChannelType == ChannelType.BOTLOG)
            {
                BotMessageLogging.loggingChannelId = thisInterfaceChannel.ChannelId;
            }
        }
    }

    // Finds ANY MessageDescription with that MessageDescription name (there can be multiple of same messages now)
    public InterfaceMessage FindInterfaceMessageWithNameInTheChannel(
        MessageName _messageName)
    {
        Log.WriteLine("Getting MessageName with name: " + _messageName, LogLevel.VERBOSE);

        var interfaceMessage = thisInterfaceChannel.InterfaceMessagesWithIds.FirstOrDefault(
            x => x.Value.MessageName == _messageName).Value;
        if (interfaceMessage == null)
        {
            string errorMsg = nameof(interfaceMessage) + " was null! with name: " + _messageName;
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            throw new InvalidOperationException(errorMsg);
        }

        Log.WriteLine("Found: " + interfaceMessage.MessageName, LogLevel.VERBOSE);
        return interfaceMessage;
    }

    // Finds all messages with that messageName
    public List<InterfaceMessage> FindAllInterfaceMessagesWithNameInTheChannel(
        MessageName _messageName)
    {
        List<InterfaceMessage> interfaceMessageValues = new();

        Log.WriteLine("Getting CategoryKvp with name: " + _messageName, LogLevel.VERBOSE);

        var foundInterfaceMessages = thisInterfaceChannel.InterfaceMessagesWithIds.Where(
            x => x.Value.MessageName == _messageName);
        if (foundInterfaceMessages == null)
        {
            Log.WriteLine(nameof(foundInterfaceMessages) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(foundInterfaceMessages) + " was null!");
        }

        foreach (var message in foundInterfaceMessages)
        {
            Log.WriteLine("Found: " + message.Value.MessageName, LogLevel.VERBOSE);
            interfaceMessageValues.Add(message.Value);
        }

        Log.WriteLine("returning messages with count: " + interfaceMessageValues.Count, LogLevel.VERBOSE);

        return interfaceMessageValues;
    }

    public async Task<IMessageChannel> GetMessageChannelById(DiscordSocketClient _client)
    {
        Log.WriteLine("Getting IMessageChannel with id: " + thisInterfaceChannel.ChannelId, LogLevel.VERBOSE);

        var channel = await _client.GetChannelAsync(thisInterfaceChannel.ChannelId) as IMessageChannel;
        if (channel == null)
        {
            Log.WriteLine(nameof(channel) + " was null!", LogLevel.ERROR);
            throw new InvalidOperationException(nameof(channel) + " was null!");
        }

        Log.WriteLine("Found: " + channel.Id, LogLevel.VERBOSE);
        return channel;
    }

    // Deletes all messages in a channel defined by enum MessageName
    // Maybe do this a bit better with the trycatch
    public async Task<string> DeleteMessagesInAChannelWithMessageName(
        MessageName _messageNameToDelete)
    {
        try
        {
            var client = BotReference.GetClientRef();

            List<InterfaceMessage> interfaceMessages =
                FindAllInterfaceMessagesWithNameInTheChannel(_messageNameToDelete);
            var iMessageChannel = await GetMessageChannelById(client);

            foreach (var interfaceMessage in interfaceMessages)
            {
                Log.WriteLine("Looping on: " + interfaceMessage.MessageId, LogLevel.VERBOSE);

                Discord.IMessage message;

                try
                {
                    message = await interfaceMessage.GetMessageById(iMessageChannel);
                    await message.DeleteAsync();

                    Log.WriteLine("Deleted the message: " + message.Id +
                        " deleting it from DB count: " + thisInterfaceChannel.InterfaceMessagesWithIds.Count, LogLevel.VERBOSE);
                }
                catch (Exception ex) 
                {
                    Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                    continue;
                }

                if (!thisInterfaceChannel.InterfaceMessagesWithIds.Any(msg => msg.Value.MessageId == message.Id))
                {
                    Log.WriteLine("Did not contain: " + message.Id, LogLevel.WARNING);
                    continue;
                }

                thisInterfaceChannel.InterfaceMessagesWithIds.TryRemove(message.Id, out InterfaceMessage? im);
                Log.WriteLine("Deleted the message: " + message.Id + " from DB. count now:" +
                    thisInterfaceChannel.InterfaceMessagesWithIds.Count, LogLevel.VERBOSE);
            }
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return ex.Message;
        }
        
        return "";
    }

    public async Task DeleteThisChannel(
        InterfaceCategory _interfaceCategory, InterfaceChannel _interfaceChannel, string _nameMustContain)
    {
        ulong channelId = _interfaceChannel.ChannelId;

        Log.WriteLine("Starting to delete channel: " + channelId + " with nameMustContain: " +
            _nameMustContain + " on category: " + _interfaceCategory, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        // Perhaps search within category for a faster operation
        var channel = guild.Channels.FirstOrDefault(
            c => c.Id == channelId &&
                c.Name.Contains(_nameMustContain)); // Just in case
        if (channel == null)
        {
            Log.WriteLine(nameof(channel) + " was null!", LogLevel.ERROR);
            return;
        }

        Log.WriteLine("Found channel: " + +channel.Id + " named: " + channel.Name +
            " deleting it.", LogLevel.VERBOSE);

        //await Task.Delay(_delayInSeconds * 1000);

        //Log.WriteLine("After delay on: " + channel.Id + " deleting channel.", LogLevel.VERBOSE);

        await channel.DeleteAsync();

        Log.WriteLine("Deleted channel: " + channel.Id + " deleting db entry.", LogLevel.VERBOSE);

        /*
        Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            _categoryId).Value.InterfaceChannels.TryRemove(
                channelId, out InterfaceChannel _ic);
        Database.Instance.Categories.MatchChannelsIdWithCategoryId.TryRemove(channelId, out ulong _id);*/

        _interfaceCategory.InterfaceChannels.TryRemove(
                channelId, out InterfaceChannel? _ic);
        Database.Instance.Categories.MatchChannelsIdWithCategoryId.TryRemove(channelId, out ulong _id);

        Log.WriteLine("Deleted channel: " + _id + " from the database.", LogLevel.DEBUG);
    }
}