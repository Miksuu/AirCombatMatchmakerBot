using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Channels;

public static class BotMessaging
{
    // Send message to a user with a mention
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

    // Send message to a specific channel in discord with the log information
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

        if (BotReference.clientRef != null && BotReference.connected)
        {
            var guild = BotReference.GetGuildRef();

            if (guild != null)
            {
                await guild.
                    GetTextChannel(1047179975805128724).
                    SendMessageAsync(completeLogString);
            }
            else Exceptions.BotGuildRefNull();
        }
    }

    // Creates a button to a specific channel
    public static async Task<ulong> CreateButtonMessage(
        ITextChannel _channel,
        string _textOnTheSameMessage,
        string _label,
        string _customId)
    {
        Log.WriteLine("Creating a button on channel: " +
            "with text before the button: " + _textOnTheSameMessage + " | label: " + _label + " | custom-id:" +
            _customId, LogLevel.DEBUG);

        var builder = new ComponentBuilder()
            .WithButton(_label, _customId);

        if (BotReference.clientRef != null)
        {
            var guild = BotReference.GetGuildRef();

            if (guild != null)
            {
                var textChannel = guild.GetTextChannel(_channel.Id);

                if (textChannel != null)
                {
                    var message = await textChannel.SendMessageAsync(
                        _textOnTheSameMessage, components: builder.Build());

                    ulong messageId = message.Id;
                    Log.WriteLine("Created a button message with id:" + messageId, LogLevel.VERBOSE);
                    return messageId;
                }
                else
                {
                    Log.WriteLine(nameof(textChannel) + " was null!", LogLevel.CRITICAL);
                }
            }
            else Exceptions.BotGuildRefNull();
        }
        else Exceptions.BotClientRefNull();

        return 0;
    }

    private static async Task ModifyMessage(
        ulong _channelId, ulong _messageId, string _content)
    {
        Log.WriteLine("Modifying a message on channel id: " + _channelId + " that has msg id: " +
            _messageId + " with content: " + _content, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild != null)
        {
            var channel = guild.GetTextChannel(_channelId) as ITextChannel;

            await channel.ModifyMessageAsync(_messageId, m => m.Content = _content);

            Log.WriteLine("Modifying the message: " + _messageId + " done.", LogLevel.VERBOSE);
        }
        else Exceptions.BotGuildRefNull();
    }

    public static async Task ModifyLeagueRegisterationChannelMessage(ILeague _dbLeagueInstance)
    {
        Log.WriteLine("Modifying league registeration channel message with: " +
            _dbLeagueInstance.LeagueName, LogLevel.VERBOSE);

        await ModifyMessage(1049555859656671232, // Hardcoded
            _dbLeagueInstance.LeagueData.leagueChannelMessageId,
         LeagueManager.GenerateALeagueJoinButtonMessage(_dbLeagueInstance));
    }
}