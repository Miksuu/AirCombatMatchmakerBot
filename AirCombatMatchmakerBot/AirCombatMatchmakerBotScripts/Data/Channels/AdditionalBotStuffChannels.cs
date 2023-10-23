using System.Collections.Concurrent;

public static class AdditionalBotStuffChannels
{
    public static ConcurrentBag<ChannelType> additionalChannelTypes = new ConcurrentBag<ChannelType>()
    {
        ChannelType.TACVIEWSTORAGE,
    };
}