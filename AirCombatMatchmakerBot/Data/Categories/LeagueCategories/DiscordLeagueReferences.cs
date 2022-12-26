[Serializable]
public class DiscordLeagueReferences
{
    // The references to the message in the registration channel
    public ulong leagueRegistrationChannelMessageId { get; set; }

    // The reference to the category created by the system
    public ulong leagueCategoryId { get; set; }

    // The references for the channelNames inside the category
    public Dictionary<ChannelName, ulong> leagueChannels { get; set; }

    // Id of the role which gives access to the league channelNames
    public ulong leagueRoleId { get; set; }

    public DiscordLeagueReferences()
    {
        leagueChannels = new();
    }
}