using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System.Net.Http.Headers;


[DataContract]
public abstract class BaseLeague : ILeague
{
    LeagueCategoryName ILeague.LeagueCategoryName
    {
        get => leagueCategoryName;
        set => leagueCategoryName = value;
    }

    /*
    List<ChannelName> ILeague.LeagueChannelNames
    {
        get => leagueChannelNames;
        set => leagueChannelNames = value;
    }

    List<InterfaceChannel> ILeague.InterfaceLeagueChannels
    {
        get => interfaceLeagueChannels;
        set => interfaceLeagueChannels = value;
    }*/

    Era ILeague.LeagueEra
    {
        get => leagueEra;
        set => leagueEra = value;
    }

    int ILeague.LeaguePlayerCountPerTeam
    {
        get => leaguePlayerCountPerTeam;
        set => leaguePlayerCountPerTeam = value;
    }

    List<UnitName> ILeague.LeagueUnits
    {
        get => leagueUnits;
        set => leagueUnits = value;
    }

    LeagueData ILeague.LeagueData
    {
        get => leagueData;
        set => leagueData = value;
    }

    DiscordLeagueReferences ILeague.DiscordLeagueReferences
    {
        get => discordleagueReferences;
        set => discordleagueReferences = value;
    }

    public LeagueCategoryName leagueCategoryName;
    public List<ChannelName> leagueChannelNames;
    public List<InterfaceChannel> interfaceLeagueChannels;

    // Generated based on the implementation
    public LeagueCategoryName leagueName;
    public Era leagueEra;
    public int leaguePlayerCountPerTeam;

    public List<UnitName> leagueUnits = new List<UnitName>();

    public LeagueData leagueData = new LeagueData();

    public DiscordLeagueReferences discordleagueReferences = new DiscordLeagueReferences();

    public BaseLeague()
    {
        interfaceLeagueChannels = new List<InterfaceChannel>();
        leagueChannelNames = new();
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);
}