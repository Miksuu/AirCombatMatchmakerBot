using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Win32.SafeHandles;
using System;
using System.Reflection.PortableExecutable;

public static class LeagueRegistrationChannelManager
{
    public static async Task CreateLeagueMessages(LEAGUEREGISTRATION _LEAGUEREGISTRATION,
        ITextChannel _leagueRegistrationChannel)
    {
        foreach (CategoryName leagueName in Enum.GetValues(typeof(CategoryName)))
        {
            Log.WriteLine("Looping on: " + leagueName.ToString(), LogLevel.VERBOSE);

            // Skip all the non-leagues
            int enumValue = (int)leagueName;
            if (enumValue < 101) continue;

            string? leagueNameString = EnumExtensions.GetEnumMemberAttrValue(leagueName);
            Log.WriteLine("leagueNameString: " + leagueNameString, LogLevel.VERBOSE);

            if (leagueNameString == null)
            {
                Log.WriteLine(nameof(leagueNameString) + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Printing all keys and values in: " + nameof(
                _LEAGUEREGISTRATION.channelFeaturesWithMessageIds) + " that has count of: " +
                _LEAGUEREGISTRATION.channelFeaturesWithMessageIds.Count, LogLevel.VERBOSE);
            foreach (var item in _LEAGUEREGISTRATION.channelFeaturesWithMessageIds)
            {
                Log.WriteLine("Key in db: " + item.Key + " with value: " + item.Value, LogLevel.VERBOSE);
            }

            // Checks if the message is present in the channelMessages
            bool containsMessage = false;
            var channelMessages = 
                await _leagueRegistrationChannel.GetMessagesAsync(50, CacheMode.AllowDownload).FirstAsync();

            Log.WriteLine("Searching: " + leagueNameString + " from: " + nameof(channelMessages) +
                " with a count of: " + channelMessages.Count, LogLevel.VERBOSE);

            foreach (var msg in channelMessages)
            {
                Log.WriteLine("Looping on msg: " + msg.Content.ToString(), LogLevel.VERBOSE);
                if (msg.Content.Contains(leagueNameString))
                {
                    Log.WriteLine($"contains: {msg.Content}", LogLevel.VERBOSE);
                    containsMessage = true;
                }
            }

            // If the channelMessages features got this already, if yes, continue, otherwise finish
            // the operation then save it to the dictionary
            if (_LEAGUEREGISTRATION.channelFeaturesWithMessageIds.ContainsKey(
                leagueNameString) && containsMessage)
            {
                Log.WriteLine("The key " + leagueNameString + " was already found in: " +
                    nameof(_LEAGUEREGISTRATION.channelFeaturesWithMessageIds) +
                    ", continuing.", LogLevel.VERBOSE);
                continue;
            }

            var leagueInterface = LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueName);
            if (leagueInterface == null)
            {
                Log.WriteLine("leagueInterface was null!", LogLevel.CRITICAL);
                return;
            }

            ulong leagueRegistrationChannelMessageId = 
                await LeagueChannelManager.CreateALeagueJoinButton(
                    _leagueRegistrationChannel, leagueInterface, leagueNameString);

            _LEAGUEREGISTRATION.channelFeaturesWithMessageIds.Add(
                leagueNameString, leagueRegistrationChannelMessageId);

            Log.WriteLine("Done looping on: " + leagueNameString, LogLevel.VERBOSE);

        }
    }
}