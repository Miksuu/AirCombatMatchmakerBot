using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class LEAGUETEMPLATE : BaseCategory
{
    public LEAGUETEMPLATE()
    {
        categoryName = CategoryName.BOTSTUFF;
        channelNames = new List<ChannelName>()
        {
            ChannelName.LEAGUESTATUS,
            ChannelName.CHALLENGE,
        };
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        Log.WriteLine("executing permissions from LEAGUETEMPLATE", LogLevel.VERBOSE);
        return new List<Overwrite>
        {
            new Overwrite(_guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),
        };
    }
}