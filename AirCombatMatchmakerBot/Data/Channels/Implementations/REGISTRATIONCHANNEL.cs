using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class REGISTRATIONCHANNEL : BaseChannel
{
    public REGISTRATIONCHANNEL()
    {
        channelType = ChannelType.REGISTRATIONCHANNEL;
        channelMessages = new Dictionary<MessageName, bool>
        {
            { MessageName.REGISTRATIONMESSAGE, false }, 
        }; 
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray)
    {
        return new List<Overwrite>
        {
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