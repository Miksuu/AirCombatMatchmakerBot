using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;
using Discord.WebSocket;

[DataContract]
public class Categories : logClass<Categories>
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

    public InterfaceCategory FindInterfaceCategoryWithCategoryId(
        ulong _categoryIdToSearchWith)
    {
        Log.WriteLine("Getting CategoryKvp with id: " + _categoryIdToSearchWith);
        InterfaceCategory interfaceCategory = CreatedCategoriesWithChannels.FirstOrDefault(x => x.Key == _categoryIdToSearchWith).Value;
        if (interfaceCategory == null)
        {
            string errorMsg = nameof(interfaceCategory) + " was null! with id: " + _categoryIdToSearchWith;
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            throw new InvalidOperationException(errorMsg);
        }
        Log.WriteLine("Found: " + interfaceCategory.CategoryType);
        return interfaceCategory;
    }
    public bool FindIfInterfaceCategoryExistsWithCategoryId(
        ulong _categoryIdToSearchWith)
    {
        Log.WriteLine("Getting if CategoryKvp exists with id: " + _categoryIdToSearchWith);
        bool exists = CreatedCategoriesWithChannels.Any(x => x.Key == _categoryIdToSearchWith);
        if (!exists)
        {
            Log.WriteLine(nameof(InterfaceCategory) + " did not exist! with id: " +
                _categoryIdToSearchWith, LogLevel.CRITICAL);

            return false;
        }
        Log.WriteLine("Found: " + _categoryIdToSearchWith);
        return true;
    }

    public InterfaceCategory FindInterfaceCategoryWithChannelId(
        ulong _channelId)
    {
        Log.WriteLine("Getting CategoryKvp with channel id: " + _channelId);
        InterfaceCategory interfaceCategory = CreatedCategoriesWithChannels.FirstOrDefault(x => x.Value.InterfaceChannels.ContainsKey(_channelId)).Value;
        if (interfaceCategory == null)
        {
            string errorMsg = nameof(interfaceCategory) + " was null! with channel id: " + _channelId;
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            throw new InvalidOperationException(errorMsg);
        }
        Log.WriteLine("Found: " + interfaceCategory.CategoryType);
        return interfaceCategory;
    }

    public InterfaceCategory FindInterfaceCategoryByCategoryName(
        CategoryType _categoryType)
    {
        Log.WriteLine("Getting CategoryKvp by category name: " + _categoryType);
        var interfaceCategory = CreatedCategoriesWithChannels.FirstOrDefault(
                x => x.Value.CategoryType == _categoryType).Value;
        if (interfaceCategory == null)
        {
            Log.WriteLine(nameof(interfaceCategory) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException("InterfaceCategory not found for the given id.");
        }

        Log.WriteLine("Found: " + interfaceCategory.CategoryType);
        return interfaceCategory;
    }

    public InterfaceChannel FindInterfaceChannelInsideACategoryWithNames(
        CategoryType _categoryTypeToFind, ChannelType _channelTypeToFind)
    {
        try
        {
            InterfaceCategory interfaceCategory =
                FindInterfaceCategoryByCategoryName(_categoryTypeToFind);

            InterfaceChannel interfaceChannel = interfaceCategory.FindInterfaceChannelWithNameInTheCategory(_channelTypeToFind);

            return interfaceChannel;
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            throw new InvalidOperationException(ex.Message);
        }
    }

    public void AddToCreatedCategoryWithChannelWithUlongAndInterfaceCategory(
        ulong _id, InterfaceCategory _InterfaceCategory)
    {
        Log.WriteLine("Adding interfaceCategory: " + _InterfaceCategory.CategoryType +
            "to the CreatedCategoriesWithChannels ConcurrentDictionary" + " with id: " + _id);
        CreatedCategoriesWithChannels.TryAdd(_id, _InterfaceCategory);
        Log.WriteLine("Done adding, count is now: " +
            CreatedCategoriesWithChannels.Count);
    }

    public void RemoveFromCreatedCategoryWithChannelWithKey(ulong _id)
    {
        Log.WriteLine("Removing with id: " + _id);
        CreatedCategoriesWithChannels.TryRemove(_id, out InterfaceCategory? _ic);
        Log.WriteLine("Done removing, count is now: " +
            CreatedCategoriesWithChannels.Count);
    }

    public async Task<string> GetMessageJumpUrl(
    ulong _leagueCategoryId, ulong _channelId, MessageName _messageName)
    {
        Log.WriteLine("Getting jump URL with: " + _leagueCategoryId +
            " | " + _channelId + " | " + _messageName);

        var messageToFind = Database.Instance.Categories.FindInterfaceCategoryWithCategoryId(
            _leagueCategoryId).FindInterfaceChannelWithIdInTheCategory(
                _channelId).FindInterfaceMessageWithNameInTheChannel(
                    _messageName);
        var client = BotReference.GetClientRef();
        var channel = client.GetChannel(_channelId) as IMessageChannel;
        var message = await channel.GetMessageAsync(messageToFind.MessageId);
        string jumpUrl = message.GetJumpUrl();
        Log.WriteLine("Found: " + jumpUrl, LogLevel.DEBUG);
        return jumpUrl;
    }
}