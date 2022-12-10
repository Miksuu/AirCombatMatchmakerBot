using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceCategory
{
    public CategoryName CategoryName { get; set; }
    public List<ChannelName> Channels { get; set; }
    public ulong CategoryId { get; set; }
}