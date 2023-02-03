using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class LEAGUESTATUS : BaseChannel
{
    public LEAGUESTATUS()
    {
        channelType = ChannelType.LEAGUESTATUS;

        channelMessages = new List<MessageName>
        {
            MessageName.LEAGUESTATUSMESSAGE,
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