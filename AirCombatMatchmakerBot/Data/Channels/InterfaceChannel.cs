using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceChannel
{
    public ChannelName ChannelName { get; set; }
    public ulong ChannelId { get; set; }
    public ulong ChannelsCategoryId { get; set; }
    public Dictionary<string, ulong> ChannelFeaturesWithMessageIds { get; set; }
    public List<MessageName> ChannelMessages { get; set; }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);
    public abstract Task PostChannelMessages();
}