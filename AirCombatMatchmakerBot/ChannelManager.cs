using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;

public static class ChannelManager
{
    public static async Task<string> CreateANewChannel(SocketGuildUser _user, SocketGuild _guild)
    {
        string channelName = "registeration-" + _user.Id;

        if (BotReference.clientRef != null)
        {
            Log.WriteLine("Creating a channel named: " + channelName, LogLevel.DEBUG);

            await _guild.CreateTextChannelAsync(channelName, tcp => tcp.CategoryId = 1047529896735428638);

            Log.WriteLine("Channel creation for: " + channelName + " done", LogLevel.DEBUG);
        }
        else Exceptions.BotClientRefNull();

        return channelName;
    }

    public static async Task SetChannelPermissions(SocketGuildUser _user, SocketGuild _guild, string _channelName)
    {
        // Sets permission overrides
        var permissionOverridesEveryone = new OverwritePermissions(viewChannel: PermValue.Deny);
        var permissionOverridesUser = new OverwritePermissions(viewChannel: PermValue.Allow);

        // Finds the channel that has been created
        var channel = _guild.Channels.SingleOrDefault(x => x.Name == _channelName);

        if (channel != null)
        {
            Log.WriteLine("FOUND CHANNEL: " + channel.Name, LogLevel.WARNING);

            // Deny the channel access for everyone else
            await channel.AddPermissionOverwriteAsync(_guild.EveryoneRole, permissionOverridesEveryone);

            // Allow the channell access to the new user
            await channel.AddPermissionOverwriteAsync(_guild.GetUser(_user.Id), permissionOverridesUser);

            Log.WriteLine("Setting permissions done.", LogLevel.DEBUG);
        }
        else
        {
            Log.WriteLine("Channel " + _channelName + " null!", LogLevel.CRITICAL);
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
}