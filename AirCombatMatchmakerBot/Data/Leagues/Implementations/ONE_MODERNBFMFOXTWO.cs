using System.Runtime.Serialization;

[DataContract]
public class ONE_MODERNBFMFOXTWO : BaseLeague
{
    public ONE_MODERNBFMFOXTWO()
    {
        leagueName = LeagueName.ONE_MODERNBFMGUNS;
        leagueType = LeagueType.BFM_FOXTWO;
        leagueEra = Era.MODERN;
        leaguePlayerCountPerTeam = 1;

        leagueUnits = new List<UnitName> {
            UnitName.FA18C,
            UnitName.F16C,
            UnitName.M2000C
        };
    }
}