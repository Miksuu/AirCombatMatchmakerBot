using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class TACVIEWSTORAGE : BaseChannel
{
    public TACVIEWSTORAGE()
    {
        thisInterfaceChannel.ChannelType = ChannelType.TACVIEWSTORAGE;
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketRole _role, params ulong[] _allowedUsersIdsArray)
    {
        return new List<Overwrite>
        {
        };
    }
}