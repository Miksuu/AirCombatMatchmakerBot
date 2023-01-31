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

    [EnumMember(Value = "<:RatingUp:1069164545785991189>")]
    RATINGUP = 3,

    [EnumMember(Value = "<:RatingDown:1069164544288641155>")]
    RATINGDOWN = 4,
}