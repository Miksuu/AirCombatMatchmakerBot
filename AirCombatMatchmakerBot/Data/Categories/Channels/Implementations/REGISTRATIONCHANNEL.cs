using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class REGISTRATIONCHANNEL : BaseChannel
{
    public REGISTRATIONCHANNEL()
    {
        channelName = ChannelName.REGISTRATIONCHANNEL;
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        return new List<Overwrite>
        {
        };
    }
}