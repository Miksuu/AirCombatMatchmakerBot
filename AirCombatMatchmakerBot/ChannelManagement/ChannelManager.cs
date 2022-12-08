﻿using Discord;
using Discord.WebSocket;

public static class ChannelManager
{
    static List<SocketChannel> waitingChannels = new();
    public static async Task FinishChannelCreationFromDelegate(SocketChannel _newChannel)
    {
        Log.WriteLine("Delegate fired. " + nameof(DowntimeManager.useWaitingChannels) +
            ": " + DowntimeManager.useWaitingChannels, LogLevel.DEBUG); ;
        if (DowntimeManager.useWaitingChannels)
        {
            waitingChannels.Add(_newChannel);
        }
        else await HandleOneChannel(_newChannel);
    }

    public static async Task CreateChannelsFromWaitingChannels()
    {
        Log.WriteLine("Creating channels from the waiting channels. Count: " +
            waitingChannels.Count, LogLevel.DEBUG);
        foreach (SocketChannel _newChannel in waitingChannels)
        {
            await HandleOneChannel(_newChannel);
        }
    }

    public static async Task HandleOneChannel(SocketChannel _newChannel)
    {
        Log.WriteLine("Invoked newChannel creation", LogLevel.DEBUG);
        if (PlayerRegisteration.channelQueue.ContainsKey(_newChannel.Id))
        {
            var guild = BotReference.GetGuildRef();

            if (guild != null)
            {
                var channel = guild.GetTextChannel(_newChannel.Id) as ITextChannel;

                // Place the newly created id to the object of non registered user
                PlayerRegisteration.channelQueue[_newChannel.Id].discordRegisterationChannelId = _newChannel.Id;

                string channelName = PlayerRegisteration.channelQueue[_newChannel.Id].ConstructChannelName();

                // Sets the players permissions to be accessible
                // (ASSUMES THAT THE CHANNEL GROUP IS PRIVATE BY DEFAULT)
                await ChannelPermissionsManager.SetRegisterationChannelPermissions(
                    PlayerRegisteration.channelQueue[_newChannel.Id].discordUserId, channel);
                // Creates the registeration button
                await ButtonComponents.CreateButtonMessage(channel,
                    "Click this button to register [verification process with DCS" +
                    " account linking will be included later here]",
                    "Register", channelName);

                Log.WriteLine("Channel creation for: " + channelName + " done", LogLevel.VERBOSE);

                PlayerRegisteration.channelQueue.Remove(_newChannel.Id);
                Log.WriteLine("Removed from the queue done: " + PlayerRegisteration.channelQueue.Count, LogLevel.DEBUG);
            }
            else Exceptions.BotGuildRefNull();
        }
        else
        {
            Log.WriteLine("Does not contain key for: " + _newChannel.Id, LogLevel.WARNING);
        }

        await SerializationManager.SerializeDB();
    }

    public static Task DeleteUsersRegisterationChannel(ulong _userId)
    {
        var guild = BotReference.GetGuildRef();

        if (guild != null)
        {
            string channelToLookFor = "registeration_" + _userId;
            SocketGuildChannel? foundChannel = null;

            foreach (SocketGuildChannel channel in guild.Channels)
            {
                if (channel.Name == channelToLookFor)
                {
                    foundChannel = channel;
                    break;
                }
            }

            if (foundChannel != null)
            {
                Log.WriteLine("Deleting channel: " + foundChannel.Name +
                    " with ID: " + foundChannel.Id, LogLevel.DEBUG);

                // Remove the player's channel
                foundChannel.DeleteAsync();
            }
            // If the registering channel is removed afterwards, maybe handle this better way.
            else
            {
                Log.WriteLine("Channel was not found, perhaps the user had registered " +
                    "and left after? Implement a better way here.", LogLevel.WARNING);
            }
            return Task.CompletedTask;
        }
        else Exceptions.BotGuildRefNull();

        return Task.CompletedTask;
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
        else { Exceptions.BotGuildRefNull(); }

        return null;
    } */

}