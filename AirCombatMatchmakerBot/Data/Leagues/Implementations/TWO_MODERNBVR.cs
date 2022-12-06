using System.Runtime.Serialization;

[DataContract]
public class TWO_MODERNBVR : BaseLeague
{
    public TWO_MODERNBVR()
    {
        leagueName = LeagueName.TWO_MODERNBVR;
        leagueType = LeagueType.BFM_FOXTWO;
        leagueEra = Era.MODERN;
        leaguePlayerCountPerTeam = 2;
    }
}