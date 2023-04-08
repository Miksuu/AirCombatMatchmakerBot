using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

[JsonObjectAttribute]   
public interface InterfaceMessage
{
    public MessageName MessageName { get; set; }
    public ConcurrentDictionary<ButtonName, int> MessageButtonNamesWithAmount { get; set; }

    // Embed properties
    public string MessageEmbedTitle { get; set; }
    public string MessageDescription { get; set; }
    public Discord.Color MessageEmbedColor { get; set; }
    public ulong MessageId { get; set; }
    public ulong MessageChannelId { get; set; }
    public ulong MessageCategoryId { get; set; }
    public ConcurrentBag<InterfaceButton> ButtonsInTheMessage { get; set; }

    public Discord.IUserMessage CachedUserMessage { get; set; }

    public Task<InterfaceMessage?> CreateTheMessageAndItsButtonsOnTheBaseClass(
        DiscordSocketClient _client, InterfaceChannel _interfaceChannel,
        bool _displayMessage = true, ulong _leagueCategoryId = 0,
        SocketMessageComponent? _component = null, bool _ephemeral = true,
        params string[] _files);

    public Task<InterfaceMessage?> CreateTheMessageAndItsButtonsOnTheBaseClassWithAttachmentData(
        DiscordSocketClient _client, InterfaceChannel _interfaceChannel, FileManager.AttachmentData[] _attachmentDatas,
        bool _displayMessage = true, ulong _leagueCategoryId = 0,
        SocketMessageComponent? _component = null, bool _ephemeral = true);
    public Task ModifyMessage(string _newContent);
    public abstract string GenerateMessage();
    public Task GenerateAndModifyTheMessage();
    public Task<Discord.IMessage?> GetMessageById(IMessageChannel _channel);
}