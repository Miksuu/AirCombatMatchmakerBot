using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum CommandName
{
    [EnumMember(Value = "COMMENT")]
    COMMENT = 0,

    [EnumMember(Value = "REMOVEPLAYER")]
    REMOVEPLAYER = 1,

    [EnumMember(Value = "SCHEDULE")]
    SCHEDULE = 2,
}