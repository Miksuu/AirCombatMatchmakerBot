using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;

[DataContract]
public abstract class BaseChannel : InterfaceChannel
{
    ChannelName InterfaceChannel.ChannelName
    {
        get => channelName;
        set => channelName = value;
    }

    ulong InterfaceChannel.ChannelId
    {
        get => channelId;
        set => channelId = value;
    }

    ulong InterfaceChannel.ChannelsCategoryId
    {
        get => channelsCategoryId;
        set => channelsCategoryId = value;
    }
    Dictionary<string, ulong> InterfaceChannel.ChannelFeaturesWithMessageIds
    {
        get => channelFeaturesWithMessageIds;
        set => channelFeaturesWithMessageIds = value;
    }

    protected ChannelName channelName;
    protected ulong channelId;
    protected ulong channelsCategoryId;
    protected Dictionary<string, ulong> channelFeaturesWithMessageIds;

    public BaseChannel()
    {
        channelFeaturesWithMessageIds = new Dictionary<string, ulong>();
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);

    public abstract Task ActivateChannelFeatures();

    public ChannelName GetChannelName()
    {
        Log.WriteLine("Getting " + nameof(ChannelName) + ": " + channelName, LogLevel.VERBOSE);
        return channelName;
    }

    public void SetChannelsId(ulong _newChannelId)
    {
        Log.WriteLine("Setting " + nameof(channelId) + channelId
            + " to: " + _newChannelId, LogLevel.VERBOSE);
        channelId = _newChannelId;
    }

    public void SetChannelsCategoryId(ulong _newCategoryId)
    {
        Log.WriteLine("Setting " + nameof(channelsCategoryId) + channelsCategoryId
            + " to: " + _newCategoryId, LogLevel.VERBOSE);
        channelsCategoryId = _newCategoryId;
    }
}