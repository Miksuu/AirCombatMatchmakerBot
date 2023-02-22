using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Reflection.Metadata.Ecma335;

[DataContract]
public class MATCHREPORTSCHANNEL : BaseChannel
{
    public MATCHREPORTSCHANNEL()
    {
        channelType = ChannelType.MATCHREPORTSCHANNEL;
        channelMessages = new Dictionary<MessageName, bool>
        {
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