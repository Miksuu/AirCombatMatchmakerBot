using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class LEAGUEREGISTRATION : BaseChannel
{
    public LEAGUEREGISTRATION()
    {
        channelName = ChannelName.LEAGUEREGISTRATION;
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        return new List<Overwrite>
        {
            new Overwrite(_guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),
        };
    }

    public override async Task ActivateChannelFeatures()
    {
    }
}