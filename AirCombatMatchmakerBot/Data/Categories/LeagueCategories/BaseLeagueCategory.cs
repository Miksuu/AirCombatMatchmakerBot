using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System.Net.Http.Headers;

[DataContract]
public abstract class BaseLeagueCategory : InterfaceLeagueCategory
{
    LeagueCategoryName InterfaceLeagueCategory.LeagueCategoryName
    {
        get => leagueCategoryName;
        set => leagueCategoryName = value;
    }

    List<LeagueChannelName> InterfaceLeagueCategory.LeagueChannelNames
    {
        get => leagueChannelNames;
        set => leagueChannelNames = value;
    }

    List<InterfaceLeagueChannel> InterfaceLeagueCategory.InterfaceLeagueChannels
    {
        get => interfaceLeagueChannels;
        set => interfaceLeagueChannels = value;
    }

    LeagueType InterfaceLeagueCategory.LeagueType
    {
        get => leagueType;
        set => leagueType = value;
    }

    Era InterfaceLeagueCategory.LeagueEra
    {
        get => leagueEra;
        set => leagueEra = value;
    }

    int InterfaceLeagueCategory.LeaguePlayerCountPerTeam
    {
        get => leaguePlayerCountPerTeam;
        set => leaguePlayerCountPerTeam = value;
    }

    List<UnitName> InterfaceLeagueCategory.LeagueUnits
    {
        get => leagueUnits;
        set => leagueUnits = value;
    }

    LeagueData InterfaceLeagueCategory.LeagueData
    {
        get => leagueData;
        set => leagueData = value;
    }

    DiscordLeagueReferences InterfaceLeagueCategory.DiscordLeagueReferences
    {
        get => discordleagueReferences;
        set => discordleagueReferences = value;
    }

    public LeagueCategoryName leagueCategoryName;
    public List<LeagueChannelName> leagueChannelNames;
    public List<InterfaceLeagueChannel> interfaceLeagueChannels;

    // Generated based on the implementation
    public LeagueCategoryName leagueName;
    public LeagueType leagueType;
    public Era leagueEra;
    public int leaguePlayerCountPerTeam;

    public List<UnitName> leagueUnits = new List<UnitName>();

    public LeagueData leagueData = new LeagueData();

    public DiscordLeagueReferences discordleagueReferences = new DiscordLeagueReferences();


    public BaseLeagueCategory()
    {
        interfaceLeagueChannels = new List<InterfaceLeagueChannel>();
        leagueChannelNames = new();
    }

    public abstract List<Overwrite> GetLeagueGuildPermissions(SocketGuild _guild);
}