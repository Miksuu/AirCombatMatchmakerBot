using Discord.WebSocket;
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

    public ulong LeagueCategoryId { get; set; }
    public ConcurrentDictionary<ChannelType, ulong> LeagueChannels { get; set; }
    public ulong LeagueRoleId { get; set; }
    public ulong LeagueRegistrationMessageId { get; set; }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);
    public InterfaceCategory? FindLeaguesInterfaceCategory();
    public Task PostMatchReport(string _finalResultMessage, string _finalResultTitle,
        AttachmentData[] _attachmentDatas);
    public void UpdateLeagueLeaderboard();

    public Task<Response> RegisterUserToALeague(ulong _userId);
}