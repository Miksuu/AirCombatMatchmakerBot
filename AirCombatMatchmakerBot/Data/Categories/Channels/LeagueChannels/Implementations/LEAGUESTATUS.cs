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
        channelName = ChannelName.LEAGUESTATUS;
        botChannelType = BotChannelType.LEAGUECHANNEL;
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        return new List<Overwrite>
        {
        };
    }

    public override void ActivateChannelFeatures()
    {
    }
}