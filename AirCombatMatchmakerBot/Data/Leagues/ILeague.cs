using Newtonsoft.Json;
using System.Reactive;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonObjectAttribute]
public interface ILeague
{
    public LeagueType LeagueType { get; set; }
    public Era LeagueEra { get; set; }

    public List<IUnit> LeagueUnits { get; set; }
}