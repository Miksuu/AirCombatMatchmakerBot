using Discord.WebSocket;

public static class CommandHandler
{
    public static async Task HandleCommand(SocketMessage _Message)
    {
        ISocketMessageChannel MessageChannel = _Message.Channel;
        ulong senderID = _Message.Author.Id;

        var message = _Message.Content;

        // IF YOU WANT TO DO ANY LOGGING ABOVE THIS LINE,
        // NEED TO MAKE A CIRCULAR DEPENDENCY FIX OR CONFIRM THAT THE MESSAGE IS NOT IN THE #log CHANNEL

        // Returns if the message doesn't start with the prefix, or the author is a bot
        if ((!_Message.Content.StartsWith('!') || _Message.Author.IsBot)) return;

        Log.WriteLine("messageTest: " + message + " | " + message.ToString(), LogLevel.DEBUG);

        Log.WriteLine("Message received from: " + senderID + " in: " + MessageChannel, LogLevel.DEBUG);

        Log.WriteLine("Received message! in " + MessageChannel, LogLevel.DEBUG);

        //if (senderID != ulongHere) return;

        // Split in to the parameters
        string[] cmdParameters = _Message.Content.Split(' ');

        foreach (var msgPart in cmdParameters)
        {
            Log.WriteLine("msgPart: " + msgPart, LogLevel.DEBUG);
        }

        Log.WriteLine("msg content: " + _Message.Content.ToString(), LogLevel.DEBUG);

        // The main switch case for handling the commands
        switch (cmdParameters[0]) // The first part of the message, the command
        {
            case "!cat":
                BotMessaging.SendMessage(MessageChannel, _Message, "https://tenor.com/view/war-dimden-cute-cat-mean-gif-22892687");
                break;
            default:
                await MessageChannel.SendMessageAsync("Unknown command!");
                break;
        }
        return;
    }
}