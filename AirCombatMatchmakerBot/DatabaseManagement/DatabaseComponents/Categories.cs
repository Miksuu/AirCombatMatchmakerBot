using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class Categories
{
    public ConcurrentDictionary<ulong, InterfaceCategory> CreatedCategoriesWithChannels
    {
        get
        {
            Log.WriteLine("Getting " + nameof(createdCategoriesWithChannels) + " with count of: " +
                createdCategoriesWithChannels.Count, LogLevel.VERBOSE);
            return createdCategoriesWithChannels;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(createdCategoriesWithChannels)
                + " to: " + value, LogLevel.VERBOSE);
            createdCategoriesWithChannels = value;
        }
    }

    public ConcurrentDictionary<ulong, ulong> MatchChannelsIdWithCategoryId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(matchChannelsIdWithCategoryId) + " with count of: " +
                matchChannelsIdWithCategoryId.Count, LogLevel.VERBOSE);
            return matchChannelsIdWithCategoryId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(matchChannelsIdWithCategoryId)
                + " to: " + value, LogLevel.VERBOSE);
            matchChannelsIdWithCategoryId = value;
        }
    }

    // ConcurrentDictionary of channel categories and channelTypes inside them
    [DataMember] private ConcurrentDictionary<ulong, InterfaceCategory> createdCategoriesWithChannels { get; set; }
    [DataMember] private ConcurrentDictionary<ulong, ulong> matchChannelsIdWithCategoryId = new ConcurrentDictionary<ulong, ulong>();

    public Categories()
    {
        createdCategoriesWithChannels = new();
        matchChannelsIdWithCategoryId = new();
    }

    public KeyValuePair<ulong, InterfaceCategory> FindCreatedCategoryWithChannelKvpWithId(
        ulong _idToSearchWith)
    {
        Log.WriteLine("Getting CategoryKvp with id: " + _idToSearchWith, LogLevel.VERBOSE);
        var FoundCategoryKvp = CreatedCategoriesWithChannels.FirstOrDefault(x => x.Key == _idToSearchWith);
        Log.WriteLine("Found: " + FoundCategoryKvp.Value.CategoryType, LogLevel.VERBOSE);
        return FoundCategoryKvp;
    }



    public KeyValuePair<ulong, InterfaceCategory> FindCreatedCategoryWithChannelKvpByCategoryName(
        CategoryType? _categoryType)
    {
        Log.WriteLine("Getting CategoryKvp by category name: " + _categoryType, LogLevel.VERBOSE);
        var FoundCategoryKvp = CreatedCategoriesWithChannels.FirstOrDefault(
                x => x.Value.CategoryType == _categoryType);
        Log.WriteLine("Found: " + FoundCategoryKvp.Value.CategoryType, LogLevel.VERBOSE);
        return FoundCategoryKvp;
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