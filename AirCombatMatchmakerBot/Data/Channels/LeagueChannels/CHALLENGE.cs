using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Collections.Concurrent;

[DataContract]
public class CHALLENGE : BaseChannel
{
    public CHALLENGE()
    {
        channelType = ChannelType.CHALLENGE;

        ChannelMessages = new ConcurrentDictionary<MessageName, bool>(
            new ConcurrentBag<KeyValuePair<MessageName, bool>>()
            {
                new KeyValuePair<MessageName, bool>(MessageName.CHALLENGEMESSAGE, false),
            });
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, SocketRole _role, params ulong[] _allowedUsersIdsArray)
    {
        return new List<Overwrite>
            {
                new Overwrite(
                    _guild.EveryoneRole.Id, PermissionTarget.Role,
                    new OverwritePermissions(sendMessages: PermValue.Deny, viewChannel: PermValue.Deny)),
                new Overwrite(_role.Id, PermissionTarget.Role,
                    new OverwritePermissions(viewChannel: PermValue.Allow)),
            };
    }
}