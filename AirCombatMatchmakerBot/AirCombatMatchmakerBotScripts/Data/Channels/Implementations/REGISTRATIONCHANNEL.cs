using Discord;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Collections.Concurrent;

[DataContract]
public class REGISTRATIONCHANNEL : BaseChannel
{
    public REGISTRATIONCHANNEL()
    {
        thisInterfaceChannel.ChannelType = ChannelType.REGISTRATIONCHANNEL;

        thisInterfaceChannel.ChannelMessages = new ConcurrentDictionary<MessageName, bool>(
        new ConcurrentBag<KeyValuePair<MessageName, bool>>()
        {
            new KeyValuePair<MessageName, bool>(MessageName.REGISTRATIONMESSAGE, false),
        });
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketRole _role, params ulong[] _allowedUsersIdsArray)
    {
        var guild = BotReference.GetGuildRef();

        return new List<Overwrite>
        {
            new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(sendMessages: PermValue.Deny)),
            new Overwrite(RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
                "Member").Result.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),
        };
    }


    /*
    public Task PostChannelMessages()
    {
        return Task.CompletedTask;
    }*/
}