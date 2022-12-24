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
        channelName = ChannelName.CHALLENGE;
        botChannelType = BotChannelType.LEAGUECHANNEL;
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        return new List<Overwrite>
        {
        };
    }

    public override async Task ActivateChannelFeatures()
    {
        //Log.WriteLine("Activating challenge system on channel: " + leagueChannelId, LogLevel.VERBOSE);
        //ChallengeSystem.GenerateChallengeQueueMessage(leagueChannelId);
        return Task.CompletedTask;
    }
}