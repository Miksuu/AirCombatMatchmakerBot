using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum MatchState
{
    [EnumMember(Value = "SchedulingPhase")]
    SCHEDULINGPHASE = 0,

    [EnumMember(Value = "PlayerReadyConfirmationPhase")]
    PLAYERREADYCONFIRMATIONPHASE = 1,
    
    [EnumMember(Value = "ReportingPhase")]
    REPORTINGPHASE = 2,

    [EnumMember(Value = "ConfirmationPhase")]
    CONFIRMATIONPHASE = 3,

    [EnumMember(Value = "MatchDone")]
    MATCHDONE = 4,
}