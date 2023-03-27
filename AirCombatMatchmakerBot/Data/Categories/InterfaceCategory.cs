using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Collections.Concurrent;

[JsonObjectAttribute]
public interface InterfaceCategory
{
    public CategoryType CategoryType { get; set; }
    public ConcurrentBag<ChannelType> ChannelTypes { get; set; }
    public ConcurrentDictionary<ulong, InterfaceChannel> InterfaceChannels { get; set; }
    public ulong SocketCategoryChannelId { get; set; }

    public abstract ConcurrentBag<Overwrite> GetGuildPermissions(
        SocketGuild _guild, SocketRole _role);

    public Task<SocketCategoryChannel?> CreateANewSocketCategoryChannelAndReturnIt(
        SocketGuild _guild, string _categoryName, SocketRole _role);
    public Task CreateChannelsForTheCategory(
        ulong _socketCategoryChannelId, DiscordSocketClient _client);

    public Task<InterfaceChannel?> CreateSpecificChannelFromChannelType(
        ChannelType _channelType, ulong _socketCategoryChannelId,
        string _overrideChannelName = "", params ulong[] _allowedUsersIdsArray);
    public InterfaceChannel FindInterfaceChannelWithIdInTheCategory(
        ulong _idToSearchWith);
    public InterfaceChannel FindInterfaceChannelWithNameInTheCategory(
        ChannelType _nameToSearchWith);
}