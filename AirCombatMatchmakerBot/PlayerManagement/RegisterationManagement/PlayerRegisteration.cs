using Discord.WebSocket;
using Discord;

public static class PlayerRegisteration
{
    public static async Task<ulong> CreateMainRegisterationChannelButton(ulong _channelId)
    {
        Log.WriteLine("Creating the main registration channel with id: " + _channelId, LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return 0;
        }

        // Creates the registration button
        var messageId = await ButtonComponents.CreateButtonMessage(_channelId,
            "Click this button to register [verification process with DCS" +
            " account linking will be included later here]",
            "Register", "mainRegistration_t"); // Needed to put something to split the string

        Log.WriteLine("Done creating the main registeration button on " + _channelId +
            " with messageId: " + messageId, LogLevel.DEBUG);

        return messageId;
    }
}