using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum LeagueType
{
    [EnumMember(Value = "BfmGuns")]
    BFM_GUNS = 0,

    [EnumMember(Value = "BfmFoxTwo")]
    BFM_FOXTWO = 1,

    [EnumMember(Value = "Bvr")]
    BVR = 2,
}