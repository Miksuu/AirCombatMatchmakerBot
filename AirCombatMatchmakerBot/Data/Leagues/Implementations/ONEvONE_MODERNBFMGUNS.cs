using System.Runtime.Serialization;

[DataContract]
public class ONEvONE_MODERNBFMGUNS : BaseLeague
{
    public ONEvONE_MODERNBFMGUNS()
    {
        leagueName = LeagueName.MODERNBFMGUNS;
        leagueType = LeagueType.BFM_GUNS;
        leagueEra = Era.MODERN;
        leaguePlayerCountPerTeam = 1;
    }
}
