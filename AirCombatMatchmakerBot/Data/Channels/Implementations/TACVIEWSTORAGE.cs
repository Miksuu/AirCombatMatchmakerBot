using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Collections.Concurrent;

[DataContract]
public class TACVIEWSTORAGE : BaseChannel
{
    public TACVIEWSTORAGE()
    {
        channelType = ChannelType.TACVIEWSTORAGE;
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, SocketRole _role, params ulong[] _allowedUsersIdsArray)
    {
        return new List<Overwrite>
        {
        };
    }
}