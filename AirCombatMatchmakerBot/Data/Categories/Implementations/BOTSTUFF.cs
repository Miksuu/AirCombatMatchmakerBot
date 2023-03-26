using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Collections.Concurrent;

[DataContract]
public class BOTSTUFF : BaseCategory
{
    public BOTSTUFF()
    {
        categoryTypes = CategoryType.BOTSTUFF;
        channelTypes = new ConcurrentBag<ChannelType>()
        {
            ChannelType.BOTLOG,
            ChannelType.BOTCOMMANDS
        };
    }

    public override ConcurrentBag<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role)
    {
        Log.WriteLine("executing permissions from BOTSTUFF", LogLevel.VERBOSE);
        return new ConcurrentBag<Overwrite>
        {
            new Overwrite(_guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),
        };
    }
}