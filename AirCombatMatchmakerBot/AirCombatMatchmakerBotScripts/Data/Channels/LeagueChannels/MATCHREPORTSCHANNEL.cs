using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Collections.Concurrent;

[DataContract]
public class MATCHREPORTSCHANNEL : BaseChannel
{
    public MATCHREPORTSCHANNEL()
    {
        thisInterfaceChannel.ChannelType = ChannelType.MATCHREPORTSCHANNEL;
        thisInterfaceChannel.ChannelMessages = new ConcurrentDictionary<MessageName, bool>
        {
        };
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketRole _role, params ulong[] _allowedUsersIdsArray)
    {
        var guild = BotReference.GetGuildRef();

        return new List<Overwrite>
            {
                new Overwrite(
                    guild.EveryoneRole.Id, PermissionTarget.Role,
                    new OverwritePermissions(sendMessages: PermValue.Deny, viewChannel: PermValue.Deny)),
                new Overwrite(_role.Id, PermissionTarget.Role,
                    new OverwritePermissions(viewChannel: PermValue.Allow)),
            };
    }
}