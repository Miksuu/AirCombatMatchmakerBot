using Discord;
using Discord.WebSocket;
using System.Runtime.Serialization;

[DataContract]
public class ONEMODERNBFMGUNS : BaseLeagueCategory
{
    public ONEMODERNBFMGUNS()
    {
        leagueName = LeagueCategoryName.ONEMODERNBFMGUNS;
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

    public override List<Overwrite> GetLeagueGuildPermissions(SocketGuild _guild)
    {
        return new List<Overwrite> { };
    }
}