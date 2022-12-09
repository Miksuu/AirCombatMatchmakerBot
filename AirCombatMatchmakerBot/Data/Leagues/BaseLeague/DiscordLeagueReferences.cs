[Serializable]
public class DiscordLeagueReferences
{
    // The references to the message in the registeration channel
    public ulong leagueRegisterationChannelMessageId { get; set; }

    // The reference to the category created by the system
    public ulong leagueCategoryId { get; set; }
    // The references for the channels inside the category
    public Dictionary<LeagueCategoryChannelType, ulong> leagueChannels { get; set; }

    // Id of the role which gives access to the league channels
    public ulong leagueRoleId { get; set; }

    public DiscordLeagueReferences()
    {
        leagueChannels = new();
    }
}