using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;
using System.Collections.Concurrent;

[JsonObjectAttribute]
public interface InterfaceChannel
{
    public ChannelType ChannelType { get; set; }
    public string ChannelName { get; set; }
    public ulong ChannelId { get; set; }
    public ulong ChannelsCategoryId { get; set; }
    public ConcurrentDictionary<MessageName, bool> ChannelMessages { get; set; }
    public ConcurrentDictionary<ulong, InterfaceMessage> InterfaceMessagesWithIds { get; set; }

    public abstract List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray);

    public Task CreateAChannelForTheCategory(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray);
    public Task<(ulong, string)> CreateAMessageForTheChannelFromMessageName(
        MessageName _MessageName, bool _displayMessage = true,
        SocketMessageComponent? _component = null, bool _ephemeral = true);
    public Task PostChannelMessages(SocketGuild _guild);
    public InterfaceMessage? FindInterfaceMessageWithNameInTheChannel(
        MessageName _messageName);
    public Task<IMessageChannel?> GetMessageChannelById(DiscordSocketClient _client);
    public Task<string> DeleteMessagesInAChannelWithMessageName(MessageName _messageNameToDelete);
}