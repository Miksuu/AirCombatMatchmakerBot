using Discord.WebSocket;

public static class CommandHandler
{
    public static async Task HandleCommand(SocketMessage _Message)
    {
        ISocketMessageChannel MessageChannel = _Message.Channel;
        ulong senderID = _Message.Author.Id;

        var message = _Message.Content;

        Console.WriteLine("messageTest: " + message + " | " + message.ToString());

        Console.WriteLine("Message received from: " + senderID + " in: " + MessageChannel);

        Console.WriteLine("Received message! in " + MessageChannel);

        // Returns if the message doesn't start with the prefix, or the author is a bot
        if ((!_Message.Content.StartsWith('!') || _Message.Author.IsBot)) return;

        //if (senderID != ulongHere) return;

        // Split in to the parameters
        string[] cmdParameters = _Message.Content.Split(' ');

        foreach (var msgPart in cmdParameters)
        {
            Console.WriteLine("msgPart: " + msgPart);
        }

        Console.WriteLine("msg content: " + _Message.Content.ToString());

        // The main switch case for handling the commands
        switch (cmdParameters[0]) // The first part of the message, the command
        {
            case "!test":
                await MessageChannel.SendMessageAsync(_Message.Author.Mention +
                    ", https://tenor.com/view/wow-omg-meme-suprise-gif-24927190");
                break;
            default:
                await MessageChannel.SendMessageAsync("Unknown command!");
                break;
        }
        return;
    }
}