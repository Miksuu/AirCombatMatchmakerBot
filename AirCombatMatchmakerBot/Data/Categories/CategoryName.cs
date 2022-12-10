using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum CategoryName
{
    [EnumMember(Value = "registration")]
    REGISTRATIONCATEGORY,

    [EnumMember(Value = "bot-stuff")]
    BOTSTUFF,
}