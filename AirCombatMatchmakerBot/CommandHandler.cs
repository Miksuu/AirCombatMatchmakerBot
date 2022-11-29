using Discord.WebSocket;

public static class CommandHandler
{
    public static Task HandleCommand(SocketMessage _Message)
    {
        ISocketMessageChannel MessageChannel = _Message.Channel;
        //ulong senderID = _Message.Author.Id;

        // IF YOU WANT TO DO ANY LOGGING ABOVE THIS LINE,
        // NEED TO MAKE A CIRCULAR DEPENDENCY FIX OR CONFIRM THAT THE MESSAGE IS NOT IN THE #log CHANNEL

        // Returns if the message doesn't start with the prefix, or the author is a bot
        if ((!_Message.Content.StartsWith('!') || _Message.Author.IsBot)) return Task.CompletedTask;

        //Log.WriteLine("messageTest: " + message + " | " + message.ToString(), LogLevel.DEBUG);

        Log.WriteLine("Message received from: " + _Message.Author.Id + " in: " + MessageChannel, LogLevel.VERBOSE);

        Log.WriteLine("Received message! in " + MessageChannel, LogLevel.VERBOSE);

        //if (senderID != ulongHere) return;

        // Split in to the parameters
        string[] cmdParameters = _Message.Content.Split(' ');

        // Just for logging
        foreach (var msgPart in cmdParameters)
        {
            Log.WriteLine("msgPart: " + msgPart, LogLevel.VERBOSE);
        }

        Log.WriteLine("msg content: " + _Message.Content.ToString(), LogLevel.VERBOSE);

        // The main switch case for handling the commands
        switch (cmdParameters[0]) // The first part of the message, the command
        {
            case "!cat":
                BotMessaging.SendMessage(MessageChannel, _Message, "https://tenor.com/view/war-dimden-cute-cat-mean-gif-22892687");
                break;
            default:
                BotMessaging.SendMessage(MessageChannel, _Message, "Unknown command.", true);
                break;
        }

        return Task.CompletedTask;
    }
}