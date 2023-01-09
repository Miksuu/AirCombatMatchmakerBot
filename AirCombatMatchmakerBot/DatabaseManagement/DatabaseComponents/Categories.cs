using Discord.WebSocket;
using System.Runtime.Serialization;

[DataContract]
public class Categories
{
    public Dictionary<ulong, InterfaceCategory> CreatedCategoriesWithChannels
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

    // Dictionary of channel categories and channelTypes inside them
    [DataMember] private Dictionary<ulong, InterfaceCategory> createdCategoriesWithChannels { get; set; }

    public Categories()
    {
        createdCategoriesWithChannels = new();
    }

    // Searches it with string, not ID. Maybe change this
    public KeyValuePair<ulong, InterfaceCategory> GetCreatedCategoryWithChannelKvpWithString(
        string _idToSearchWith)
    {
        Log.WriteLine("Getting CategoryKvp with id: " + _idToSearchWith, LogLevel.VERBOSE);
        var FoundCategoryKvp = CreatedCategoriesWithChannels.First(
            x => x.Key.ToString() == _idToSearchWith.ToString());
        Log.WriteLine("Found: " + FoundCategoryKvp.Value.CategoryType, LogLevel.VERBOSE);
        return FoundCategoryKvp;
    }

    public KeyValuePair<ulong, InterfaceCategory> GetCreatedCategoryWithChannelKvpWithId(
        ulong _idToSearchWith)
    {
        Log.WriteLine("Getting CategoryKvp with id: " + _idToSearchWith, LogLevel.VERBOSE);
        var FoundCategoryKvp = CreatedCategoriesWithChannels.First(x => x.Key == _idToSearchWith);
        Log.WriteLine("Found: " + FoundCategoryKvp.Value.CategoryType, LogLevel.VERBOSE);
        return FoundCategoryKvp;
    }

    public KeyValuePair<ulong, InterfaceCategory> GetCreatedCategoryWithChannelKvpByCategoryName(
        CategoryType? _categoryType)
    {
        Log.WriteLine("Getting CategoryKvp by category name: " + _categoryType, LogLevel.VERBOSE);
        var FoundCategoryKvp = CreatedCategoriesWithChannels.First(
                x => x.Value.CategoryType == _categoryType);
        Log.WriteLine("Found: " + FoundCategoryKvp.Value.CategoryType, LogLevel.VERBOSE);
        return FoundCategoryKvp;
    }

    public void AddToCreatedCategoryWithChannelWithUlongAndInterfaceCategory(
        ulong _id, InterfaceCategory _InterfaceCategory)
    {
        Log.WriteLine("Adding interfaceCategory: " + _InterfaceCategory.CategoryType +
            "to the CreatedCategoriesWithChannels Dictionary" + " with id: " + _id, LogLevel.VERBOSE);
        CreatedCategoriesWithChannels.Add(_id, _InterfaceCategory);
        Log.WriteLine("Done adding, count is now: " +
            CreatedCategoriesWithChannels.Count, LogLevel.VERBOSE);
    }

    public void RemoveFromCreatedCategoryWithChannelWithKey(ulong _id)
    {
        Log.WriteLine("Removing with id: " + _id, LogLevel.VERBOSE);
        CreatedCategoriesWithChannels.Remove(_id);
        Log.WriteLine("Done removing, count is now: " +
            CreatedCategoriesWithChannels.Count, LogLevel.VERBOSE);
    }
}