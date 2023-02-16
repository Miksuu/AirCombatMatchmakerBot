using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.Serialization;

[JsonObjectAttribute]   
public interface InterfaceMessage
{
    public MessageName MessageName { get; set; }
    public Dictionary<ButtonName, int> MessageButtonNamesWithAmount { get; set; }
    public string Message { get; set; }
    public ulong MessageId { get; set; }
    public ulong MessageChannelId { get; set; }
    public ulong MessageCategoryId { get; set; }
    public List<InterfaceButton> ButtonsInTheMessage { get; set; }

    public Task<string> CreateTheMessageAndItsButtonsOnTheBaseClass(
        Discord.WebSocket.SocketGuild _guild, InterfaceChannel _interfaceChannel,
        bool _displayMessage = true, ulong _leagueCategoryId = 0);
    public Task ModifyMessage(string _newContent);
    public abstract string GenerateMessage();
    public Task GenerateAndModifyTheMessage(bool _serialize = true);
    public Task<Discord.IMessage?> GetMessageById(IMessageChannel _channel);
}