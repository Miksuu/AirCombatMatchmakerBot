using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceCategory
{
    public CategoryName CategoryName { get; set; }
    public List<ChannelName> ChannelNames { get; set; }
    public List<InterfaceChannel> InterfaceChannels { get; set; }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);

    public Task<SocketCategoryChannel?> CreateANewSocketCategoryChannelAndReturnIt(
        SocketGuild _guild, string _categoryName);
    public Task CreateChannelsForTheCategory(
        InterfaceCategory _interfaceCategory, 
        SocketCategoryChannel _socketCategoryChannel, 
        SocketGuild _guild);

    public Task CreateSpecificChannelFromChannelName(
    SocketGuild _guild, ChannelName _channelName);
}