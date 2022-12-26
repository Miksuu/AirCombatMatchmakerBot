public static class Exceptions
{
    public static void BotClientRefNull()
    {
        Log.WriteLine("ClientRef was null!", LogLevel.CRITICAL);
    }

    public static void BotGuildRefNull()
    {
        Log.WriteLine("Guild ref was null! Wrong ID?" , LogLevel.CRITICAL);
    }
}