using Discord;
using Discord.WebSocket;
using System.ComponentModel;
using System.Linq;

public static class ButtonFunctionality
{
    public static InterfaceLeague? FindLeagueInterfaceWithSplitStringPart(
        string _splitStringIdPart)
    {
        Log.WriteLine("Starting to find Ileague from db with: " +
            _splitStringIdPart, LogLevel.VERBOSE);

        KeyValuePair<ulong, InterfaceCategory> findLeagueCategoryType =
            Database.Instance.Categories.GetCreatedCategoryWithChannelKvpWithString(
                _splitStringIdPart);
        CategoryName leagueCategoryName = findLeagueCategoryType.Value.CategoryName;

        Log.WriteLine("found: " + nameof(leagueCategoryName) + ": " +
            leagueCategoryName.ToString(), LogLevel.VERBOSE);

        var leagueInterface =
            LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueCategoryName);

        Log.WriteLine(
            "Found interface " + nameof(leagueInterface) + ": " +
            leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

        InterfaceLeague? dbLeagueInstance =
            Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(leagueInterface);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine(nameof(dbLeagueInstance) + " db: " +
            dbLeagueInstance.LeagueCategoryName, LogLevel.VERBOSE);
        return dbLeagueInstance;
    }

    public static async Task PostChallenge(
        SocketMessageComponent _component, string _splitString)
    {
        Log.WriteLine("Starting processing a challenge by: " + _component.User.Id +
            " for league: " + _splitString, LogLevel.VERBOSE);

        InterfaceLeague? dbLeagueInstance = FindLeagueInterfaceWithSplitStringPart(_splitString);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return;
        }

        dbLeagueInstance.LeagueData.PostChallengeToThisLeague(
            _component.User.Id, dbLeagueInstance.LeaguePlayerCountPerTeam);
    }
}