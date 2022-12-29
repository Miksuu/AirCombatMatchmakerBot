using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceLeague
{
    public CategoryName LeagueCategoryName { get; set; }

    public Era LeagueEra { get; set; }
    public int LeaguePlayerCountPerTeam { get; set; }

    public List<UnitName> LeagueUnits { get; set; }

    public LeagueData LeagueData { get; set; }
    public DiscordLeagueReferences DiscordLeagueReferences { get; set; }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);
    public string GenerateALeagueJoinButtonMessage();
    public string GetAllowedUnitsAsString();
    public string GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface();
    public Task ModifyLeagueRegisterationChannelMessage();
    public string GenerateALeagueChallengeButtonMessage();
}