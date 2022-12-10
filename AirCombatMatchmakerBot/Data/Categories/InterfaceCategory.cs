using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceCategory
{
    public CategoryName CategoryName { get; set; }
    public List<ChannelName> ChannelNames { get; set; }
    public ulong CategoryId { get; set; }
    public List<InterfaceChannel> InterfaceChannels { get; set; }
}