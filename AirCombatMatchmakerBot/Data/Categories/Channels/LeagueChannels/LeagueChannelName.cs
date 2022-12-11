using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum LeagueChannelName
{
    [EnumMember(Value = "league-status")]
    LEAGUESTATUS = 0,

    [EnumMember(Value = "challenge-channel")]
    CHALLENGE = 1,
}