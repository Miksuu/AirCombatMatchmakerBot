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
    public Task<(string, bool)> ReceiveCommandAndCheckForAdminRights(
        SocketSlashCommand _command, string _firstOptionString);
    public Task AddNewCommandWithOption(
        Discord.WebSocket.DiscordSocketClient _client);
}