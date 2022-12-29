using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum MessageName

{
    [EnumMember(Value = "REGISTRATION MESSAGE")]
    REGISTRATIONMESSAGE = 0,
}