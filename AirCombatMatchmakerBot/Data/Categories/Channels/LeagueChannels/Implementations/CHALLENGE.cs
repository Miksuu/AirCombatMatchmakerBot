using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CHALLENGE : BaseLeagueChannel
{
    public CHALLENGE()
    {
        leagueChannelName = LeagueChannelName.CHALLENGE;
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        return new List<Overwrite>
        {
        };
    }

    public override Task ActivateChannelFeatures()
    {
        //Log.WriteLine("Activating challenge system on channel: " + leagueChannelId, LogLevel.VERBOSE);
        //ChallengeSystem.GenerateChallengeQueueMessage(leagueChannelId);
        return Task.CompletedTask;
    }
}