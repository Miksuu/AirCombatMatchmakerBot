//using Discord.Commands;
using Discord.WebSocket;

public static class BotReference
{
    public static DiscordSocketClient? clientRef;
    public static SocketGuild? guildRef;
    public static ulong GuildID = 1047140922950942760;
    public static bool connected = false;

    public static SocketGuild? GetGuildRef()
    {
        if (clientRef != null)
        {
            if (guildRef == null)
            {
                guildRef = clientRef.GetGuild(GuildID);
                return guildRef;
            }
            else return guildRef;

        } else Exceptions.BotClientRefNull();
        return null;
    }
}