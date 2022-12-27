using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;

[DataContract]
public abstract class BaseCategory : InterfaceCategory
{
    CategoryName InterfaceCategory.CategoryName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(categoryName) + ": " + categoryName, LogLevel.VERBOSE);
            return categoryName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(categoryName) + categoryName
                + " to: " + value, LogLevel.VERBOSE);
            categoryName = value;
        }
    }

    List<ChannelName> InterfaceCategory.ChannelNames
    {
        get
        {
            Log.WriteLine("Getting " + nameof(channelNames) + " with count of: " +
                channelNames.Count, LogLevel.VERBOSE);
            return channelNames;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(channelNames)
                + " to: " + value, LogLevel.VERBOSE);
            channelNames = value;
        }
    }

    List<InterfaceChannel> InterfaceCategory.InterfaceChannels
    {
        get
        {
            Log.WriteLine("Getting " + nameof(interfaceChannels) + " with count of: " +
                interfaceChannels.Count, LogLevel.VERBOSE);
            return interfaceChannels;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(interfaceChannels)
                + " to: " + value, LogLevel.VERBOSE);
            interfaceChannels = value;
        }
    }

    [DataMember] protected CategoryName categoryName;
    [DataMember] protected List<ChannelName> channelNames;
    [DataMember] protected List<InterfaceChannel> interfaceChannels;

    public BaseCategory()
    {
        channelNames= new List<ChannelName>();
        interfaceChannels = new List<InterfaceChannel>();
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);
}