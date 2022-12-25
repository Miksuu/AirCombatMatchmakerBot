using Discord;
using Discord.WebSocket;
using System;

public static class LeagueRegistrationChannelManager
{
    public static async Task CreateLeagueMessages(LEAGUEREGISTRATION _LEAGUEREGISTRATION,
        ITextChannel _leagueRegistrationChannel)
    {
        foreach (CategoryName leagueName in Enum.GetValues(typeof(CategoryName)))
        {
            Log.WriteLine("Looping on: " + leagueName.ToString(), LogLevel.VERBOSE);

            string? leagueNameString = EnumExtensions.GetEnumMemberAttrValue(leagueName);
            if (leagueNameString == null)
            {
                Log.WriteLine(nameof(leagueNameString) + " was null!", LogLevel.CRITICAL);
                return;
            }

            // If the channel features got this already, if yes, continue, otherwise finish
            // the operation then save it to the dictionary
            if (_LEAGUEREGISTRATION.channelFeaturesWithMessageIds.ContainsKey(
                leagueNameString))
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