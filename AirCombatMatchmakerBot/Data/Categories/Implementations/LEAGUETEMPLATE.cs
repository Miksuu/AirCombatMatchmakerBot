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
        categoryTypes = CategoryType.LEAGUETEMPLATE;
        channelTypes = new List<ChannelType>()
        {
            ChannelType.LEAGUESTATUS,
            ChannelType.CHALLENGE,
            ChannelType.MATCHCHANNEL,
            ChannelType.MATCHREPORTSCHANNEL,
        };
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, SocketRole _role)
    {
        Log.WriteLine("executing permissions from LEAGUETEMPLATE", LogLevel.VERBOSE);
        return new List<Overwrite>
        {
            new Overwrite(_guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),

            new Overwrite(_role.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Allow)),
        };
    }
}