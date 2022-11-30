using Discord;
using Discord.WebSocket;

public static class ChannelManager
{
    public static async Task HandleChannelCreation(SocketChannel _channel)
    {
        Log.WriteLine("HANDING CHANNEL CREATION FOR CHANNEL: " + _channel.Id + ", Queue size: " +
            PlayerRegisteration.channelCreationQueue.Count, LogLevel.DEBUG);

        foreach (var kvp in PlayerRegisteration.channelCreationQueue)
        {
            SocketGuildChannel SGC = (SocketGuildChannel)_channel;

            if (kvp.Key == SGC.Name)
            {
                Log.WriteLine("Found channel: " + SGC.Name + " in " +
                    nameof(PlayerRegisteration.channelCreationQueue), LogLevel.DEBUG);

                if (BotReference.clientRef != null)
                {
                    SocketGuild guild = BotReference.clientRef.GetGuild(BotReference.GuildID);
                    await SetRegisterationChannelPermissions(kvp.Value, guild, (SocketGuildChannel)_channel);
                }
                else Exceptions.BotClientRefNull();
            }
        }
    }

    public static async Task SetRegisterationChannelPermissions(SocketGuildUser _user, SocketGuild _guild, SocketGuildChannel _channel)
    {
        // Sets permission overrides
        var permissionOverridesUser = new OverwritePermissions(viewChannel: PermValue.Allow);

        if (_channel != null)
        {
            Log.WriteLine("FOUND CHANNEL TO SET PERMISSIONS ON: " + _channel.Id, LogLevel.DEBUG);

            // Allow the channell access to the new user
            await _channel.AddPermissionOverwriteAsync(_guild.GetUser(_user.Id), permissionOverridesUser);

            PlayerRegisteration.channelCreationQueue.Remove(_channel.Name);

            Log.WriteLine("Setting permissions done. Queue size is now: " +
                PlayerRegisteration.channelCreationQueue.Count, LogLevel.DEBUG);
        }
        else
        {
            Log.WriteLine("_Channel was null!", LogLevel.CRITICAL);
        }
    }

    public static SocketGuildChannel FindChannel(SocketGuild _guild, string _channelName)
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
                Log.WriteLine("Didn't find channel that I was looking for named: " + _channelName, LogLevel.ERROR);
            }
        }
        else { Exceptions.GuildRefNull(); }
        return null;
    }
}