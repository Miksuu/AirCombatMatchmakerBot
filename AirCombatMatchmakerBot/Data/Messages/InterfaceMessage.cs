using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonObjectAttribute]
public interface InterfaceMessage
{
    public MessageName MessageName { get; set; }
    public Dictionary<ButtonName, int> MessageButtonNamesWithAmount { get; set; }
    public string Message { get; set; }
    public ulong MessageId { get; set; }
    public List<InterfaceButton> ButtonsInTheMessage { get; set; }

    public Task<ulong> CreateTheMessageAndItsButtonsOnTheBaseClass(
        SocketGuild _guild, ulong _channelId, ulong _channelCategoryId);

    public abstract string GenerateMessage(ulong _channelId, ulong _channelCategoryId);
}