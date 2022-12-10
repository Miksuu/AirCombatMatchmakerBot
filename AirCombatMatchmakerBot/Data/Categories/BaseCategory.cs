using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;

[DataContract]
public abstract class BaseCategory : InterfaceCategory
{
    CategoryName InterfaceCategory.CategoryName
    {
        get => categoryName;
        set => categoryName = value;
    }

    List<ChannelName> InterfaceCategory.ChannelNames
    {
        get => channelNames;
        set => channelNames = value;
    }

    ulong InterfaceCategory.CategoryId
    {
        get => categoryId;
        set => categoryId = value;
    }

    List<InterfaceChannel> InterfaceCategory.InterfaceChannels
    {
        get => interfaceChannels;
        set => interfaceChannels = value;
    }

    public CategoryName categoryName;
    public List<ChannelName> channelNames;
    public ulong categoryId;
    public List<InterfaceChannel> interfaceChannels;

    public BaseCategory()
    {
        interfaceChannels= new List<InterfaceChannel>();
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);
}