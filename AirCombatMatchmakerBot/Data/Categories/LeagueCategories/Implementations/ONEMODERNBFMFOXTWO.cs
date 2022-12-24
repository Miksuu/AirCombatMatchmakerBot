using Discord;
using Discord.WebSocket;
using System.Data;
using System;
using System.Runtime.Serialization;

[DataContract]
public class ONEMODERNBFMFOXTWO : BaseLeague
{
    public ONEMODERNBFMFOXTWO()
    {
        leagueName = LeagueCategoryName.ONEMODERNBFMFOXTWO;
        leagueEra = Era.MODERN;
        leaguePlayerCountPerTeam = 1;

        leagueUnits = new List<UnitName> {
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
        return new List<Overwrite> {
            new Overwrite(_guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),

            new Overwrite(_role.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Allow)) };
    }
}