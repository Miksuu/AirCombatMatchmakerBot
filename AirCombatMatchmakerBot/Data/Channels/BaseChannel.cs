using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;

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

    Dictionary<MessageName, ulong> InterfaceChannel.ChannelMessagesWithIds
    {
        get
        {
            Log.WriteLine("Getting " + nameof(channelMessagesWithIds) + " with count of: " +
                channelMessagesWithIds.Count, LogLevel.VERBOSE);
            return channelMessagesWithIds;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(channelMessagesWithIds)
                + " to: " + value, LogLevel.VERBOSE);
            channelMessagesWithIds = value;
        }
    }

    [DataMember] protected ChannelName channelName;
    [DataMember] protected ulong channelId;
    [DataMember] protected ulong channelsCategoryId;
    [DataMember] protected Dictionary<MessageName, ulong> channelMessagesWithIds { get; set; }

    public BaseChannel()
    {
        channelMessagesWithIds = new Dictionary<MessageName, ulong>();
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);
    public virtual Task PostChannelMessages()
    {
        Log.WriteLine("Starting to post channel messages on: " + channelName, LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return Task.CompletedTask;
        }

        foreach (var messageNameWithId in channelMessagesWithIds)
        {
            Log.WriteLine("on: " + nameof(messageNameWithId) + messageNameWithId.Key, LogLevel.VERBOSE);

            if (messageNameWithId.Value != 0)
            {
                Log.WriteLine("Already contains key " + messageNameWithId.Key, LogLevel.VERBOSE);
                return Task.CompletedTask;
            }

            Log.WriteLine("Does not contain the key: " +
                messageNameWithId.Key + ", continuing", LogLevel.VERBOSE);

            InterfaceMessage interfaceMessage = (InterfaceMessage)EnumExtensions.GetInstance(messageNameWithId.Key.ToString());

            ulong id = interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClass(
                    guild, channelId).Result;

            channelMessagesWithIds[messageNameWithId.Key] = id;

            Log.WriteLine("Done with: " + messageNameWithId.Key + " | " + messageNameWithId.Value, LogLevel.VERBOSE);
        }
        Log.WriteLine("Done posting channel messages on " +
            channelName + " id: " + channelId, LogLevel.VERBOSE);

        return Task.CompletedTask;
    }
}