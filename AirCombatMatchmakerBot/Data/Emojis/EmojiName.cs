using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum EmojiName
{
    [EnumMember(Value = ":white_check_mark:")]
    WHITECHECKMARK = 0,

    [EnumMember(Value = ":yellow_square:")]
    YELLOWSQUARE = 1,

    [EnumMember(Value = ":red_square:")]
    REDSQUARE = 2,
}