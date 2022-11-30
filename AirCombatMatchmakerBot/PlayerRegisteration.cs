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
    public static async Task HandleUserJoin(SocketGuildUser _user)
    {
        if (!_user.IsBot)
        {
            Log.WriteLine("User: " + _user + " has joined the discord with id: " + _user.Id +
                " starting the registation process", LogLevel.DEBUG);

            SocketGuild guild = BotReference.clientRef.GetGuild(BotReference.GuildID);

            // Creates a private channel for the user to proceed with the registeration
            string chName = await ChannelManager.CreateANewChannel(_user, guild);

            await SetPermissionsForNewPlayer(_user, guild, chName);
        }
        else
        {
            Log.WriteLine("A bot: " + _user.Nickname +
                " joined the discord, disregarding the registeration process", LogLevel.WARNING);
        }
    }

    private static async Task SetPermissionsForNewPlayer(SocketGuildUser _user, SocketGuild _guild, string _channelName)
    {
        Log.WriteLine("Starting to set permissions for the new user", LogLevel.DEBUG);
        await ChannelManager.SetChannelPermissions(_user, _guild, _channelName);
    }
}