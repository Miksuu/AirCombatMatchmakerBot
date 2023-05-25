using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

[DataContract]
public class Categories : logClass<Categories>, InterfaceLoggableClass
{
    [IgnoreDataMember]
    public ConcurrentDictionary<ulong, InterfaceCategory> CreatedCategoriesWithChannels
    {
        get => createdCategoriesWithChannels.GetValue();
        set => createdCategoriesWithChannels.SetValue(value);
    }

    [IgnoreDataMember]
    public ConcurrentDictionary<ulong, ulong> MatchChannelsIdWithCategoryId
    {
        get => matchChannelsIdWithCategoryId.GetValue();
        set => matchChannelsIdWithCategoryId.SetValue(value);
    }

    // ConcurrentDictionary of channel categories and channelTypes inside them
    [DataMember] private logConcurrentDictionary<ulong, InterfaceCategory> createdCategoriesWithChannels =
        new logConcurrentDictionary<ulong, InterfaceCategory>();
    [DataMember] private logConcurrentDictionary<ulong, ulong> matchChannelsIdWithCategoryId =
        new logConcurrentDictionary<ulong, ulong>();

    public List<string> GetClassParameters()
    {
        return new List<string> { createdCategoriesWithChannels.GetLoggingClassParameters(),
            matchChannelsIdWithCategoryId.GetLoggingClassParameters() };
    }

    public InterfaceCategory? FindInterfaceCategoryWithId(
        ulong _idToSearchWith)
    {
        Log.WriteLine("Getting CategoryKvp with id: " + _idToSearchWith, LogLevel.VERBOSE);
        var interfaceCategory = CreatedCategoriesWithChannels.FirstOrDefault(x => x.Key == _idToSearchWith).Value;
        Log.WriteLine("Found: " + interfaceCategory.CategoryType, LogLevel.VERBOSE);
        return interfaceCategory;
    }

    public InterfaceCategory? FindInterfaceCategoryByCategoryName(
        CategoryType? _categoryType)
    {
        Log.WriteLine("Getting CategoryKvp by category name: " + _categoryType, LogLevel.VERBOSE);
        var interfaceCategory = CreatedCategoriesWithChannels.FirstOrDefault(
                x => x.Value.CategoryType == _categoryType).Value;
        if (interfaceCategory == null)
        {
            Log.WriteLine(nameof(interfaceCategory) + " was null!", LogLevel.CRITICAL);
            return interfaceCategory;
        }

        Log.WriteLine("Found: " + interfaceCategory.CategoryType, LogLevel.VERBOSE);
        return interfaceCategory;
    }

    public InterfaceChannel? FindInterfaceChannelInsideACategoryWithIds(
        ulong _categoryIdToFind, ulong _channelIdToFind)
    {
        return FindInterfaceCategoryWithId(_categoryIdToFind).
            FindInterfaceChannelWithIdInTheCategory(_channelIdToFind);
    }

    public InterfaceChannel? FindInterfaceChannelInsideACategoryWithIdAndName(
        ulong _categoryIdToFind, ChannelType _channelTypeToFind)
    {
        return FindInterfaceCategoryWithId(_categoryIdToFind).
            FindInterfaceChannelWithNameInTheCategory(_channelTypeToFind);
    }

    public InterfaceChannel? FindInterfaceChannelInsideACategoryWithNames(
        CategoryType _categoryTypeToFind, ChannelType _channelTypeToFind)
    {
        InterfaceCategory? interfaceCategory =
            FindInterfaceCategoryByCategoryName(_categoryTypeToFind);
        if (interfaceCategory == null)
        {
            Log.WriteLine(nameof(interfaceCategory) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        InterfaceChannel? interfaceChannel = interfaceCategory.FindInterfaceChannelWithNameInTheCategory(_channelTypeToFind);
        if (interfaceCategory == null)
        {
            Log.WriteLine(nameof(interfaceCategory) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        return interfaceChannel;
    }

    public InterfaceMessage? FindAMessageInAnInterfaceChannelInsideACategoryWithIdAndNames(
        ulong _categoryIdToFind, ChannelType _channelTypeToFind, MessageName _messageNameToFind)
    {
        var foundChannel = FindInterfaceChannelInsideACategoryWithIdAndName(_categoryIdToFind, _channelTypeToFind);
        if (foundChannel == null)
        {
            Log.WriteLine(nameof(foundChannel) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        var foundMessage = foundChannel.FindInterfaceMessageWithNameInTheChannel(_messageNameToFind);
        if (foundMessage == null)
        {
            Log.WriteLine(nameof(foundMessage) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        return foundMessage;
    }

    public InterfaceMessage? FindInterfaceMessageWithComponentChannelIdAndMessageId(
        ulong _componentChannelId, ulong _componentMessageId)
    {
        Log.WriteLine("Getting CategoryKvp with channel id: " + _componentChannelId, LogLevel.VERBOSE);
        foreach (var createdCategory in createdCategoriesWithChannels)
        {
            Log.WriteLine("Looping on: " + createdCategory.Value.CategoryType, LogLevel.VERBOSE);

            var interfaceChannelTemp = createdCategory.Value.InterfaceChannels.FirstOrDefault(
                    x => x.Value.ChannelId == _componentChannelId);
            if (interfaceChannelTemp.Key == 0 || interfaceChannelTemp.Value == null)
            {
                Log.WriteLine("Was null, continuing...", LogLevel.VERBOSE);
                continue;
            }

            Log.WriteLine("Found " + nameof(interfaceChannelTemp) + ":" + interfaceChannelTemp.Value.ChannelName + " with id: " +
                interfaceChannelTemp.Key, LogLevel.VERBOSE);

            var interfaceMessageKvp =
                interfaceChannelTemp.Value.InterfaceMessagesWithIds.FirstOrDefault(
                    x => x.Value.MessageId == _componentMessageId);
            if (interfaceMessageKvp.Key == 0 || interfaceMessageKvp.Value == null)
            {
                Log.WriteLine(nameof(interfaceMessageKvp) + " was null!", LogLevel.CRITICAL);
                continue;
            }

            Log.WriteLine("Found " + nameof(interfaceMessageKvp) + ":" + interfaceMessageKvp.Value.MessageName + " with id: " +
                interfaceMessageKvp.Key, LogLevel.VERBOSE);

            return interfaceMessageKvp.Value;
        }

        return null;
    }

    public void AddToCreatedCategoryWithChannelWithUlongAndInterfaceCategory(
        ulong _id, InterfaceCategory _InterfaceCategory)
    {
        Log.WriteLine("Adding interfaceCategory: " + _InterfaceCategory.CategoryType +
            "to the CreatedCategoriesWithChannels ConcurrentDictionary" + " with id: " + _id, LogLevel.VERBOSE);
        CreatedCategoriesWithChannels.TryAdd(_id, _InterfaceCategory);
        Log.WriteLine("Done adding, count is now: " +
            CreatedCategoriesWithChannels.Count, LogLevel.VERBOSE);
    }

    public void RemoveFromCreatedCategoryWithChannelWithKey(ulong _id)
    {
        Log.WriteLine("Removing with id: " + _id, LogLevel.VERBOSE);
        CreatedCategoriesWithChannels.TryRemove(_id, out InterfaceCategory? _ic);
        Log.WriteLine("Done removing, count is now: " +
            CreatedCategoriesWithChannels.Count, LogLevel.VERBOSE);
    }
}