using Discord;
using Discord.WebSocket;

public static class ChannelManager
{
    public static async Task SetRegisterationChannelPermissions(
        ulong _userId, SocketGuild _guild, SocketGuildChannel _channel)
    {
        // Sets permission overrides
        var permissionOverridesUser = new OverwritePermissions(viewChannel: PermValue.Allow);

        if (_channel != null)
        {
            Log.WriteLine("FOUND CHANNEL TO SET PERMISSIONS ON: " + _channel.Id, LogLevel.DEBUG);

            // Allow the channell access to the new user
            await _channel.AddPermissionOverwriteAsync(_guild.GetUser(_userId), permissionOverridesUser);
        }
        else
        {
            Log.WriteLine("_Channel was null!", LogLevel.CRITICAL);
        }
    }

    /*
    public static SocketGuildChannel? FindChannel(SocketGuild _guild, string _channelName)
    {
        if (_guild.Channels != null)
        {
            var result = _guild.Channels.SingleOrDefault(x => x.Name == _channelName);
            if (result != null)
            {
                return result;
            }
            else
            {
                Log.WriteLine("Didn't find channel that I was looking for named: " + _channelName, LogLevel.DEBUG);
            }
        }
        else { Exceptions.GuildRefNull(); }

        return null;
    } */

    public static Task DeleteUsersChannelsOnLeave(SocketGuild _guild, SocketUser _user)
    {
        var channelToBeDeleted = _guild.Channels.First(x => x.Name.Contains("registeration_" + _user.Id));

        Log.WriteLine("Deleting channel: " + channelToBeDeleted.Name +
            " with ID: " + channelToBeDeleted.Id, LogLevel.DEBUG);

        if (channelToBeDeleted != null)
        {
            channelToBeDeleted.DeleteAsync();
        }
        // If the registeing channel is removed afterwards, maybe handle this better way.
        else
        {
            Log.WriteLine("Channel was not found, perhaps the user had registered " +
                "and left after? Implement a better way here.", LogLevel.WARNING);
        }
        return Task.CompletedTask;
    }
}