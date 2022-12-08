using Discord.WebSocket;

public static class BotReference
{
    public static DiscordSocketClient? clientRef;
    public static SocketGuild? guildRef;
    public static ulong GuildID = 1047140922950942760;
    public static bool connected = false;

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
}