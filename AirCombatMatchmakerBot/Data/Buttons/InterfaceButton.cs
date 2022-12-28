using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceButton
{
    public ButtonName ButtonName { get; set; }

    public abstract void TempMethod();
}