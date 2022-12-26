using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;

[DataContract]
public abstract class BaseLeague : ILeague
{
    CategoryName ILeague.LeagueCategoryName
    {
        get => leagueCategoryName;
        set => leagueCategoryName = value;
    }

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

    public CategoryName leagueCategoryName;

    // Generated based on the implementation
    public Era leagueEra;
    public int leaguePlayerCountPerTeam;

    public List<UnitName> leagueUnits = new List<UnitName>();

    public LeagueData leagueData = new LeagueData();

    public DiscordLeagueReferences discordleagueReferences = new DiscordLeagueReferences();

    public BaseLeague()
    {
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);

}