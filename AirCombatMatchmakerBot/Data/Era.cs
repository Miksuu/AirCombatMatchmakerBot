using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum Era
{
    [EnumMember(Value = "World War Two")]
    WORLD_WAR_TWO = 0,

    [EnumMember(Value = "Korean War")]
    KOREAN_WAR = 1,

    [EnumMember(Value = "Cold War")]
    COLD_WAR = 2,

    [EnumMember(Value = "Modern")]
    MODERN = 3,
}