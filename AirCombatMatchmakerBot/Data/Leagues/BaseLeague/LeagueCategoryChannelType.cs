using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum LeagueCategoryChannelType
{
    [EnumMember(Value = "main-channel")]
    MAIN = 0,

    [EnumMember(Value = "test-channel")]
    TEST = 1,

    [EnumMember(Value = "test2-channel")]
    TESTTWO = 2,
}