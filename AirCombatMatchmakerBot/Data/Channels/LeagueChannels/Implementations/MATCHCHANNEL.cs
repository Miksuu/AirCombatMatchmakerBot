using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class MATCHCHANNEL : BaseChannel
{
    public MATCHCHANNEL()
    {
        channelType = ChannelType.MATCHCHANNEL;
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        return new List<Overwrite>
        {
        };
    }

    /*
    public Task PostChannelMessages()
    {
        return Task.CompletedTask;
    }*/
}