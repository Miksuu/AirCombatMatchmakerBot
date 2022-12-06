using System.Reactive;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[DataContract]
public class BaseLeague : ILeague
{
    LeagueName ILeague.LeagueName
    {
        get => leagueName;
        set => leagueName = value;
    }

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

    int ILeague.LeaguePlayerCountPerTeam
    {
        get => leaguePlayerCountPerTeam;
        set => leaguePlayerCountPerTeam = value;
    }

    List<IUnit> ILeague.LeagueUnits
    {
        get => leagueUnits;
        set => leagueUnits = value;
    }

    // Generated based on the implementation
    public LeagueName leagueName;
    public LeagueType leagueType;
    public Era leagueEra;
    public int leaguePlayerCountPerTeam;

    public List<IUnit> leagueUnits;

    public LeagueData LeagueData;

    public BaseLeague() { }
}