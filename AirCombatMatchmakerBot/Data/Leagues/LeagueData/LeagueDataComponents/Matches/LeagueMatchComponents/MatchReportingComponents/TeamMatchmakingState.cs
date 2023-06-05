using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum TeamMatchmakingState
{
    [EnumMember(Value = "In Queue")]
    INQUEUE = 0,
    
    [EnumMember(Value = "In Match")]
    INMATCH = 1,
}