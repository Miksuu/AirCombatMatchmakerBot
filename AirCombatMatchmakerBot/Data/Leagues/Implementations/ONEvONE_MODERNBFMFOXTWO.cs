using System.Runtime.Serialization;

[DataContract]
public class ONEvONE_MODERNBFMFOXTWO : BaseLeague
{
    public ONEvONE_MODERNBFMFOXTWO()
    {
        leagueName = LeagueName.MODERNBFMGUNS;
        leagueType = LeagueType.BFM_FOXTWO;
        leagueEra = Era.MODERN;
        leaguePlayerCountPerTeam = 1;
    }
}
