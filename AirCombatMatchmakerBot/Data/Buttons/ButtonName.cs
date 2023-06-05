using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum ButtonName
{
    [EnumMember(Value = "REGISTRATIONBUTTON")]
    REGISTRATIONBUTTON,

    [EnumMember(Value = "LEAGUEREGISTRATIONBUTTON")]
    LEAGUEREGISTRATIONBUTTON,

    [EnumMember(Value = "CHALLENGEBUTTON")]
    CHALLENGEBUTTON,
    [EnumMember(Value = "CHALLENGEQUEUECANCELBUTTON")]
    CHALLENGEQUEUECANCELBUTTON,

    [EnumMember(Value = "REPORTSCOREBUTTON")]
    REPORTSCOREBUTTON,

    [EnumMember(Value = "CONFIRMMATCHRESULTBUTTON")]
    CONFIRMMATCHRESULTBUTTON,

    [EnumMember(Value = "DISPUTEMATCHRESULTBUTTON")]
    DISPUTEMATCHRESULTBUTTON,

    [EnumMember(Value = "CONFIRMSCOREBUTTON")]
    CONFIRMSCOREBUTTON,

    [EnumMember(Value = "LINKBUTTON")]
    LINKBUTTON,

    /*
    [EnumMember(Value = "CONFIRMMATCHENTRYBUTTON")]
    CONFIRMMATCHENTRYBUTTON,*/

    [EnumMember(Value = "PLANESELECTIONBUTTON")]
    PLANESELECTIONBUTTON,

    [EnumMember(Value = "JOINMATCHSCHEDULER")]
    JOINMATCHSCHEDULER,

    [EnumMember(Value = "PLANESELECTIONBUTTON")]
    LEAVEMATCHSCHEDULER,
}