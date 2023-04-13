using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum MessageName
{
    [EnumMember(Value = "REGISTRATION MESSAGE")]
    REGISTRATIONMESSAGE,

    [EnumMember(Value = "LEAGUE REGISTRATION MESSAGE")]
    LEAGUEREGISTRATIONMESSAGE,

    [EnumMember(Value = "CHALLENGE MESSAGE")]
    CHALLENGEMESSAGE,

    [EnumMember(Value = "MATCH START MESSAGE")]
    MATCHSTARTMESSAGE,

    [EnumMember(Value = "REPORTING MESSAGE")]
    REPORTINGMESSAGE,

    [EnumMember(Value = "CONFIRMATION MESSAGE")]
    CONFIRMATIONMESSAGE,

    [EnumMember(Value = "REPORTING STATUS MESSAGE")]
    REPORTINGSTATUSMESSAGE,

    [EnumMember(Value = "MATCH FINAL RESULT MESSAGE")]
    MATCHFINALRESULTMESSAGE,

    [EnumMember(Value = "LEAGUE STATUS MESSAGE")]
    LEAGUESTATUSMESSAGE,

    [EnumMember(Value = "MODIFY MATCH RESULTS MESSAGE")]
    MODIFYMATCHRESULTSMESSAGE,

    [EnumMember(Value = "RAW MESSAGE INPUT")]
    RAWMESSAGEINPUT,

    [EnumMember(Value = "RAW MESSAGE INPUT")]
    CONFIRMMATCHENTRYMESSAGE,    
}