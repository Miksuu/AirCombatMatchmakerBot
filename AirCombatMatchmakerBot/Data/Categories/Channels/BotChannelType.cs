using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonConverter(typeof(StringEnumConverter))]
public enum BotChannelType
{
    CHANNEL,
    LEAGUECHANNEL,
}