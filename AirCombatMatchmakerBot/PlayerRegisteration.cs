using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public static class PlayerRegisteration
{
    public static Dictionary<string, SocketGuildUser> channelCreationQueue = new();

    public static async Task HandleUserJoin(SocketGuildUser _user)
    {
        if (!_user.IsBot)
        {
            Log.WriteLine("User: " + _user + " has joined the discord with id: " + _user.Id +
                " starting the registation process", LogLevel.DEBUG);

            if (BotReference.clientRef != null)
            {
                SocketGuild guild = BotReference.clientRef.GetGuild(BotReference.GuildID);

                // Creates a private channel for the user to proceed with the registeration 
                await CreateANewRegisterationChannel(_user, guild, 1047529896735428638);
            }
            else Exceptions.BotClientRefNull();
        }
        else
        {
            Log.WriteLine("A bot: " + _user.Nickname +
                " joined the discord, disregarding the registeration process", LogLevel.WARNING);
        }
    }

    public static async Task CreateANewRegisterationChannel(SocketGuildUser _user, SocketGuild _guild, ulong _forCategory)
    {
        string channelName = "registeration-" + _user.Id;

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
            channelCreationQueue.Add(channelName, _user);
            await ChannelManager.HandleChannelCreation(channel);
        }
    }
}