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
        thisInterfaceCategory.CategoryType = CategoryType.BOTSTUFF;
        thisInterfaceCategory.ChannelTypes = new ConcurrentBag<ChannelType>()
        {
            ChannelType.BOTLOG,
            ChannelType.BOTCOMMANDS,
            ChannelType.TACVIEWSTORAGE,
        };
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role)
    {
        Log.WriteLine("executing permissions from BOTSTUFF");
        return new List<Overwrite>
        {
            new Overwrite(_guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),
        };
    }
}