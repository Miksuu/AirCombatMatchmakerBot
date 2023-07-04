using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

// REMEMBER TO SET THE VALUES CORRECTLY HERE !!!
[JsonConverter(typeof(StringEnumConverter))]
public enum CategoryType
{
    [EnumMember(Value = "bot-stuff")]
    BOTSTUFF = 0,

    [EnumMember(Value = "registration")]
    REGISTRATIONCATEGORY = 1,

    [EnumMember(Value = "league-template")]
    LEAGUETEMPLATE = 2,
}