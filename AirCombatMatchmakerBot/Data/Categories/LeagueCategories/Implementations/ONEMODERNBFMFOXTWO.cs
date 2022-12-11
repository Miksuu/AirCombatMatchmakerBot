﻿using Discord;
using Discord.WebSocket;
using System.Runtime.Serialization;

[DataContract]
public class ONEMODERNBFMFOXTWO : BaseLeagueCategory
{
    public ONEMODERNBFMFOXTWO()
    {
        leagueName = LeagueCategoryName.ONEMODERNBFMFOXTWO;
        leagueType = LeagueType.BFM_FOXTWO;
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