using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceMessage
{
    public MessageName MessageName { get; set; }
    public bool ShowOnChannelGeneration { get; set; }
    public List<ButtonName> MessageButtonNames { get; set; }
    public string Message { get; set; }
    public ulong MessageId { get; set; }

    public Task<ulong> CreateTheMessageAndItsButtonsOnTheBaseClass(
        SocketGuild _guild, ulong _channelId, string _customIdForButton);
}