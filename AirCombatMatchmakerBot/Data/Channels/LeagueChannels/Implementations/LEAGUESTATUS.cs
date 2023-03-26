using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Collections.Concurrent;

[DataContract]
public class LEAGUESTATUS : BaseChannel
{
    public LEAGUESTATUS()
    {
        channelType = ChannelType.LEAGUESTATUS;

        channelMessages = new ConcurrentDictionary<MessageName, bool>(
            new ConcurrentBag<KeyValuePair<MessageName, bool>>()
            {
                        new KeyValuePair<MessageName, bool>(MessageName.LEAGUESTATUSMESSAGE, false),
            });
    }

    public override ConcurrentBag<Overwrite> GetGuildPermissions(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray)
    {
        return new ConcurrentBag<Overwrite>
        {
        };
    }
}