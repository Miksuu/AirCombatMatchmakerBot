using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class REGISTRATIONCATEGORY : BaseCategory
{
    public REGISTRATIONCATEGORY()
    {
        categoryTypes = CategoryType.REGISTRATIONCATEGORY;
        channelTypes = new ConcurrentBag<ChannelType>()
        {
            ChannelType.REGISTRATIONCHANNEL,
            ChannelType.LEAGUEREGISTRATION,
        };
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role)
    {
        Log.WriteLine("executing permissions from REGISTRATIONCATEGORY", LogLevel.VERBOSE);
        return new List<Overwrite>
        {
        };
    }
}