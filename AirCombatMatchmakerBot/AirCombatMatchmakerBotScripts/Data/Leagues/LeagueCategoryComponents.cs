using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

[JsonObjectAttribute]
public class LeagueCategoryComponents
{
    public InterfaceLeague? interfaceLeagueCached { get; set; }

    public LeagueCategoryComponents(ulong _messageCategoryId)
    {
        Log.WriteLine("Starting to find with category id: " + _messageCategoryId);

        if (interfaceLeagueCached != null)
        {
            Log.WriteLine("Already cached, returning");
            return;
        }

        try
        {
            interfaceLeagueCached =
                ApplicationDatabase.Instance.Leagues.GetILeagueByCategoryId(_messageCategoryId);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            throw new InvalidOperationException(ex.Message);
        }

        Log.WriteLine("Set: " + interfaceLeagueCached);
    }
}