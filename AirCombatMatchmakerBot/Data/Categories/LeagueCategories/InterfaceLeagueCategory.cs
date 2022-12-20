using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceLeagueCategory : InterfaceCategory
{
    public LeagueCategoryName LeagueCategoryName { get; set; }
    public List<LeagueChannelName> LeagueChannelNames { get; set; }
    public List<InterfaceLeagueChannel> InterfaceLeagueChannels { get; set; }

    public Era LeagueEra { get; set; }
    public int LeaguePlayerCountPerTeam { get; set; }

    public List<UnitName> LeagueUnits { get; set; }

    public LeagueData LeagueData { get; set; }
    public DiscordLeagueReferences DiscordLeagueReferences { get; set; }
}