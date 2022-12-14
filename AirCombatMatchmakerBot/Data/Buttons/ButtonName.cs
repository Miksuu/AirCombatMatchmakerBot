using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum ButtonName

{
    [EnumMember(Value = "REGISTRATIONBUTTON")]
    REGISTRATIONBUTTON = 0,

    [EnumMember(Value = "LEAGUEREGISTRATIONBUTTON")]
    LEAGUEREGISTRATIONBUTTON = 1,

    [EnumMember(Value = "CHALLENGEBUTTON")]
    CHALLENGEBUTTON = 2,

    [EnumMember(Value = "REPORTSCOREBUTTON")]
    REPORTSCOREBUTTON = 3,
}