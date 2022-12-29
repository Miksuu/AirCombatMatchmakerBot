using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceChannel
{
    public ChannelName ChannelName { get; set; }
    public ulong ChannelId { get; set; }
    public ulong ChannelsCategoryId { get; set; }
    public List<MessageName> ChannelMessages { get; set; }
    public Dictionary<string, InterfaceMessage> InterfaceMessagesWithIds { get; set; }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);

    public Task PrepareChannelMessages();
    public Task PostChannelMessages(
        Dictionary<string, InterfaceMessage> _interfaceMessagesWithIdsOnDatabase);
}