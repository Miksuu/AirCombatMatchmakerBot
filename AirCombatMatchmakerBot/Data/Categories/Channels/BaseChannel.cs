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

    public ChannelName channelName;

    public ulong channelId;
    public BaseChannel()
    {
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);
}