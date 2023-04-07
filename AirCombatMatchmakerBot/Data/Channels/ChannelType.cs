using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum ChannelType
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
    [EnumMember(Value = "tacview-storage")]
    TACVIEWSTORAGE,

    // League categories
    [EnumMember(Value = "league-status")]
    LEAGUESTATUS,
    [EnumMember(Value = "challenge-channel")]
    CHALLENGE,
    [EnumMember(Value = "match-channel")]
    MATCHCHANNEL,
    [EnumMember(Value = "match-reports")]
    MATCHREPORTSCHANNEL,
}