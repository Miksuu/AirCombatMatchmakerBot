using Discord;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class LEAGUETEMPLATE : BaseCategory
{
    public LEAGUETEMPLATE()
    {
        thisInterfaceCategory.CategoryType = global::CategoryType.LEAGUETEMPLATE;
        thisInterfaceCategory.ChannelTypes = new ConcurrentBag<ChannelType>()
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