using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonObjectAttribute]
public interface InterfaceCommand
{
    public CommandName CommandName{ get; set; }
    public string CommandDescription { get; set; }
    public CommandOption? CommandOption { get; set; }
    public abstract Task ActivateCommandFunction();
    public Task AddNewCommandWithOption(
        Discord.WebSocket.DiscordSocketClient _client);
}