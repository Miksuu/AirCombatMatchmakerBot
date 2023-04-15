using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Collections.Concurrent;

[DataContract]
public class REGISTRATIONCHANNEL : BaseChannel
{
    public REGISTRATIONCHANNEL()
    {
        channelType = ChannelType.REGISTRATIONCHANNEL;

        channelMessages = new ConcurrentDictionary<MessageName, bool>(
        new ConcurrentBag<KeyValuePair<MessageName, bool>>()
        {
            new KeyValuePair<MessageName, bool>(MessageName.REGISTRATIONMESSAGE, false),
        });
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, SocketRole _role, params ulong[] _allowedUsersIdsArray)
    {
        return new List<Overwrite>
        {
            new Overwrite(_guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(sendMessages: PermValue.Deny)),
            new Overwrite(RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
                _guild, "Member").Result.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),
        };
    }


    /*
    public Task PostChannelMessages()
    {
        return Task.CompletedTask;
    }*/
}