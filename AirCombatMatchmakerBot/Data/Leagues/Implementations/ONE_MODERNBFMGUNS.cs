using System.Runtime.Serialization;

[DataContract]
public class ONE_MODERNBFMGUNS : BaseLeague
{
    public ONE_MODERNBFMGUNS()
    {
        leagueName = LeagueName.ONE_MODERNBFMGUNS;
        leagueType = LeagueType.BFM_GUNS;
        leagueEra = Era.MODERN;
        leaguePlayerCountPerTeam = 1;
    }
}
