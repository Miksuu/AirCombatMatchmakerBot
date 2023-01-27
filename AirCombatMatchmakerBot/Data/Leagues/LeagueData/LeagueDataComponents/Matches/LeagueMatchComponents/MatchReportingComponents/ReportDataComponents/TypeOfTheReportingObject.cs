using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum TypeOfTheReportingObject
{
    [EnumMember(Value = "reportedScore")]
    REPORTEDSCORE = 0,

    [EnumMember(Value = "tacviewLink")]
    TACVIEWLINK = 1,
}