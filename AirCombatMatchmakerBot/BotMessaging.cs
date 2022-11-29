using Discord;
using Discord.WebSocket;
using static System.Net.WebRequestMethods;

public static class BotMessaging
{
    // Send message to a user with a mention
    public static async void SendMessage(
        ISocketMessageChannel _SocketMessageChannel,
        SocketMessage _SocketMessage,
        string _messageString,
        bool unknownCommand = false) // just for logging purposes
    {
        string logMessageString = "Received an message that is a ";

        if (!unknownCommand)
        {
            logMessageString += "VALID";
        }
        else
        {
            logMessageString += "UNKNOWN";
        }

        logMessageString += " command on channel: " + _SocketMessageChannel.Name +
            " | that contains: " + _SocketMessage.Content +
            " | response: " + _messageString;

        Log.WriteLine(logMessageString, LogLevel.DEBUG);

        await _SocketMessageChannel.SendMessageAsync(
            _SocketMessage.Author.Mention + ", " +
            _messageString);

        Log.WriteLine("Sending message done", LogLevel.VERBOSE);
    }

    // Send message to a specific channel in discord with the log information
    public static async void SendLogMessage(string _logMessage)
    {
        string completeLogString = "```" + _logMessage + "```";

        if (BotReference.clientRef != null && BotReference.connected)
        {
            await BotReference.clientRef.
                GetGuild(BotReference.GuildID).
                GetTextChannel(1047179975805128724).
                SendMessageAsync(completeLogString);
        }
    }
}