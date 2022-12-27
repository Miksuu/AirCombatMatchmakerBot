using Discord;
using Discord.WebSocket;

// Reference to the bot's client/guild reference variable
public static class BotReference
{
    // Use this to use the bot's functions such as the listeners
    private static DiscordSocketClient? clientRef;
    // Use guild ref to get the guild specific things, such as channels in a category
    private static SocketGuild? guildRef;
    // Hardcoded to work only on one discord server (so enter it's ID here)
    private readonly static ulong GuildID = 1047140922950942760;

    public static bool connected = false;

    public static DiscordSocketClient? GetClientRef()
    {
        if (clientRef == null)
        {
            Exceptions.BotClientRefNull();
            return null;
        }
        return clientRef;
    }

    public static DiscordSocketClient SetClientRefAndReturnIt()
    {
        // Required after some discord API change
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All
        };

        // Make the reference to a static class that has the main bot reference
        clientRef = new DiscordSocketClient(config);
        return clientRef;
    }

    public static SocketGuild? GetGuildRef()
    {
        if (clientRef == null)
        {
            Exceptions.BotClientRefNull();
            return null;
        }

        if (guildRef != null)
        {
            return guildRef;
        }
        else
        {
            guildRef = clientRef.GetGuild(GuildID);
            return guildRef;
        }
    }

    public static ulong GetGuildID()
    {
        Log.WriteLine("Getting the guild id.", LogLevel.VERBOSE);
        return GuildID;
    }
}