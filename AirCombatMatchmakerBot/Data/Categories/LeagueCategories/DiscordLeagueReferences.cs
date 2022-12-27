using System.Runtime.Serialization;

[DataContract]
public class DiscordLeagueReferences
{
    public ulong LeagueRegistrationChannelMessageId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueRegistrationChannelMessageId) +
                leagueRegistrationChannelMessageId, LogLevel.VERBOSE);
            return leagueRoleId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueRegistrationChannelMessageId) +
                leagueRegistrationChannelMessageId + " to: " + value, LogLevel.VERBOSE);
            leagueRegistrationChannelMessageId = value;
        }
    }

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

    /*
    public ulong GetLeagueRegistrationChannelMessageId()
    {
        Log.WriteLine("Getting id: " + leagueRegistrationChannelMessageId, LogLevel.VERBOSE);
        return leagueRegistrationChannelMessageId;
    }
    public void SetLeagueRegistrationChannelMessageId(ulong _id)
    {
        Log.WriteLine("Setting id: " + leagueRegistrationChannelMessageId + " to: " +
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
        Log.WriteLine("Setting id: " + leagueCategoryId + " to: " + _id, LogLevel.VERBOSE);
        leagueCategoryId = _id;
    }

    public ulong GetLeagueRoleId()
    {
        Log.WriteLine("Getting id: " + leagueRoleId, LogLevel.VERBOSE);
        return leagueRoleId;
    }

    public void SetLeagueRoleId(ulong _id)
    {
        Log.WriteLine("Setting id: " + leagueRoleId + " to: " +
            _id, LogLevel.VERBOSE);
        leagueRoleId = _id;
    }*/
}