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
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild)
    {
        return new List<Overwrite>
        {
        };
    }

    public override async Task ActivateChannelFeatures()
    {
        Log.WriteLine("Activating challenge system on channel: " + channelId, LogLevel.VERBOSE);

        string channelFeatureKey = "challenge";

        if (channelFeaturesWithMessageIds.ContainsKey(channelFeatureKey))
        {
            Log.WriteLine("Already contains key " + channelFeatureKey, LogLevel.VERBOSE);
            return;
        }

        ulong buttonId = await ButtonComponents.CreateButtonMessage(
            channelId,
            ChallengeSystem.GenerateChallengeQueueMessage(channelId),
            "CHALLENGE!",
            channelFeatureKey + "_" + channelsCategoryId);

        channelFeaturesWithMessageIds.Add(channelFeatureKey, buttonId);

        Log.WriteLine("Done activating channel features on " +
            nameof(CHALLENGE) + " id: " + base.channelId, LogLevel.VERBOSE);
    }
}