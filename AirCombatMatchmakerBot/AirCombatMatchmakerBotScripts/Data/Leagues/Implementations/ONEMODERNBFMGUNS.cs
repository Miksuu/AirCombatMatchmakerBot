﻿// using Discord;
// using Discord.WebSocket;
// using System.Runtime.Serialization;
// using System.Collections.Concurrent;

// [DataContract]
// public class ONEMODERNBFMGUNS : BaseLeague
// {
//     public ONEMODERNBFMGUNS()
//     {
//         thisInterfaceLeague.LeagueCategoryName = LeagueName.ONEMODERNBFMGUNS;
//         thisInterfaceLeague.LeagueEra = Era.MODERN;
//         thisInterfaceLeague.LeaguePlayerCountPerTeam = 1;

//         thisInterfaceLeague.LeagueUnits = new ConcurrentBag<UnitName> {
//             UnitName.FA18C,
//             UnitName.F16C,
//             UnitName.M2000C,
//             UnitName.JF17,
//             UnitName.SU27,
//             UnitName.F14B,
//         };
//     }

//     public override List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role)
//     {
//         return new List<Overwrite>();
//     }
// }