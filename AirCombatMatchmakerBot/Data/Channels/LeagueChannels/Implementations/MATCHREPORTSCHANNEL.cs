using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Collections.Concurrent;

[DataContract]
public class MATCHREPORTSCHANNEL : BaseChannel
{
    public MATCHREPORTSCHANNEL()
    {
        channelType = ChannelType.MATCHREPORTSCHANNEL;
        channelMessages = new ConcurrentDictionary<MessageName, bool>
        {
        };
    }

    public override ConcurrentBag<Overwrite> GetGuildPermissions(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray)
    {
        return new ConcurrentBag<Overwrite>
        {
        };
    }
}