﻿using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using Discord.Rest;
using System;

[DataContract]
public abstract class BaseCategory : InterfaceCategory
{
    CategoryType InterfaceCategory.CategoryType
    {
        get
        {
            Log.WriteLine("Getting " + nameof(categoryTypes) + ": " + categoryTypes, LogLevel.VERBOSE);
            return categoryTypes;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(categoryTypes) + categoryTypes
                + " to: " + value, LogLevel.VERBOSE);
            categoryTypes = value;
        }
    }

    List<ChannelType> InterfaceCategory.ChannelTypes
    {
        get
        {
            Log.WriteLine("Getting " + nameof(channelTypes) + " with count of: " +
                channelTypes.Count, LogLevel.VERBOSE);
            return channelTypes;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(channelTypes)
                + " to: " + value, LogLevel.VERBOSE);
            channelTypes = value;
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

    ulong InterfaceCategory.SocketCategoryChannelId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(socketCategoryChannelId) +
                ": " + socketCategoryChannelId, LogLevel.VERBOSE);
            return socketCategoryChannelId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(socketCategoryChannelId) + socketCategoryChannelId
                + " to: " + value, LogLevel.VERBOSE);
            socketCategoryChannelId = value;
        }
    }

    [DataMember] protected CategoryType categoryTypes;
    [DataMember] protected List<ChannelType> channelTypes;
    [DataMember] protected List<InterfaceChannel> interfaceChannels;
    [DataMember] protected ulong socketCategoryChannelId;

    public BaseCategory()
    {
        channelTypes = new List<ChannelType>();
        interfaceChannels = new List<InterfaceChannel>();
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);

    public async Task<SocketCategoryChannel?> CreateANewSocketCategoryChannelAndReturnIt(
        SocketGuild _guild, string _categoryName, SocketRole _role)
    {
        Log.WriteLine("Starting to create a new category with name: " +
            _categoryName, LogLevel.VERBOSE);

        RestCategoryChannel newCategory = await _guild.CreateCategoryChannelAsync(
            _categoryName, x => x.PermissionOverwrites = GetGuildPermissions(_guild, _role));
        if (newCategory == null)
        {
            Log.WriteLine(nameof(newCategory) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Created a new RestCategoryChannel with ID: " +
            newCategory.Id, LogLevel.VERBOSE);

        SocketCategoryChannel socketCategoryChannel =
            _guild.GetCategoryChannel(newCategory.Id);

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
        InterfaceCategory _interfaceCategory, ulong _socketCategoryChannelId,
        SocketGuild _guild)
    {
        Log.WriteLine("Starting to create channels for: " + _socketCategoryChannelId + ")" + 
            " Channel count: " + _interfaceCategory.ChannelTypes.Count +
            " and setting the references", LogLevel.DEBUG);

        socketCategoryChannelId = _socketCategoryChannelId;

        foreach (ChannelType channelType in _interfaceCategory.ChannelTypes)
        {
            await CreateSpecificChannelFromChannelType(_guild, channelType, _socketCategoryChannelId);
        }
    }

    public async Task CreateSpecificChannelFromChannelType(
        SocketGuild _guild, ChannelType _channelType, ulong _socketCategoryChannelId,
        string _overrideChannelName = "") // Keeps the functionality, but overrides the channel name
        // It is used for creating matches with correct ID right now.
    {
        Log.WriteLine("Creating channel name: " + _channelType, LogLevel.DEBUG);

        bool channelExists = false;
        InterfaceChannel? interfaceChannel = null;

        interfaceChannel = GetChannelInstance(_channelType.ToString());
        Log.WriteLine("interfaceChanneltest: " +
            interfaceChannel.ChannelType.ToString(), LogLevel.DEBUG);

        if (interfaceChannel == null)
        {
            Log.WriteLine(nameof(interfaceChannel) + " was null!", LogLevel.CRITICAL);
            return;
        }

        // Channel found from the basecategory (it exists)
        if (interfaceChannels.Any(
            x => x.ChannelType == interfaceChannel.ChannelType))
        {
            Log.WriteLine(nameof(interfaceChannels) +
                " already contains channel: " + _channelType.ToString(), LogLevel.VERBOSE);

            // Replace interfaceChannel with a one that is from the database
            interfaceChannel = interfaceChannels.First(
                x => x.ChannelType == _channelType);

            Log.WriteLine("Replaced with: " +
                interfaceChannel.ChannelType + " from db", LogLevel.DEBUG);

            channelExists =
               await ChannelRestore.CheckIfChannelHasBeenDeletedAndRestoreForCategory(
              _socketCategoryChannelId, interfaceChannel, _guild);
        }
        else
        {
            Log.WriteLine(nameof(interfaceChannels) +
                " does not contain channel: " + _channelType.ToString() +
                ", getting instance of it", LogLevel.VERBOSE);
            interfaceChannel = GetChannelInstance(_channelType.ToString());
        }

        // Insert the category's ID for easier access for the channels later on
        // (for channel specific features for example)
        interfaceChannel.ChannelsCategoryId = _socketCategoryChannelId;

        if (!channelExists)
        {
            List<Overwrite> permissionsList = interfaceChannel.GetGuildPermissions(_guild);

            Log.WriteLine("Creating a channel named: " + interfaceChannel.ChannelType +
                " for category: " + categoryTypes + " (" +
                _socketCategoryChannelId + ")", LogLevel.VERBOSE);

            ulong categoryId =
                Database.Instance.Categories.GetCreatedCategoryWithChannelKvpByCategoryName(
                    categoryTypes).Key;

            await interfaceChannel.CreateAChannelForTheCategory(_guild);

            interfaceChannel.InterfaceMessagesWithIds.Clear();

            interfaceChannels.Add(interfaceChannel);

            Log.WriteLine("Done adding to the db. Count is now: " +
                interfaceChannels.Count +
                " for the list of category: " + categoryTypes.ToString() +
                " (" + _socketCategoryChannelId + ")", LogLevel.VERBOSE);
        }

        await interfaceChannel.PrepareChannelMessages();

        Log.WriteLine("Done looping through.", LogLevel.VERBOSE);
    }

    private static InterfaceChannel GetChannelInstance(string _channelType)
    {
        return (InterfaceChannel)EnumExtensions.GetInstance(_channelType);
    }
}