using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Exceptions
{
    public static void BotClientRefNull()
    {
        Log.WriteLine(nameof(BotReference.clientRef) + " was null!", LogLevel.CRITICAL);
    }

    public static void GuildRefNull()
    {
        Log.WriteLine("Guild ref was null! Wrong ID?" , LogLevel.CRITICAL);
    }
}