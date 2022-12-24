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
    Dictionary<string, ulong> InterfaceChannel.ChannelFeaturesWithMessageIds
    {
        get => channelFeaturesWithMessageIds;
        set => channelFeaturesWithMessageIds = value;
    }

    BotChannelType InterfaceChannel.BotChannelType
    {
        get => botChannelType;
        set => botChannelType = value;
    }

    public ChannelName channelName;
    public ulong channelId;
    public BotChannelType botChannelType;

    public Dictionary<string, ulong> channelFeaturesWithMessageIds;
    public BaseChannel()
    {
        channelFeaturesWithMessageIds = new Dictionary<string, ulong>();
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);

    public extern virtual Task ActivateChannelFeatures();
}