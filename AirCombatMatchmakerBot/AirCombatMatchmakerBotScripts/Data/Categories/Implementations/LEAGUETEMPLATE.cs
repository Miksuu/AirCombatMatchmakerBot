using Discord;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class LEAGUETEMPLATE : BaseCategory
{
    public LEAGUETEMPLATE()
    {
        thisInterfaceCategory.CategoryType = CategoryType.LEAGUETEMPLATE;
        thisInterfaceCategory.ChannelTypes = new ConcurrentBag<ChannelType>()
        {
            ChannelType.LEAGUESTATUS,
            //ChannelType.MATCHCHANNEL,
            ChannelType.MATCHREPORTSCHANNEL,
            ChannelType.CHALLENGE,
            ChannelType.MATCHSCHEDULERCHANNEL,
        };
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketRole _role)
    {
        var guild = BotReference.GetGuildRef();

        Log.WriteLine("executing permissions from LEAGUETEMPLATE");
        return new List<Overwrite>
        {
            new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),

            new Overwrite(_role.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Allow)),
        };
    }
}