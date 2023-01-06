using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CHALLENGE : BaseChannel
{
    public CHALLENGE()
    {
        channelType = ChannelType.CHALLENGE;
        channelMessages = new List<MessageName>
        {
            MessageName.CHALLENGEMESSAGE,
        };
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray)
    { 
        return new List<Overwrite>
        {
        };
    }
}