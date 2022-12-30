using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum MessageName
{
    [EnumMember(Value = "REGISTRATION MESSAGE")]
    REGISTRATIONMESSAGE = 0,

    [EnumMember(Value = "LEAGUE REGISTRATION MESSAGE")]
    LEAGUEREGISTRATIONMESSAGE = 1,

    [EnumMember(Value = "CHALLENGE MESSAGE")]
    CHALLENGEMESSAGE = 2,
}