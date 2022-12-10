using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;

[DataContract]
public class REGISTRATIONCATEGORY : BaseCategory
{
    public REGISTRATIONCATEGORY()
    {
        categoryName = CategoryName.REGISTRATIONCATEGORY;
        channels = new List<ChannelName>()
        {
            ChannelName.REGISTRATIONCHANNEL,
            ChannelName.LEAGUEREGISTRATION
        };
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        Log.WriteLine("executing permissions from REGISTRATIONCATEGORY", LogLevel.SERIALIZATION);
        return new List<Overwrite>
        {
        };
    }
}