using System.Runtime.Serialization;

[DataContract]
public class ONEMODERNBFMFOXTWO : BaseLeague
{
    public ONEMODERNBFMFOXTWO()
    {
        leagueName = LeagueName.ONEMODERNBFMFOXTWO;
        leagueType = LeagueType.BFM_FOXTWO;
        leagueEra = Era.MODERN;
        leaguePlayerCountPerTeam = 1;

        leagueUnits = new List<UnitName> {
            UnitName.FA18C,
            UnitName.F16C,
            UnitName.M2000C,
            UnitName.JF17,
            UnitName.SU27
        };
    }
}