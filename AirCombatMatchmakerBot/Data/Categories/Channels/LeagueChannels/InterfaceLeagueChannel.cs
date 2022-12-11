using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceLeagueChannel
{
    public LeagueChannelName LeagueChannelName { get; set; }
    public ulong LeagueChannelId { get; set; }
    public Dictionary<string, ulong> LeagueChannelFeaturesWithMessageIds { get; set; }
}