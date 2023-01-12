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
    public ulong ButtonCategoryId { get; set; }
    public string ButtonCustomId { get; set; }

    public Discord.ButtonBuilder CreateTheButton(
        string _customId, int _buttonIndex, ulong _buttonCategoryId, string _messageKey);
    public abstract Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage);
}