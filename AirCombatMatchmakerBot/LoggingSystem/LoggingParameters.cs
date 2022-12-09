public static class LoggingParameters
{
    // Prevents the messages below certain level appearing in the #log channel
    public static LogLevel BotLogDiscordChannelLevel = LogLevel.WARNING;

    // Warns the admins (pings) them in the #log channel
    // if something was logged by the bot that is an warning or abové for example
    public static LogLevel BotLogWarnAdminsLevel = LogLevel.WARNING;
}