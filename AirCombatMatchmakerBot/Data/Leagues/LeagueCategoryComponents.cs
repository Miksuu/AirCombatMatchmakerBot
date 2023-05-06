using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

[JsonObjectAttribute]
public class LeagueCategoryComponents
{
    public InterfaceLeague? interfaceLeagueCached { get; set; }

    public string FindCategorysLeagueAndInsertItToTheCache(
        ulong _messageCategoryId)
    {
        Log.WriteLine("Starting to find with category id: " + _messageCategoryId, LogLevel.VERBOSE);

        if (interfaceLeagueCached != null)
        {
            Log.WriteLine("Already cached, returning", LogLevel.VERBOSE);
            return "";
        }
        interfaceLeagueCached =
            Database.Instance.Leagues.GetILeagueByCategoryId(_messageCategoryId);
        if (interfaceLeagueCached == null)
        {
            Log.WriteLine(nameof(interfaceLeagueCached) + " was null!", LogLevel.CRITICAL);
            return "Could not find the interface league!";
        }

        Log.WriteLine("Set: " + interfaceLeagueCached, LogLevel.VERBOSE);

        return "";
    }
}