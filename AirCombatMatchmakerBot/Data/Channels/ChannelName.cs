using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum ChannelName
{
    // Registration category
    [EnumMember(Value = "registration")]
    REGISTRATIONCHANNEL,
    [EnumMember(Value = "league-registration")]
    LEAGUEREGISTRATION,

    // Bot-stuff category
    [EnumMember(Value = "bot-commands")]
    BOTCOMMANDS,
    [EnumMember(Value = "bot-log")]
    BOTLOG,

    // League categories
    [EnumMember(Value = "league-status")]
    LEAGUESTATUS,
    [EnumMember(Value = "challenge-channel")]
    CHALLENGE,
    [EnumMember(Value = "match-channel")]
    MATCHCHANNEL,
}