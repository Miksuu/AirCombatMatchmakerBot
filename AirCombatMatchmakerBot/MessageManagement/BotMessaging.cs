public static class BotMessaging
{
    // Send messageDescription to a user with a mention
    public static string GetMessageResponse(
        string _commandDataName,
        string _messageString,
        string _channel)
    {
        string logMessageString = "Received an message that is a command on channel: " + _channel +
            " | that contains: " + _commandDataName +
            " | response: " + _messageString;

        Log.WriteLine(logMessageString, LogLevel.DEBUG);

        return _messageString;
    }
}