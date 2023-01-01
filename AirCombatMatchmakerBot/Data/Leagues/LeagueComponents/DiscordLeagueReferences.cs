using System.Runtime.Serialization;

[DataContract]
public class DiscordLeagueReferences
{
    /*
    public ulong LeagueRegistrationChannelMessageId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueRegistrationChannelMessageId) +
                leagueRegistrationChannelMessageId, LogLevel.VERBOSE);
            return leagueRegistrationChannelMessageId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueRegistrationChannelMessageId) +
                leagueRegistrationChannelMessageId + " to: " + value, LogLevel.VERBOSE);
            leagueRegistrationChannelMessageId = value;
        }
    }*/

    public ulong LeagueCategoryId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueCategoryId) + leagueCategoryId, LogLevel.VERBOSE);
            return leagueCategoryId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueCategoryId) + leagueCategoryId
                + " to: " + value, LogLevel.VERBOSE);
            leagueCategoryId = value;
        }
    }


    public Dictionary<ChannelName, ulong> LeagueChannels
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueChannels) + " with count of: " +
                leagueChannels.Count, LogLevel.VERBOSE);
            return leagueChannels;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueChannels)
                + " to: " + value, LogLevel.VERBOSE);
            leagueChannels = value;
        }
    }

    public ulong LeagueRoleId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueRoleId) + leagueRoleId, LogLevel.VERBOSE);
            return leagueRoleId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueRoleId) + leagueRoleId
                + " to: " + value, LogLevel.VERBOSE);
            leagueRoleId = value;
        }
    }

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
}