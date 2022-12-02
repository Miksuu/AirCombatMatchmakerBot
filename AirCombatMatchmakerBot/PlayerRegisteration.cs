using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;

public static class PlayerRegisteration
{
    public static Dictionary<string, SocketGuildUser> channelCreationQueue = new();

    public static async Task CreateANewRegisterationChannel(
        SocketGuildUser _user, SocketGuild _guild, ulong _forCategory)
    {
        string channelName = "registeration_" + _user.Id;

        // Try to find if the channel exists already (should not be the case)
        var channel = ChannelManager.FindChannel(_guild, channelName);
        if (channel == null)
        {
            if (BotReference.clientRef != null)
            {
                Log.WriteLine("Creating a channel named: " + channelName, LogLevel.DEBUG);

                var newChannel = await _guild.CreateTextChannelAsync(channelName, tcp => tcp.CategoryId = _forCategory);

                Log.WriteLine("Channel creation for: " + channelName + " done", LogLevel.VERBOSE);

                channelCreationQueue.Add(newChannel.Name, _user);

                Log.WriteLine("Added to the queue done: " + channelCreationQueue.Count, LogLevel.DEBUG);
            }
            else Exceptions.BotClientRefNull();
        }
        // If the channel exists showhow, give the permissions manually still.
        else
        {
            Log.WriteLine("This channel already exists! (should not be the case). Giving permissions anyway.", LogLevel.ERROR);
            await CreateANewRegisterationChannelManually(channelName, _user, _guild);
        }
    }

    public static async Task CreateANewRegisterationChannelManually(
        string _channelName, SocketGuildUser _user, SocketGuild _guild)
    {
        Log.WriteLine("Starting the creation of registration channelName: " + _channelName +
            " for user: " + _user.Username, LogLevel.DEBUG);
        var channel = ChannelManager.FindChannel(_guild, _channelName);
        channelCreationQueue.Add(_channelName, _user);
        await ChannelManager.HandleChannelCreation(channel);
    }
}