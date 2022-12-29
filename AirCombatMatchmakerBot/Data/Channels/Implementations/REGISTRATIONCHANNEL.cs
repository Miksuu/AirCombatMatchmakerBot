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
        channelName = ChannelName.REGISTRATIONCHANNEL;
        channelMessagesWithIds = new Dictionary<MessageName, ulong>
        {
            { MessageName.REGISTRATIONMESSAGE, 0 }
        }; 
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
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