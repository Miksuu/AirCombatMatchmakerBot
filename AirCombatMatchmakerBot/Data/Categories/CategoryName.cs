using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum CategoryName
{
    [EnumMember(Value = "bot-stuff")]
    BOTSTUFF,

    [EnumMember(Value = "registration")]
    REGISTRATIONCATEGORY,

    [EnumMember(Value = "league-template")]
    LEAGUETEMPLATE,


    // Leagues. They have value larger than 100 so during league creation
    // those other CategoryNames won't be tried to make a league
    [EnumMember(Value = "1v1 Modern BFM Guns")]
    ONEMODERNBFMGUNS = 101,

    [EnumMember(Value = "1v1 Modern BFM Fox2")]
    ONEMODERNBFMFOXTWO = 102,
}