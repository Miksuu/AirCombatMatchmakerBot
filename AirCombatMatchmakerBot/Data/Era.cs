using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum Era
{
    [EnumMember(Value = "WorldWarTwo")]
    WORLD_WAR_TWO = 0,

    [EnumMember(Value = "Eighties")]
    KOREAN_WAR = 1,

    [EnumMember(Value = "ColdWar")]
    COLD_WAR = 2,

    [EnumMember(Value = "Modern")]
    MODERN = 3,
}