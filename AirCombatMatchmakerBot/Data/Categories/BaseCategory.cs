using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using Discord.Rest;
using System;

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

    public InterfaceCategory InterfaceCategoryRef
    {
        get
        {
            Log.WriteLine("Getting " + nameof(interfaceCategoryRef) + ": " +
                interfaceCategoryRef, LogLevel.VERBOSE);
            return interfaceCategoryRef;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(interfaceCategoryRef) + interfaceCategoryRef
                + " to: " + value, LogLevel.VERBOSE);
            interfaceCategoryRef = value;
        }
    }

    public SocketCategoryChannel SocketCategoryChannelRef
    {
        get
        {
            Log.WriteLine("Getting " + nameof(socketCategoryChannelRef) +
                ": " + socketCategoryChannelRef, LogLevel.VERBOSE);
            return socketCategoryChannelRef;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(socketCategoryChannelRef) +
                socketCategoryChannelRef + " to: " + value, LogLevel.VERBOSE);
            socketCategoryChannelRef = value;
        }
    }


    [DataMember] protected CategoryName categoryName;
    [DataMember] protected List<ChannelName> channelNames;
    [DataMember] protected List<InterfaceChannel> interfaceChannels;

    // Doesn't need to serialized, only cached on the start of the program
    protected InterfaceCategory interfaceCategoryRef;
    protected SocketCategoryChannel socketCategoryChannelRef;

    public BaseCategory()
    {
        channelNames= new List<ChannelName>();
        interfaceChannels = new List<InterfaceChannel>();
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild);

    public async Task<SocketCategoryChannel?> CreateANewSocketCategoryChannelAndReturnIt(
        SocketGuild _guild, string _categoryName)
    {
        Log.WriteLine("Starting to create a new category with name: " +
            _categoryName, LogLevel.VERBOSE);

        RestCategoryChannel newCategory = await _guild.CreateCategoryChannelAsync(
            _categoryName, x => x.PermissionOverwrites = GetGuildPermissions(_guild));
        if (newCategory == null)
        {
            Log.WriteLine(nameof(newCategory) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Created a new RestCategoryChannel with ID: " +
            newCategory.Id, LogLevel.VERBOSE);

        SocketCategoryChannel socketCategoryChannel = _guild.GetCategoryChannel(newCategory.Id);

        Log.WriteLine("socketCategoryId: " +
            socketCategoryChannel.Id.ToString(), LogLevel.VERBOSE);

        if (socketCategoryChannel == null)
        {
            Log.WriteLine(nameof(socketCategoryChannel) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Created a new socketCategoryChannel :" +
            socketCategoryChannel.Id.ToString() +" named: " +
            socketCategoryChannel.Name, LogLevel.DEBUG);

        return socketCategoryChannel;
    }

    public async Task CreateChannelsForTheCategory(
        InterfaceCategory _interfaceCategory,
        SocketCategoryChannel _socketCategoryChannel,
        SocketGuild _guild)
    {
        Log.WriteLine("Starting to create channels for: " + _socketCategoryChannel.Name +
            " (" + _socketCategoryChannel.Id + ")" + " Channel count: " + 
            _interfaceCategory.ChannelNames.Count +
            " and setting the references", LogLevel.DEBUG);

        interfaceCategoryRef = _interfaceCategory;
        socketCategoryChannelRef = _socketCategoryChannel;

        foreach (ChannelName channelName in _interfaceCategory.ChannelNames)
        {

        }
    }

    public async Task CreateSpecificChannelFromChannelName(
        SocketGuild _guild, ChannelName _channelName)
    {
        Log.WriteLine("Creating channel name: " + _channelName, LogLevel.DEBUG);

        bool channelExists = false;
        InterfaceChannel? interfaceChannel = null;

        interfaceChannel = GetChannelInstance(_channelName.ToString());
        Log.WriteLine("interfaceChanneltest: " +
            interfaceChannel.ChannelName.ToString(), LogLevel.DEBUG);

        if (interfaceChannel == null)
        {
            Log.WriteLine(nameof(interfaceChannel) + " was null!", LogLevel.CRITICAL);
            return;
        }

        // Channel found from the basecategory (it exists)
        if (interfaceCategoryRef.InterfaceChannels.Any(
            x => x.ChannelName == interfaceChannel.ChannelName))
        {
            Log.WriteLine(nameof(interfaceCategoryRef.InterfaceChannels) +
                " already contains channel: " + _channelName.ToString(), LogLevel.VERBOSE);

            // Replace interfaceChannel with a one that is from the database
            interfaceChannel = interfaceCategoryRef.InterfaceChannels.First(
                x => x.ChannelName == _channelName);

            Log.WriteLine("Replaced with: " +
                interfaceChannel.ChannelName + " from db", LogLevel.DEBUG);

            channelExists =
               await ChannelRestore.CheckIfChannelHasBeenDeletedAndRestoreForCategory(
              socketCategoryChannelRef.Id, interfaceChannel, _guild);
        }
        else
        {
            Log.WriteLine(nameof(interfaceCategoryRef.InterfaceChannels) +
                " does not contain channel: " + _channelName.ToString() +
                ", getting instance of it", LogLevel.VERBOSE);
            interfaceChannel = GetChannelInstance(_channelName.ToString());
        }

        // Insert the category's ID for easier access for the channels later on
        // (for channel specific features for example)
        interfaceChannel.ChannelsCategoryId = socketCategoryChannelRef.Id;

        if (!channelExists)
        {
            List<Overwrite> permissionsList = interfaceChannel.GetGuildPermissions(_guild);

            Log.WriteLine("Creating a channel named: " + interfaceChannel.ChannelName +
                " for category: " + interfaceCategoryRef.CategoryName + " (" +
                socketCategoryChannelRef.Id + ")", LogLevel.VERBOSE);

            ulong categoryId =
                Database.Instance.Categories.GetCreatedCategoryWithChannelKvpByCategoryName(
                    interfaceCategoryRef.CategoryName).Key;

            await interfaceChannel.CreateAChannelForTheCategory(_guild);

            interfaceChannel.InterfaceMessagesWithIds.Clear();

            interfaceCategoryRef.InterfaceChannels.Add(interfaceChannel);

            Log.WriteLine("Done adding to the db. Count is now: " +
                interfaceCategoryRef.InterfaceChannels.Count +
                " for the list of category: " + interfaceCategoryRef.CategoryName.ToString() +
                " (" + socketCategoryChannelRef.Id + ")", LogLevel.VERBOSE);
        }

        await interfaceChannel.PrepareChannelMessages();

        Log.WriteLine("Done looping through.", LogLevel.VERBOSE);
    }

    private static InterfaceChannel GetChannelInstance(string _channelName)
    {
        return (InterfaceChannel)EnumExtensions.GetInstance(_channelName);
    }
}