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

                SocketGuild guild = BotReference.clientRef.GetGuild(BotReference.GuildID);

                await SetChannelPermissions(kvp.Value, guild, (SocketGuildChannel)_channel);
            }
        }
    }

    /*
    private static async Task SetPermissionsForNewPlayer(
        SocketGuildUser _user, SocketGuild _guild, SocketChannel _channel)
    {
        Log.WriteLine("Starting to set permissions for the new user", LogLevel.DEBUG);
        
    } */

    public static async Task<string> CreateANewChannel(SocketGuildUser _user, SocketGuild _guild)
    {
        string channelName = "registeration-" + _user.Id;

        var channel = FindChannel(_guild, channelName);

        if (channel == null)
        {
            if (BotReference.clientRef != null)
            {
                Log.WriteLine("Creating a channel named: " + channelName, LogLevel.DEBUG);

                var newChannel = await _guild.CreateTextChannelAsync(channelName, tcp => tcp.CategoryId = 1047529896735428638);

                Log.WriteLine("Channel creation for: " + channelName + " done", LogLevel.DEBUG);

                PlayerRegisteration.channelCreationQueue.Add(newChannel.Name, _user);

                Log.WriteLine("Added to the queue done: " + PlayerRegisteration.channelCreationQueue.Count, LogLevel.DEBUG);
            }
            else Exceptions.BotClientRefNull();
        }
        else
        {
            Log.WriteLine("This channel already exists! (should not be the case)", LogLevel.WARNING);
        }

        return channelName;
    }

    public static async Task SetChannelPermissions(SocketGuildUser _user, SocketGuild _guild, SocketGuildChannel _channel)
    {
        // Sets permission overrides
        var permissionOverridesUser = new OverwritePermissions(viewChannel: PermValue.Allow);

        if (_channel != null)
        {
            Log.WriteLine("FOUND CHANNEL: " + _channel.Id, LogLevel.DEBUG);

            // Allow the channell access to the new user
            await _channel.AddPermissionOverwriteAsync(_guild.GetUser(_user.Id), permissionOverridesUser);

            PlayerRegisteration.channelCreationQueue.Remove(_channel.Name);

            Log.WriteLine("Setting permissions done. Queue size is now: " +
                PlayerRegisteration.channelCreationQueue.Count, LogLevel.DEBUG);
        }
        else
        {
            Log.WriteLine("Channel " + _channel.Id + " null!", LogLevel.CRITICAL);
        }

        /*
        foreach (var ch in _guild.Channels)
        {
            Log.WriteLine("Looping through channel name: " + ch.Name, LogLevel.VERBOSE);

            if (_channelName == ch.Name)
            {

            }
        }*/
    }

    public static SocketGuildChannel FindChannel(SocketGuild _guild, string _channelName)
    {
        if (_guild != null)
        {
            return _guild.Channels.SingleOrDefault(x => x.Name == _channelName);
        }
        else Exceptions.GuildRefNull();

        return null;
    }
}