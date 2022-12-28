using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class BOTCOMMANDS : BaseChannel
{
    public BOTCOMMANDS()
    {
        channelName = ChannelName.BOTCOMMANDS;
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        return new List<Overwrite>
        {
        };
    }

    public override Task ActivateChannelFeatures()
    {
        return Task.CompletedTask;
    }
}