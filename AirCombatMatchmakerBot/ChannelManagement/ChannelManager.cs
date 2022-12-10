using Discord;
using Discord.WebSocket;

public static class ChannelManager
{
    public static async Task<ulong> CreateAChannelForTheCategory(
    SocketGuild _guild, string _name, ulong _forCategory, List<Overwrite> _permissions)
    {
        Log.WriteLine("Create a channel named: " + _name +
            " for category: " + _forCategory, LogLevel.VERBOSE);

        TextChannelProperties guildChannelProperties = new TextChannelProperties();
        guildChannelProperties.PermissionOverwrites= _permissions;
        guildChannelProperties.CategoryId = _forCategory;

        var channel = await _guild.CreateTextChannelAsync(_name, x => {
            x.PermissionOverwrites = _permissions;
            x.CategoryId = _forCategory;
        });

        Log.WriteLine("Done creating a channel named: " + _name + " with ID: " + channel.Id +
            " for category: " + _forCategory, LogLevel.DEBUG);

        return channel.Id;
    }
}