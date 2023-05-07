using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

[JsonObjectAttribute]
public class MatchChannelComponents
{
    public InterfaceLeague? interfaceLeagueCached { get; set; }
    public LeagueMatch? leagueMatchCached { get; set; }

    public string FindMatchAndItsLeagueAndInsertItToTheCache(
        InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("Starting to find with: " + _interfaceMessage.MessageId +
            " and with category id: " + _interfaceMessage.MessageCategoryId, LogLevel.VERBOSE);

        if (interfaceLeagueCached != null || leagueMatchCached != null)
        {
            Log.WriteLine("Already cached, returning", LogLevel.VERBOSE);
            return "";
        }

        interfaceLeagueCached =
            Database.Instance.Leagues.GetILeagueByCategoryId(_interfaceMessage.MessageCategoryId);
        if (interfaceLeagueCached == null)
        {
            Log.WriteLine(nameof(interfaceLeagueCached) + " was null!", LogLevel.CRITICAL);
            return "Could not find the interface league!";
        }

        leagueMatchCached =
            interfaceLeagueCached.LeagueData.Matches.FindLeagueMatchByTheChannelId(
                _interfaceMessage.MessageChannelId);
        if (leagueMatchCached == null)
        {
            Log.WriteLine("Match with: " + _interfaceMessage.MessageChannelId +
                " was not found.", LogLevel.CRITICAL);
            return "Could not find the LeagueMatch!";
        }

        Log.WriteLine("Set: " + interfaceLeagueCached + " | " +
            leagueMatchCached, LogLevel.VERBOSE);

        return "";
    }
}