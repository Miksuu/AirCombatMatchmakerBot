using Discord;
using Discord.WebSocket;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class ONEMODERNBFMFOXTWO : BaseLeague
{
    public ONEMODERNBFMFOXTWO()
    {
        leagueCategoryName = CategoryType.ONEMODERNBFMFOXTWO;
        leagueEra = Era.MODERN;
        thisInterfaceLeague.LeaguePlayerCountPerTeam = 1;

        thisInterfaceLeague.LeagueUnits = new ConcurrentBag<UnitName> {
            UnitName.FA18C,
            UnitName.F16C,
            UnitName.M2000C,
            UnitName.JF17,
            UnitName.SU27,
            UnitName.F14B,
        };
    }

    public override List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role)
    {
        return new List<Overwrite>();
    }
}