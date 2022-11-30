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
            SocketGuildChannel SocketGuildChannel = (SocketGuildChannel)_channel;

            if (kvp.Key == SocketGuildChannel.Name)
            {
                Log.WriteLine("Found channel: " + SocketGuildChannel.Name + " in " +
                    nameof(PlayerRegisteration.channelCreationQueue), LogLevel.DEBUG);

                if (BotReference.clientRef != null)
                {
                    SocketGuild guild = BotReference.clientRef.GetGuild(BotReference.GuildID);

                    // Sets the players permissions to be accessible
                    // (ASSUMES THAT THE CHANNEL GROUP IS PRIVATE BY DEFAULT)
                    await SetRegisterationChannelPermissions(kvp.Value, guild, SocketGuildChannel);
                    // Creates the registeration button
                    BotMessaging.CreateButton(SocketGuildChannel,
                        "Click this button to register [verification process with DCS" +
                        " account linking will be included later here]",
                        "Register", SocketGuildChannel.Name);
                }
                else Exceptions.BotClientRefNull();
            }
        }
    }

    public static async Task SetRegisterationChannelPermissions(
        SocketGuildUser _user, SocketGuild _guild, SocketGuildChannel _channel)
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
                Log.WriteLine("Didn't find channel that I was looking for named: " + _channelName, LogLevel.DEBUG);
            }
        }
        else { Exceptions.GuildRefNull(); }
        return null;
    }

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