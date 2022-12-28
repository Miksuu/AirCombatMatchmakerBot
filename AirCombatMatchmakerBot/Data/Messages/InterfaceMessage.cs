using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceMessage
{
    public MessageName MessageName { get; set; }
    public bool ShowOnChannelGeneration { get; set; }
    public List<ButtonName> ButtonNames { get; set; }

    public abstract void TempMethod();
}