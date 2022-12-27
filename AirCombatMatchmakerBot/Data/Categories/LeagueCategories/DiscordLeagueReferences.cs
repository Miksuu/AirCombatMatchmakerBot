using System.Runtime.Serialization;

[DataContract]
public class DiscordLeagueReferences
{
    // The references to the message in the registration channel
    [DataMember] private ulong leagueRegistrationChannelMessageId { get; set; }

    // The reference to the category created by the system
    [DataMember] private ulong leagueCategoryId { get; set; }

    // The references for the channelNames inside the category
    [DataMember] private Dictionary<ChannelName, ulong> leagueChannels { get; set; }

    // Id of the role which gives access to the league channelNames
    [DataMember] private ulong leagueRoleId { get; set; }

    public DiscordLeagueReferences()
    {
        leagueChannels = new();
    }
    public ulong GetLeagueRegistrationChannelMessageId()
    {
        Log.WriteLine("Getting id: " + leagueRegistrationChannelMessageId, LogLevel.VERBOSE);
        return leagueRegistrationChannelMessageId;
    }
    public void SetLeagueRegistrationChannelMessageId(ulong _id)
    {
        Log.WriteLine("Settind id: " + leagueRegistrationChannelMessageId + " to: " +
            _id, LogLevel.VERBOSE);
        leagueRegistrationChannelMessageId = _id;
    }

    public ulong GetLeagueCategoryId()
    {
        Log.WriteLine("Getting id: " + leagueCategoryId, LogLevel.VERBOSE);
        return leagueCategoryId;
    }

    public void SetLeagueCategoryId(ulong _id)
    {
        Log.WriteLine("Settind id: " + leagueCategoryId + " to: " +
            _id, LogLevel.VERBOSE);
        leagueCategoryId = _id;
    }

    public ulong GetLeagueRoleId()
    {
        Log.WriteLine("Getting id: " + leagueRoleId, LogLevel.VERBOSE);
        return leagueRoleId;
    }

    public void SetLeagueRoleId(ulong _id)
    {
        Log.WriteLine("Settind id: " + leagueRoleId + " to: " +
            _id, LogLevel.VERBOSE);
        leagueRoleId = _id;
    }
}