using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceChannel
{
    public ChannelName ChannelName { get; set; }
    public ulong ChannelId { get; set; }
    public ulong ChannelsCategoryId { get; set; }
    public Dictionary<MessageName, ulong> ChannelMessagesWithIds { get; set; }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);
    public Task PostChannelMessages();
}