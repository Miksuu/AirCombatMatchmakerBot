using Discord;

public static class Exceptions
{
    public static string BotClientRefNull()
    {
        string errorMessage = "ClientRef was null!";
        Log.WriteLine(errorMessage, LogLevel.CRITICAL);
        return errorMessage;
    }

    public static string BotGuildRefNull()
    {
        string errorMessage = "Guild ref was null!";
        Log.WriteLine(errorMessage + " wrong ID probably", LogLevel.CRITICAL);
        return errorMessage;
    }
}