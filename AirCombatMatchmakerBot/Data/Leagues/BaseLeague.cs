﻿using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using System.Threading.Channels;
using System;
using Discord.Rest;

[DataContract]
public abstract class BaseLeague : InterfaceLeague
{
    CategoryType InterfaceLeague.LeagueCategoryName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueCategoryName) + ": " + leagueCategoryName, LogLevel.VERBOSE);
            return leagueCategoryName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueCategoryName) + leagueCategoryName
                + " to: " + value, LogLevel.VERBOSE);
            leagueCategoryName = value;
        }
    }

    Era InterfaceLeague.LeagueEra
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueEra) + ": " + leagueEra, LogLevel.VERBOSE);
            return leagueEra;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueEra) + leagueEra
                + " to: " + value, LogLevel.VERBOSE);
            leagueEra = value;
        }
    }

    int InterfaceLeague.LeaguePlayerCountPerTeam
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leaguePlayerCountPerTeam) + ": " + leaguePlayerCountPerTeam, LogLevel.VERBOSE);
            return leaguePlayerCountPerTeam;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leaguePlayerCountPerTeam) + leaguePlayerCountPerTeam
                + " to: " + value, LogLevel.VERBOSE);
            leaguePlayerCountPerTeam = value;
        }
    }

    List<UnitName> InterfaceLeague.LeagueUnits
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueUnits) + ": " + leagueUnits, LogLevel.VERBOSE);
            return leagueUnits;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueUnits) + leagueUnits
                + " to: " + value, LogLevel.VERBOSE);
            leagueUnits = value;
        }
    }

    LeagueData InterfaceLeague.LeagueData
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueData) + ": " + leagueData, LogLevel.VERBOSE);
            return leagueData;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueData) + leagueData
                + " to: " + value, LogLevel.VERBOSE);
            leagueData = value;
        }
    }

    DiscordLeagueReferences InterfaceLeague.DiscordLeagueReferences
    {
        get
        {
            Log.WriteLine("Getting " + nameof(discordLeagueReferences) + ": " + discordLeagueReferences, LogLevel.VERBOSE);
            return discordLeagueReferences;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(discordLeagueReferences) + discordLeagueReferences
                + " to: " + value, LogLevel.VERBOSE);
            discordLeagueReferences = value;
        }
    }

    // Generated based on the implementation
    [DataMember] protected CategoryType leagueCategoryName;
    [DataMember] protected Era leagueEra;
    [DataMember] protected int leaguePlayerCountPerTeam;
    [DataMember] protected List<UnitName> leagueUnits = new List<UnitName>();
    [DataMember] protected LeagueData leagueData = new LeagueData();
    [DataMember] protected DiscordLeagueReferences discordLeagueReferences = new DiscordLeagueReferences();

    public BaseLeague()
    {
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);

    public InterfaceCategory? FindLeaguesInterfaceCategory()
    {
        Log.WriteLine("Finding interfaceCategory in: " + leagueCategoryName +
            " with id: " + discordLeagueReferences.LeagueCategoryId, LogLevel.VERBOSE);

        InterfaceCategory interfaceCategory = 
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                discordLeagueReferences.LeagueCategoryId).Value;
        if (interfaceCategory == null)
        {
            Log.WriteLine(nameof(interfaceCategory) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found: " + interfaceCategory.CategoryType, LogLevel.VERBOSE);

        return interfaceCategory;
    }

    public async void PostMatchReport(SocketGuild _guild, string _finalResult)
    {
        InterfaceCategory leagueCategory =
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                discordLeagueReferences.LeagueCategoryId).Value;

        InterfaceChannel matchReportsChannelInterface =
            leagueCategory.FindInterfaceChannelWithNameInTheCategory(ChannelType.MATCHREPORTSCHANNEL);

        var textChannel = _guild.GetChannel(matchReportsChannelInterface.ChannelId) as ITextChannel;

        if (textChannel == null)
        {
            Log.WriteLine(nameof(textChannel) + " was null!", LogLevel.ERROR);
            return;
        }

        await textChannel.SendMessageAsync(_finalResult);
    }

    public void UpdateLeagueLeaderboard()
    {
        Log.WriteLine("Updating leaderboard on: " + leagueCategoryName, LogLevel.VERBOSE);

        var leagueStatusMessage = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            discordLeagueReferences.LeagueCategoryId).Value.FindInterfaceChannelWithNameInTheCategory(
                ChannelType.LEAGUESTATUS).FindInterfaceMessageWithNameInTheChannel(
                    MessageName.LEAGUESTATUSMESSAGE);

        if (leagueStatusMessage == null)
        {
            Log.WriteLine(nameof(leagueStatusMessage) + " was null!", LogLevel.ERROR);
            return;
        }

        leagueStatusMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("Done updating leaderboard on: " + leagueCategoryName, LogLevel.VERBOSE);
    }
}