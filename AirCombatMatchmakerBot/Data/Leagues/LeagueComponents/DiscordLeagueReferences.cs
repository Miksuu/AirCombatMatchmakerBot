using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class DiscordLeagueReferences
{
    public ulong LeagueCategoryId
    {
        get
        {
            return leagueCategoryId.GetValue();
        }
        set
        {
            leagueCategoryId.SetValue(value);
        }
    }

    public ConcurrentDictionary<ChannelType, ulong> LeagueChannels
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
            return leagueRoleId.GetValue();
        }
        set
        {
            leagueRoleId.SetValue(value);
        }
    }

    public ulong LeagueRegistrationMessageId
    {
        get
        {
            return leagueRegistrationMessageId.GetValue();
        }
        set
        {
            leagueRegistrationMessageId.SetValue(value);
        }
    }

    // The reference to the category created by the system
    [DataMember] private logUlong leagueCategoryId = new logUlong();

    // The references for the channelTypes inside the category
    [DataMember] private ConcurrentDictionary<ChannelType, ulong> leagueChannels { get; set; }

    // Id of the role which gives access to the league channelTypes
    [DataMember] private logUlong leagueRoleId = new logUlong();

    // Reference to the messageDescription related to this league on the #league-registration channel
    [DataMember] private logUlong leagueRegistrationMessageId = new logUlong();

    public DiscordLeagueReferences()
    {
        leagueChannels = new();
    }
}