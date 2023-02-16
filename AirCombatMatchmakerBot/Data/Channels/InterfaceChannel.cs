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
    public Dictionary<ulong, InterfaceMessage> InterfaceMessagesWithIds { get; set; }

    public abstract List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray);

    public Task CreateAChannelForTheCategory(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray);
    public Task<string> CreateAMessageForTheChannelFromMessageName(
        MessageName _MessageName, bool _displayMessage = true);
    //public Task PrepareChannelMessages();
    public Task PostChannelMessages(SocketGuild _guild);
    public InterfaceMessage? FindInterfaceMessageWithNameInTheChannel(
        MessageName _messageName);
    public Task<IMessageChannel?> GetMessageChannelById(DiscordSocketClient _client);
}