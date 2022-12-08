public static class Exceptions
{
    public static void BotClientRefNull()
    {
        Log.WriteLine(nameof(BotReference.clientRef) + " was null!", LogLevel.CRITICAL);
    }

    public static void BotGuildRefNull()
    {
        Log.WriteLine("Guild ref was null! Wrong ID?" , LogLevel.CRITICAL);
    }
}