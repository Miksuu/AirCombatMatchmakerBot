using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum MessageName

{
    [EnumMember(Value = "ENTER A MESSAGE NAME HERE")]
    ENTERAMESSAGENAMEHERE = 0,
}