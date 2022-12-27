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
            GenerateChallengeQueueMessage(),
            "CHALLENGE!",
            channelFeatureKey + "_" + channelsCategoryId);

        channelFeaturesWithMessageIds.Add(channelFeatureKey, buttonId);

        Log.WriteLine("Done activating channel features on " +
            nameof(CHALLENGE) + " id: " + base.channelId, LogLevel.VERBOSE);
    }

    public string GenerateChallengeQueueMessage()
    {
        Log.WriteLine("Generating a challenge queue message with _channelId: " +
            channelId, LogLevel.VERBOSE);

        foreach (var createdCategoriesKvp in
            Database.Instance.Categories.GetDictionaryOfCreatedCategoriesWithChannels())
        {
            Log.WriteLine("On league: " + createdCategoriesKvp.Value.CategoryName, LogLevel.VERBOSE);

            string leagueName =
                EnumExtensions.GetEnumMemberAttrValue(createdCategoriesKvp.Value.CategoryName);

            Log.WriteLine("Full league name: " + leagueName, LogLevel.VERBOSE);

            if (createdCategoriesKvp.Value.InterfaceChannels.Any(
                    x => x.ChannelId == channelId))
            {
                ulong channelIdToLookFor =
                    createdCategoriesKvp.Value.InterfaceChannels.First(
                        x => x.ChannelId == channelId).ChannelId;

                Log.WriteLine("Looping on league: " + leagueName +
                    " looking for id: " + channelIdToLookFor, LogLevel.VERBOSE);

                if (channelId == channelIdToLookFor)
                {
                    Log.WriteLine("Found: " + channelIdToLookFor +
                        " is league: " + leagueName, LogLevel.DEBUG);

                    string challengeMessage = ". \n" +
                        leagueName + " challenge. Players In The Queue:. \n";

                    return challengeMessage;
                }
            }
        }

        Log.WriteLine(
            "Did not find a channel id to generate a challenge queue message on!", LogLevel.ERROR);
        return string.Empty;
    }
}