using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class LEAGUESTATUS : BaseLeagueChannel
{
    public LEAGUESTATUS()
    {
        leagueChannelName = LeagueChannelName.LEAGUESTATUS;
    }

    public override List<Overwrite> GetGuildLeaguePermissions(SocketGuild _guild)
    {
        return new List<Overwrite>
        {
        };
    }

    public override async Task ActivateLeagueChannelFeatures()
    {
    }
}