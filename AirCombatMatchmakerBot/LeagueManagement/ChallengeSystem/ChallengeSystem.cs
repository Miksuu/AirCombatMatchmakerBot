﻿using Discord;
using Discord.WebSocket;

public static class ChallengeSystem
{
    public static string GenerateChallengeQueueMessage(ulong _channelId)
    {
        Log.WriteLine("Generating a challenge queue message with _channelId: " + _channelId, LogLevel.VERBOSE);

        foreach (ILeague storedLeague in Database.Instance.StoredLeagues)
        {
            string? leagueName = EnumExtensions.GetEnumMemberAttrValue(storedLeague.LeagueName);

            ulong channelIdToLookFor = storedLeague.DiscordLeagueReferences.leagueChannels[
                LeagueCategoryChannelType.CHALLENGE];

            Log.WriteLine("Looping on league: " + leagueName +
                " looking for id: " +channelIdToLookFor, LogLevel.VERBOSE);

            if (_channelId == channelIdToLookFor)
            {
                Log.WriteLine("Found: " + channelIdToLookFor +
                    " is league: " + leagueName, LogLevel.DEBUG);

                string challengeMessage = ". \n" +
                    leagueName + " challenge. \n";

                return challengeMessage;
            }
        }

        Log.WriteLine("Did not find a channel id to generate a challenge queue message on!", LogLevel.ERROR);
        return string.Empty;
    }
}