using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class BOTSTUFF : BaseCategory
{
    public BOTSTUFF()
    {
        categoryName = CategoryName.BOTSTUFF;
        channelNames = new List<ChannelName>()
        {
            ChannelName.BOTLOG,
            ChannelName.BOTCOMMANDS

        };
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        Log.WriteLine("executing permissions from BOTSTUFF", LogLevel.VERBOSE);
        return new List<Overwrite>
        {
            new Overwrite(_guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),
        };
    }
}