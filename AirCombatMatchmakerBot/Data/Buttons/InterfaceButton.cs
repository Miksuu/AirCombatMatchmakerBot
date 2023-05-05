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
    public bool EphemeralResponse { get; set; }

    public Discord.ButtonBuilder CreateTheButton(
        string _customId, int _buttonIndex, ulong _buttonCategoryId,
        ulong _leagueCategoryId = 0);
    public abstract Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage);

}