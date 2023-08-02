using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Collections.Concurrent;

[DataContract]
public class MATCHSCHEDULERCHANNEL : BaseChannel
{
    public MATCHSCHEDULERCHANNEL()
    {
        thisInterfaceChannel.ChannelType = ChannelType.MATCHSCHEDULERCHANNEL;
        thisInterfaceChannel.ChannelMessages = new ConcurrentDictionary<MessageName, bool>(
            new ConcurrentBag<KeyValuePair<MessageName, bool>>()
            {
                new KeyValuePair<MessageName, bool>(MessageName.MATCHSCHEDULERSTATUSMESSAGE, false),
            });
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