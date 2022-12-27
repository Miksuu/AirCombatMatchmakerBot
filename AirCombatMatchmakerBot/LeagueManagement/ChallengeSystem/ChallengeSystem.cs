using Discord;
using Discord.WebSocket;

public static class ChallengeSystem
{
    public static string GenerateChallengeQueueMessage(ulong _channelId)
    {
        Log.WriteLine("Generating a challenge queue message with _channelId: " +
            _channelId, LogLevel.VERBOSE);

        foreach (var createdCategoriesKvp in 
            Database.Instance.Categories.GetDictionaryOfCreatedCategoriesWithChannels())
        {
            Log.WriteLine("On league: " + createdCategoriesKvp.Value.CategoryName, LogLevel.VERBOSE);

            string leagueName =
                EnumExtensions.GetEnumMemberAttrValue(createdCategoriesKvp.Value.CategoryName);

            Log.WriteLine("Full league name: " + leagueName, LogLevel.VERBOSE);

            if (createdCategoriesKvp.Value.InterfaceChannels.Any(
                    x => x.ChannelId == _channelId))
            {
                ulong channelIdToLookFor =
                    createdCategoriesKvp.Value.InterfaceChannels.First(
                        x => x.ChannelId == _channelId).ChannelId;

                Log.WriteLine("Looping on league: " + leagueName +
                    " looking for id: " + channelIdToLookFor, LogLevel.VERBOSE);

                if (_channelId == channelIdToLookFor)
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