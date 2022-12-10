using System.Runtime.Serialization;

[DataContract]
public class ONEMODERNBFMGUNS : BaseLeague
{
    public ONEMODERNBFMGUNS()
    {
        leagueName = LeagueName.ONEMODERNBFMGUNS;
        leagueType = LeagueType.BFM_GUNS;
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