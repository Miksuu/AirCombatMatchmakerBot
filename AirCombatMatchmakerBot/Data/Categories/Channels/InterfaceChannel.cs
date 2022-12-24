﻿using Newtonsoft.Json;

[JsonObjectAttribute]
public interface InterfaceChannel
{
    public ChannelName ChannelName { get; set; }
    public ulong ChannelId { get; set; }
    public Dictionary<string, ulong> ChannelFeaturesWithMessageIds { get; set; }
    public BotChannelType BotChannelType { get; set; }
}