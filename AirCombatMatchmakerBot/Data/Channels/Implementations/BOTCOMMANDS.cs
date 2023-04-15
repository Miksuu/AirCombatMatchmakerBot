using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Collections.Concurrent;

[DataContract]
public class BOTCOMMANDS : BaseChannel
{
    public BOTCOMMANDS()
    {
        channelType = ChannelType.BOTCOMMANDS;
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, SocketRole _role, params ulong[] _allowedUsersIdsArray)
    {
        return new List<Overwrite>
        {
        };
    }
}