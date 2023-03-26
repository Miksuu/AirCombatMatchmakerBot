﻿using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class DiscordLeagueReferences
{
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

    public ulong LeagueRegistrationMessageId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueRegistrationMessageId) + leagueRegistrationMessageId, LogLevel.VERBOSE);
            return leagueRegistrationMessageId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueRegistrationMessageId) + leagueRegistrationMessageId
                + " to: " + value, LogLevel.VERBOSE);
            leagueRegistrationMessageId = value;
        }
    }

    // The reference to the category created by the system
    [DataMember] private ulong leagueCategoryId { get; set; }

    // The references for the channelTypes inside the category
    [DataMember] private ConcurrentDictionary<ChannelType, ulong> leagueChannels { get; set; }

    // Id of the role which gives access to the league channelTypes
    [DataMember] private ulong leagueRoleId { get; set; }

    // Reference to the message related to this league on the #league-registration channel
    [DataMember] private ulong leagueRegistrationMessageId { get; set; }

    public DiscordLeagueReferences()
    {
        leagueChannels = new();
    }
}