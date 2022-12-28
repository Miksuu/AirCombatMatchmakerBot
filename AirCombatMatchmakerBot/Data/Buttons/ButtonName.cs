using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum ButtonName

{
    [EnumMember(Value = "ENTER A BUTTON NAME HERE")]
    ENTERABUTTONNAMEHERE = 0,
}