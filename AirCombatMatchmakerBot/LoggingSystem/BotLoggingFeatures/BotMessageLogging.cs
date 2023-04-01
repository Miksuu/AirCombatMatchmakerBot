using Discord;
using Pastel;
using System.Globalization;
using System.Runtime.CompilerServices;

public static class BotMessageLogging
{
    public static ulong loggingChannelId;

    // Send messageDescription to a specific channel in discord with the log information
    public static async void SendLogMessage(string _logMessage, LogLevel _logLevel)
    {
        string completeLogString = "";

        // Warns the admins if something is probably wrong with the bot
        if (_logLevel <= LoggingParameters.BotLogWarnAdminsLevel)
        {
            completeLogString += "WARNING <@111788167195033600>! The bot produced an log level of "
                + _logLevel.ToString() + ". Here's the log:";
        }

        completeLogString += "```" + _logMessage + "```";

        if (BotReference.GetConnectionState())
        {
            var client = BotReference.GetClientRef();
            if (client == null)
            {
                Exceptions.BotClientRefNull();
                return;
            }

            var loggingChannel = await client.GetChannelAsync(loggingChannelId) as ITextChannel;

            if (loggingChannel != null)
            {
                await loggingChannel.SendMessageAsync(completeLogString);
            }
            // Do not print anything here, might end up in circular dependency 
            // (or need to handle it, which might be unnecessary)
        }
    }
}