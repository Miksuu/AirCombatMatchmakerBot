using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceButton
{
    public ButtonName ButtonName { get; set; }
    public string ButtonLabel { get; set; }
    public ButtonStyle ButtonStyle { get; set; }

    public Discord.ButtonBuilder CreateTheButton(string _customId);
    public abstract void ActivateButtonFunction();
}