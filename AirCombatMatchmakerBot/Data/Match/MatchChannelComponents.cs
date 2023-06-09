using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

[JsonObjectAttribute]
public class MatchChannelComponents
{
    public InterfaceLeague? interfaceLeagueCached { get; set; }
    public LeagueMatch? leagueMatchCached { get; set; }

    public MatchChannelComponents()
    {
        Log.WriteLine("Default constructor!", LogLevel.CRITICAL);
    }

    public MatchChannelComponents(InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("Starting to find with: " + _interfaceMessage.MessageId +
            " and with category id: " + _interfaceMessage.MessageCategoryId, LogLevel.VERBOSE);

        if (interfaceLeagueCached != null || leagueMatchCached != null)
        {
            Log.WriteLine("Already cached, returning", LogLevel.VERBOSE);
            return;
        }

        try
        {
            interfaceLeagueCached =
                Database.Instance.Leagues.GetILeagueByCategoryId(_interfaceMessage.MessageCategoryId);
            leagueMatchCached = interfaceLeagueCached.LeagueData.Matches.FindLeagueMatchByTheChannelId(
                _interfaceMessage.MessageChannelId);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Set: " + interfaceLeagueCached + " | " +
            leagueMatchCached, LogLevel.VERBOSE);

        return;
    }

    public MatchChannelComponents(ulong _matchChannelIdCached)
    {
        ulong leagueCategoryIdCached = Database.Instance.Categories.MatchChannelsIdWithCategoryId[_matchChannelIdCached];

        Log.WriteLine("Starting to find with matchChannelId: " + _matchChannelIdCached + 
            " and with category id: " + leagueCategoryIdCached, LogLevel.VERBOSE);

        if (interfaceLeagueCached != null || leagueMatchCached != null)
        {
            Log.WriteLine("Already cached, returning", LogLevel.VERBOSE);
            return;
        }

        try
        {
            interfaceLeagueCached =
                Database.Instance.Leagues.GetILeagueByCategoryId(leagueCategoryIdCached);
            leagueMatchCached =
                interfaceLeagueCached.LeagueData.Matches.FindLeagueMatchByTheChannelId(_matchChannelIdCached);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Set: " + interfaceLeagueCached + " | " +
            leagueMatchCached, LogLevel.VERBOSE);
    }
}