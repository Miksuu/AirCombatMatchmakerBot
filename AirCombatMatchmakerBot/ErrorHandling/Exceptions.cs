public static class Exceptions
{
    public static string BotClientRefNull()
    {
        string errorMessage = "ClientRef was null!";
        Log.WriteLine(errorMessage, LogLevel.CRITICAL);
        return errorMessage;
    }

    public static void BotGuildRefNull()
    {
        Log.WriteLine("Guild ref was null! Wrong ID?" , LogLevel.CRITICAL);
    }
}