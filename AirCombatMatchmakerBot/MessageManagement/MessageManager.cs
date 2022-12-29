using Discord;

public static class MessageManager
{
    public static async Task ModifyChallengeChannelMessage(ulong _targetChannel)
    {
        /*
        await ModifyMessage(LeagueManager.leagueRegistrationChannelId,
            _dbLeagueInstance.DiscordLeagueReferences.leagueRegistrationChannelMessageId,
         GenerateALeagueJoinButtonMessage(_dbLeagueInstance)); */
    }


    public static async Task ModifyMessage(
        ulong _channelId, ulong _messageId, string _content)
    {
        Log.WriteLine("Modifying a message on channel id: " + _channelId + " that has msg id: " +
            _messageId + " with content: " + _content, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        var channel = guild.GetTextChannel(_channelId) as ITextChannel;

        await channel.ModifyMessageAsync(_messageId, m => m.Content = _content);

        Log.WriteLine("Modifying the message: " + _messageId + " done.", LogLevel.VERBOSE);
    }
}