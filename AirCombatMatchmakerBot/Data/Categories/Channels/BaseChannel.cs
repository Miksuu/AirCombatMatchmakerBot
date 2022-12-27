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
            Log.WriteLine("Getting " + nameof(channelFeaturesWithMessageIds) + ": " +
                channelFeaturesWithMessageIds, LogLevel.VERBOSE);
            return channelFeaturesWithMessageIds;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(channelFeaturesWithMessageIds) + channelFeaturesWithMessageIds
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

    /*
    public ChannelName GetChannelName()
    {
        Log.WriteLine("Getting " + nameof(ChannelName) + ": " + channelName, LogLevel.VERBOSE);
        return channelName;
    }*/

    /*
    public void SetChannelsId(ulong _newChannelId)
    {
        Log.WriteLine("Setting " + nameof(channelId) + channelId
            + " to: " + _newChannelId, LogLevel.VERBOSE);
        channelId = _newChannelId;
    }*/

    /*
    public void SetChannelsCategoryId(ulong _newCategoryId)
    {
        Log.WriteLine("Setting " + nameof(channelsCategoryId) + channelsCategoryId
            + " to: " + _newCategoryId, LogLevel.VERBOSE);
        channelsCategoryId = _newCategoryId;
    }*/
}