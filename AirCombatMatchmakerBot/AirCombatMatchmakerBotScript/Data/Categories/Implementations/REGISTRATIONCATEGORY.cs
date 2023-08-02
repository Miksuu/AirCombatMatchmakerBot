using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class REGISTRATIONCATEGORY : BaseCategory
{
    public REGISTRATIONCATEGORY()
    {
        thisInterfaceCategory.CategoryType = CategoryType.REGISTRATIONCATEGORY;
        thisInterfaceCategory.ChannelTypes = new ConcurrentBag<ChannelType>()
        {
            ChannelType.REGISTRATIONCHANNEL,
            ChannelType.LEAGUEREGISTRATION,
        };
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role)
    {
        Log.WriteLine("executing permissions from REGISTRATIONCATEGORY");
        return new List<Overwrite>
        {
        };
    }
}