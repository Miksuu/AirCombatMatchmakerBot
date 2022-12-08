using Newtonsoft.Json;

[JsonObjectAttribute]
public interface ILeague
{
    public LeagueName LeagueName { get; set; }    
    public LeagueType LeagueType { get; set; }
    public Era LeagueEra { get; set; }
    public int LeaguePlayerCountPerTeam { get; set; }

    public List<UnitName> LeagueUnits { get; set; }

    public LeagueData LeagueData { get; set; }
    public DiscordLeagueReferences DiscordLeagueReferences { get; set; } 
}