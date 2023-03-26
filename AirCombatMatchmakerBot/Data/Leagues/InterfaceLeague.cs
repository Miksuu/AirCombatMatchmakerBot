﻿using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;
using System.Collections.Concurrent;

[JsonObjectAttribute]
public interface InterfaceLeague
{
    public CategoryType LeagueCategoryName { get; set; }

    public Era LeagueEra { get; set; }
    public int LeaguePlayerCountPerTeam { get; set; }

    public ConcurrentBag<UnitName> LeagueUnits { get; set; }

    public LeagueData LeagueData { get; set; }
    public DiscordLeagueReferences DiscordLeagueReferences { get; set; }

    public abstract ConcurrentBag<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);
    public InterfaceCategory FindLeaguesInterfaceCategory();
    public void PostMatchReport(SocketGuild _guild, string _finalResult);
    public void UpdateLeagueLeaderboard();
}