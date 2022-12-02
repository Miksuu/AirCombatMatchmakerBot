using Discord;
using Discord.WebSocket;

public static class ChannelManager
{


    public static async Task HandleChannelCreationFromDelegate(SocketChannel _newChannel)
    {
        if (_newChannel != null)
        {
            NonRegisteredUser _nonRegisteredUser = Database.Instance.NonRegisteredUsers.Find(
                x => x.discordRegisterationChannelId == _newChannel.Id);

            if (_nonRegisteredUser != null) 
            {
                await HandleChannelCreationManually(_nonRegisteredUser);
            }
            else
            {
                Log.WriteLine(nameof(_nonRegisteredUser) + " was null!", LogLevel.CRITICAL);
            }
        }    
        else
        {
            Log.WriteLine(nameof(_newChannel) + " was null!", LogLevel.CRITICAL);
        }
    }

    public static async Task HandleChannelCreationManually(NonRegisteredUser _nonRegisteredUser)
    {


        Log.WriteLine("HANDING CHANNEL CREATION FOR CHANNEL: " + _nonRegisteredUser.discordRegisterationChannelId +
            "discordUserId: " + _nonRegisteredUser.discordUserId, LogLevel.DEBUG);

        if (BotReference.clientRef != null)
        {
            SocketGuild guild = BotReference.clientRef.GetGuild(BotReference.GuildID);
            SocketGuildChannel channel = guild.GetTextChannel(_nonRegisteredUser.discordRegisterationChannelId);

            // Sets the players permissions to be accessible
            // (ASSUMES THAT THE CHANNEL GROUP IS PRIVATE BY DEFAULT)
            await SetRegisterationChannelPermissions(_nonRegisteredUser.discordUserId, guild, channel);
            // Creates the registeration button
            BotMessaging.CreateButton(channel,
                "Click this button to register [verification process with DCS" +
                " account linking will be included later here]",
                "Register", channel.Name);
        }
        else Exceptions.BotClientRefNull();

        /*
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
        }*/
    }

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