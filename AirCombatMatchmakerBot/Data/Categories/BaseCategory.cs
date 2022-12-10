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

    List<ChannelName> InterfaceCategory.Channels
    {
        get => channels;
        set => channels = value;
    }

    ulong InterfaceCategory.CategoryId
    {
        get => categoryId;
        set => categoryId = value;
    }

    public CategoryName categoryName;
    public List<ChannelName> channels;
    public ulong categoryId;

    public BaseCategory()
    {
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);
}