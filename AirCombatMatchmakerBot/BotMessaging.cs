﻿using Discord;
using Discord.WebSocket;

public static class BotMessaging
{
    // Send message to a user with a mention
    public static string GetResponse(
        SocketSlashCommand _command,
        string _messageString)
    {
        string logMessageString = "Received an message that is a command on channel: " + _command.Channel +
            " | that contains: " + _command.Data.Name +
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
            await BotReference.clientRef.
                GetGuild(BotReference.GuildID).
                GetTextChannel(1047179975805128724).
                SendMessageAsync(completeLogString);
        }
    }

    // Creates a button to a specific channel
    public static async void CreateButton(SocketGuildChannel _channel,
        string _textOnTheSameMessage,
        string _label,
        string _customId)
    {
        var builder = new ComponentBuilder()
            .WithButton(_label, _customId);

        if (BotReference.clientRef != null)
        {
            var textChannel = BotReference.clientRef.GetGuild(BotReference.GuildID).GetTextChannel(_channel.Id);

            if (textChannel != null) 
            {
                await textChannel.SendMessageAsync(_textOnTheSameMessage, components: builder.Build());
            }
            else
            {
                Log.WriteLine(nameof(textChannel) + " was null!", LogLevel.CRITICAL);
            }
        }
        else Exceptions.BotClientRefNull();
    }
}