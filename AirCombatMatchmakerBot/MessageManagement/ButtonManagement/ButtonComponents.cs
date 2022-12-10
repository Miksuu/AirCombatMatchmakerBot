using Discord;
using Discord.WebSocket;
using System.ComponentModel;
using System.Linq;

public static class ButtonComponents
{
    // Creates a button to a specific channel
    public static async Task<ulong> CreateButtonMessage(
        ulong _channelId,
        string _textOnTheSameMessage,
        string _label,
        string _customId)
    {
        Log.WriteLine("Creating a button on channel: " +
            "with text before the button: " + _textOnTheSameMessage + " | label: " + _label + " | custom-id:" +
            _customId, LogLevel.VERBOSE);

        var builder = new ComponentBuilder()
            .WithButton(_label, _customId);

        
        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return 0;
        }

        /*
        if (BotReference.clientRef == null)
        {
            Exceptions.BotClientRefNull();
            return 0;
        }*/

        //var textChannel = await BotReference.clientRef.GetChannelAsync(_channelId) as SocketTextChannel;

        var textChannel = guild.GetChannel(_channelId) as ITextChannel;

        if (textChannel == null)
        {
            Log.WriteLine(nameof(textChannel) + " was null!", LogLevel.CRITICAL);
            return 0;
        }

        var message = await textChannel.SendMessageAsync(
            _textOnTheSameMessage, components: builder.Build());

        ulong messageId = message.Id;
        Log.WriteLine("Created a button message with id:" + messageId, LogLevel.DEBUG);
        return messageId;
    }
}