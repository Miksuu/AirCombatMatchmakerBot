using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceChannel
{
    public ChannelType ChannelType { get; set; }
    public string ChannelName { get; set; }
    public ulong ChannelId { get; set; }
    public ulong ChannelsCategoryId { get; set; }
    public List<MessageName> ChannelMessages { get; set; }
    public Dictionary<string, InterfaceMessage> InterfaceMessagesWithIds { get; set; }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);

    public Task CreateAChannelForTheCategory(SocketGuild _guild);
    public Task PrepareChannelMessages();
    public Task PostChannelMessages(SocketGuild _guild,
        InterfaceChannel _databaseInterfaceChannel);
}