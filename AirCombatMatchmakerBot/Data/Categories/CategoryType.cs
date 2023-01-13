using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

// REMEMBER TO SET THE VALUES CORRECTLY HERE !!!

[JsonConverter(typeof(StringEnumConverter))]
public enum CategoryType
{
    [EnumMember(Value = "1v1 Modern BFM Guns")]
    ONEMODERNBFMGUNS = 0,

    [EnumMember(Value = "1v1 Modern BFM Fox2")]
    ONEMODERNBFMFOXTWO = 1,

    // Categories. They have value larger than 100 so during league creation
    // those other CategoryNames won't be made a league
    [EnumMember(Value = "bot-stuff")]
    BOTSTUFF = 101,

    [EnumMember(Value = "registration")]
    REGISTRATIONCATEGORY = 102,

    [EnumMember(Value = "league-template")]
    LEAGUETEMPLATE = 103,
}