using Newtonsoft.Json;
using System.Reactive;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonObjectAttribute]
public interface ILeague
{
    public LeagueName LeagueName { get; set; }    
    public LeagueType LeagueType { get; set; }
    public Era LeagueEra { get; set; }
    public int LeaguePlayerCountPerTeam { get; set; }

    public List<UnitName> LeagueUnits { get; set; }

    public LeagueData LeagueData { get; set; }
}