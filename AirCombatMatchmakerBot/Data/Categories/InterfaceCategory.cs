using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceCategory
{
    public CategoryType CategoryType { get; set; }
    public List<ChannelType> ChannelTypes { get; set; }
    public Dictionary<ulong, InterfaceChannel> InterfaceChannels { get; set; }
    public ulong SocketCategoryChannelId { get; set; }

    public abstract List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, SocketRole _role);

    public Task<SocketCategoryChannel?> CreateANewSocketCategoryChannelAndReturnIt(
        SocketGuild _guild, string _categoryName, SocketRole _role);
    public Task CreateChannelsForTheCategory(
        InterfaceCategory _interfaceCategory, ulong _socketCategoryChannelId,
        SocketGuild _guild);

    public Task<InterfaceChannel> CreateSpecificChannelFromChannelType(
        SocketGuild _guild, ChannelType _channelType, ulong _socketCategoryChannelId,
        string _overrideChannelName = "", params ulong[] _allowedUsersIdsArray);
}