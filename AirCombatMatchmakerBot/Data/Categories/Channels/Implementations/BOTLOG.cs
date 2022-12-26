using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class BOTLOG : BaseChannel
{
    public BOTLOG()
    {
        channelName = ChannelName.BOTLOG;
        //botChannelType = BotChannelType.CHANNEL;
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        return new List<Overwrite>
        {
        };
    }

    public override Task ActivateChannelFeatures()
    {
        BotMessageLogging.loggingChannelId = channelId;
        return Task.CompletedTask;
    }
}