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
    Dictionary<string, ulong> InterfaceChannel.ChannelFeaturesWithMessageIds
    {
        get
        {
            Log.WriteLine("Getting " + nameof(channelFeaturesWithMessageIds) + " with count of: " +
                channelFeaturesWithMessageIds.Count, LogLevel.VERBOSE);
            return channelFeaturesWithMessageIds;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(channelFeaturesWithMessageIds)
                + " to: " + value, LogLevel.VERBOSE);
            channelFeaturesWithMessageIds = value;
        }
    }

    [DataMember] protected ChannelName channelName;
    [DataMember] protected ulong channelId;
    [DataMember] protected ulong channelsCategoryId;
    [DataMember] protected Dictionary<string, ulong> channelFeaturesWithMessageIds;

    public BaseChannel()
    {
        channelFeaturesWithMessageIds = new Dictionary<string, ulong>();
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);

    public abstract Task ActivateChannelFeatures();
}