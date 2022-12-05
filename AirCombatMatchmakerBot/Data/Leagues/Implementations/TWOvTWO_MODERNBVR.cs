using System.Runtime.Serialization;

[DataContract]
public class TWOvTWO_MODERNBVR : BaseLeague
{
    public TWOvTWO_MODERNBVR()
    {
        leagueName = LeagueName.MODERNBFMGUNS;
        leagueType = LeagueType.BFM_FOXTWO;
        leagueEra = Era.MODERN;
        leaguePlayerCountPerTeam = 2;
    }
}