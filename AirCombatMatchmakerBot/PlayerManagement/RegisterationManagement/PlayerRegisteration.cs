using Discord.WebSocket;
using Discord;

public static class PlayerRegisteration
{
    public static async Task CreateMainRegisterationChannel(SocketChannel _newChannel)
    {
        Log.WriteLine("Creating the main registration channel", LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        var channel = guild.GetTextChannel(_newChannel.Id) as ITextChannel;

        // Creates the registration button
        await ButtonComponents.CreateButtonMessage(channel,
            "Click this button to register [verification process with DCS" +
            " account linking will be included later here]",
            "Register", "mainRegisteration");


        await SerializationManager.SerializeDB();
    }
}