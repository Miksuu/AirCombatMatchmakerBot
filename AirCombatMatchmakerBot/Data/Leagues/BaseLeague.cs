using System.Reactive;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[DataContract]
public class BaseLeague : ILeague
{
    LeagueType ILeague.LeagueType
    {
        get => leagueType;
        set => leagueType = value;
    }

    Era ILeague.LeagueEra
    {
        get => leagueEra;
        set => leagueEra = value;
    }

    List<IUnit> ILeague.LeagueUnits
    {
        get => leagueUnits;
        set => leagueUnits = value;
    }

    public LeagueType leagueType;
    public Era leagueEra;
    public List<IUnit> leagueUnits;

    public BaseLeague() { }
}