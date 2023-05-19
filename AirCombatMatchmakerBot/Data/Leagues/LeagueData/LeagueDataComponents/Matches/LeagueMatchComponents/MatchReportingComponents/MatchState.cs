using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum MatchState
{
    // Add scheduling here (index 0)

    [EnumMember(Value = "PlayerReadyConfirmationPhase")]
    PLAYERREADYCONFIRMATIONPHASE = 0,
    
    [EnumMember(Value = "ReportingPhase")]
    REPORTINGPHASE = 1,

    [EnumMember(Value = "ConfirmationPhase")]
    CONFIRMATIONPHASE = 2,

    [EnumMember(Value = "MatchDone")]
    MATCHDONE = 3,
}