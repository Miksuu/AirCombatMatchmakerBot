using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Runtime.Serialization;

[JsonObjectAttribute]
public interface InterfaceEmoji
{
    public EmojiName EmojiName{ get; set; }
    public string EmojiString { get; set; }
}