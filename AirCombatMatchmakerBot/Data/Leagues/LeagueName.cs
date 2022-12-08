using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum LeagueName
{
    [EnumMember(Value = "1v1 Modern BFM Guns")]
    ONEMODERNBFMGUNS,

    [EnumMember(Value = "1v1 Modern BFM Fox2")]
    ONEMODERNBFMFOXTWO,

    /*
    [EnumMember(Value = "2v2 Modern BVR")]
    TWOMODERNBVR,*/
}