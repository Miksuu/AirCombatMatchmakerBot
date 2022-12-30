using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System;

[DataContract]
public abstract class BaseChannel : InterfaceChannel
{
    ChannelName InterfaceChannel.ChannelName
    {
        get 
        {
            Log.WriteLine("Getting " + nameof(ChannelName) + ": " + channelName, LogLevel.VERBOSE);
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

    List<MessageName> InterfaceChannel.ChannelMessages
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

    Dictionary<string, InterfaceMessage> InterfaceChannel.InterfaceMessagesWithIds
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

    [DataMember] protected ChannelName channelName;
    [DataMember] protected ulong channelId;
    [DataMember] protected ulong channelsCategoryId;
    protected List<MessageName> channelMessages;
    [DataMember] protected Dictionary<string, InterfaceMessage> interfaceMessagesWithIds;


    public BaseChannel()
    {
        channelMessages = new List<MessageName>();
        interfaceMessagesWithIds = new Dictionary<string, InterfaceMessage>();
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);

    public async Task CreateAChannelForTheCategory(SocketGuild _guild)
    {
        Log.WriteLine("Creating a channel named: " + channelName +
            " for category: " + channelsCategoryId, LogLevel.VERBOSE);

        string channelNameString = EnumExtensions.GetEnumMemberAttrValue(channelName);

        var channel = await _guild.CreateTextChannelAsync(channelNameString, x => {
            x.PermissionOverwrites = GetGuildPermissions(_guild);
            x.CategoryId = channelsCategoryId;
        });

        channelId = channel.Id;

        Log.WriteLine("Done creating a channel named: " + channelName + " with ID: " + channel.Id +
            " for category: " + channelsCategoryId, LogLevel.DEBUG);
    }

    public virtual async Task PrepareChannelMessages()
    {
        Log.WriteLine("Starting to prepare channel messages on: " + channelName, LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        // Add to a method later
        var databaseInterfaceChannel =
            Database.Instance.Categories.CreatedCategoriesWithChannels.First(
                x => x.Key == channelsCategoryId).Value.InterfaceChannels.First(
                    x => x.ChannelId == channelId);

        foreach (var messageName in channelMessages)
        {
            Log.WriteLine("on: " + nameof(messageName) + messageName, LogLevel.VERBOSE);

            InterfaceMessage interfaceMessage = (InterfaceMessage)EnumExtensions.GetInstance(messageName.ToString());

            if (databaseInterfaceChannel.InterfaceMessagesWithIds.ContainsKey(messageName.ToString())) continue;

            Log.WriteLine("Does not contain the key: " +
                messageName + ", continuing", LogLevel.VERBOSE);

            databaseInterfaceChannel.InterfaceMessagesWithIds.Add(messageName.ToString(), interfaceMessage);

            Log.WriteLine("Done with: " + messageName, LogLevel.VERBOSE);
        }
        Log.WriteLine("Done posting channel messages on " +
            channelName + " id: " + channelId, LogLevel.VERBOSE);

        await PostChannelMessages(guild, databaseInterfaceChannel);
    }

    public virtual async Task PostChannelMessages(SocketGuild _guild, 
        InterfaceChannel _databaseInterfaceChannel)
    {
        Log.WriteLine("Starting to post channel messages on: " + channelName, LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        Log.WriteLine("Finding channels: " + channelName + " parent category with id: " +
            channelsCategoryId, LogLevel.VERBOSE);

        foreach (var interfaceMessageKvp in _databaseInterfaceChannel.InterfaceMessagesWithIds) 
        {
            Log.WriteLine("Looping on message: " + interfaceMessageKvp.Value.MessageName + " with id: " +
                interfaceMessageKvp.Key, LogLevel.VERBOSE);

            /*

            var channelMessages =
                await _leagueRegistrationChannel.GetMessagesAsync(
                    50, CacheMode.AllowDownload).FirstAsync();

            Log.WriteLine("Searching: " + leagueNameString + " from: " + nameof(channelMessages) +
                " with a count of: " + channelMessages.Count, LogLevel.VERBOSE);

            foreach (var msg in channelMessages)
            {
                Log.WriteLine("Looping on msg: " + msg.Content.ToString(), LogLevel.VERBOSE);
                if (msg.Content.Contains(leagueNameString))
                {
                    Log.WriteLine($"contains: {msg.Content}", LogLevel.VERBOSE);
                    //containsMessage = true;
                }
            }*/

            var messageKey = interfaceMessagesWithIds[interfaceMessageKvp.Key];

            // If the message doesn't exist, set it ID to 0 to regenerate it
            var channel = _guild.GetTextChannel(_databaseInterfaceChannel.ChannelId);
            var message = await channel.GetMessageAsync(interfaceMessageKvp.Value.MessageId);
            if (message == null) 
            {
                Log.WriteLine("Message " + messageKey.MessageId +
                    "not found! Setting it to 0 and regenerating", LogLevel.WARNING);
                messageKey.MessageId = 0;
            }

            if (messageKey.MessageId != 0) continue;

            Log.WriteLine("Key was 0, message does not exist. Creating it.", LogLevel.VERBOSE);

            ulong id = interfaceMessageKvp.Value.CreateTheMessageAndItsButtonsOnTheBaseClass(
            guild, channelId, interfaceMessageKvp.Key).Result;

            messageKey.MessageId = id;
        }
        return;
    }
}