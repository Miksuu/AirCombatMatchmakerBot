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

    public async Task CreateChannelsForTheCategory(
        InterfaceCategory _interfaceCategory,
        SocketCategoryChannel _socketCategoryChannel,
        SocketGuild _guild)
    {
        Log.WriteLine("Starting to create channels for: " + _socketCategoryChannel.Name +
            " (" + _socketCategoryChannel.Id + ")" + " Channel count: " +
            _interfaceCategory.ChannelNames.Count, LogLevel.DEBUG);

        foreach (ChannelName channelName in _interfaceCategory.ChannelNames)
        {
            Log.WriteLine("Looping with channel name: " + channelName, LogLevel.DEBUG);

            bool channelExists = false;
            InterfaceChannel? interfaceChannel = null;

            interfaceChannel = GetChannelInstance(channelName.ToString());
            Log.WriteLine("interfaceChanneltest: " +
                interfaceChannel.ChannelName.ToString(), LogLevel.DEBUG);

            if (interfaceChannel == null)
            {
                Log.WriteLine(nameof(interfaceChannel) + " was null!", LogLevel.CRITICAL);
                return;
            }


               
            // Channel found from the basecategory (it exists)
            if (_interfaceCategory.InterfaceChannels.Any(x => x.ChannelName == interfaceChannel.ChannelName))
            {
                Log.WriteLine(nameof(_interfaceCategory.InterfaceChannels) + " already contains channel: " +
                    channelName.ToString(), LogLevel.VERBOSE);

                // Replace interfaceChannel with a one that is from the database
                interfaceChannel = _interfaceCategory.InterfaceChannels.First(
                    x => x.ChannelName == channelName);

                Log.WriteLine("Replaced with: " +
                    interfaceChannel.ChannelName + " from db", LogLevel.DEBUG);

                channelExists =
                   await ChannelRestore.CheckIfChannelHasBeenDeletedAndRestoreForCategory(
                  _socketCategoryChannel.Id, interfaceChannel, _guild);
            }
            else
            {
                Log.WriteLine(nameof(_interfaceCategory.InterfaceChannels) +
                    " does not contain channel: " + channelName.ToString() +
                    ", getting instance of it", LogLevel.VERBOSE);
                interfaceChannel = GetChannelInstance(channelName.ToString());
            }

            // Insert the category's ID for easier access for the channels later on
            // (for channel specific features for example)
            interfaceChannel.ChannelsCategoryId = _socketCategoryChannel.Id;

            //string? channelNameString = EnumExtensions.GetEnumMemberAttrValue(channelName);
            /*
            if (channelNameString == null)
            {
                Log.WriteLine(nameof(channelNameString).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }*/

            if (!channelExists)
            {
                List<Overwrite> permissionsList = interfaceChannel.GetGuildPermissions(_guild);

                Log.WriteLine("Creating a channel named: " + interfaceChannel.ChannelName + " for category: "
                             + _interfaceCategory.CategoryName + " (" +
                             _socketCategoryChannel.Id + ")", LogLevel.VERBOSE);

                ulong categoryId =
                    Database.Instance.Categories.GetCreatedCategoryWithChannelKvpByCategoryName(
                        _interfaceCategory.CategoryName).Key;

                await interfaceChannel.CreateAChannelForTheCategory(_guild);

                _interfaceCategory.InterfaceChannels.Add(interfaceChannel);

                Log.WriteLine("Done adding to the db. Count is now: " +
                    _interfaceCategory.InterfaceChannels.Count +
                    " for the list of category: " + _interfaceCategory.CategoryName.ToString() +
                    " (" + _socketCategoryChannel.Id + ")", LogLevel.VERBOSE);
            }

            await interfaceChannel.PrepareChannelMessages();

            Log.WriteLine("Done looping through.", LogLevel.VERBOSE);
        }
    }

    private static InterfaceChannel GetChannelInstance(string _channelName)
    {
        return (InterfaceChannel)EnumExtensions.GetInstance(_channelName);
    }
}