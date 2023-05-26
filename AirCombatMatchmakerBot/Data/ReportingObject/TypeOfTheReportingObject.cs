using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum TypeOfTheReportingObject
{
    [EnumMember(Value = "Comment")]
    COMMENTBYTHEUSER = 0,

    [EnumMember(Value = "Tacview link")]
    TACVIEWLINK = 1,

    [EnumMember(Value = "Reported score")]
    REPORTEDSCORE = 2,

    [EnumMember(Value = "Plane")]
    PLAYERPLANE = 3,
}