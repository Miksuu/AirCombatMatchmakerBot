using Discord;
using Discord.WebSocket;

public static class LeagueChannelFeatures
{
    public static async void ActivateFeatureOfTheChannel(
        ulong _channelId, LeagueCategoryChannelType _leagueCategoryChannelType)
    {
        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        var channel = guild.GetTextChannel(_channelId) as ITextChannel;

        switch ( _leagueCategoryChannelType ) 
        {
            case LeagueCategoryChannelType.CHALLENGE:
                string challengeString = "challenge_" + _channelId;

                await ButtonComponents.CreateButtonMessage(_channelId,
                    ChallengeSystem.GenerateChallengeQueueMessage(_channelId),
                    "Challenge", challengeString);
                break;

            default:
                Log.WriteLine("Ended up in default, not implemeted for the channel type: " +
                    _leagueCategoryChannelType.ToString(), LogLevel.VERBOSE);
                break;
        }
    }
}