[Serializable]
public class DiscordLeagueReferences
{
    // The references to the message in the registration channel
    public ulong leagueRegistrationChannelMessageId { get; set; }

    // The reference to the category created by the system
    private ulong leagueCategoryId { get; set; }

    // The references for the channelNames inside the category
    public Dictionary<ChannelName, ulong> leagueChannels { get; set; }

    // Id of the role which gives access to the league channelNames
    public ulong leagueRoleId { get; set; }

    public DiscordLeagueReferences()
    {
        leagueChannels = new();
    }

    public ulong GetLeagueCategoryId()
    {
        Log.WriteLine("Getting id: " + leagueCategoryId, LogLevel.VERBOSE);
        return leagueCategoryId;
    }

    public void SetLeagueCategoryId(ulong _id)
    {
        Log.WriteLine("Settind id: " + leagueCategoryId + " to: " +
            leagueCategoryId,LogLevel.VERBOSE);
        leagueCategoryId = _id;
    }
}