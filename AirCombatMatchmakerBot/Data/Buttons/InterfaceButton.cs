using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonObjectAttribute]
public interface InterfaceButton
{
    public ButtonName ButtonName { get; set; }
    public string ButtonLabel { get; set; }
    public ButtonStyle ButtonStyle { get; set; }
    //public ulong ChannelId { get; set; }
    //public ulong MessageId { get; set; } 

    public Discord.ButtonBuilder CreateTheButton(string _customId);
    public abstract Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, string _splitString,
        ulong _channelId, ulong _messageId);
}