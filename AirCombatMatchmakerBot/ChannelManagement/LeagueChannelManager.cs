using Discord;
using Discord.WebSocket;

public static class LeagueChannelManager
{
    public static async Task<ulong> CreateALeagueJoinButton(
        ITextChannel _leagueRegistrationChannel, ILeague? _leagueInterface, string _leagueNameString)
    {
        Log.WriteLine("Starting to create a league join button for: " + _leagueNameString, LogLevel.VERBOSE);

        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return 0;
        }

        Log.WriteLine(nameof(_leagueInterface) + " before creating leagueButtonRegisterationCustomId: "
            + _leagueInterface.ToString(), LogLevel.VERBOSE);

        string leagueButtonRegisterationCustomId =
           "leagueRegistration_" + _leagueInterface.LeagueCategoryName.ToString();

        Log.WriteLine(nameof(leagueButtonRegisterationCustomId) + ": " +
            leagueButtonRegisterationCustomId, LogLevel.VERBOSE);

        _leagueInterface = LeagueManager.GetInterfaceLeagueCategoryFromTheDatabase(_leagueInterface);

        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return 0;
        }

        _leagueInterface.DiscordLeagueReferences.leagueRegistrationChannelMessageId =
            await ButtonComponents.CreateButtonMessage(
                _leagueRegistrationChannel.Id,
                MessageManager.GenerateALeagueJoinButtonMessage(_leagueInterface),
                "Join",
                leagueButtonRegisterationCustomId); // Maybe replace this with some other system

        Log.WriteLine("Done creating a league join button for: " + _leagueNameString, LogLevel.DEBUG);

        return _leagueInterface.DiscordLeagueReferences.leagueRegistrationChannelMessageId;
    }
}