using Discord.WebSocket;

[Serializable]
public class Categories
{
    // Dictionary of channel categories and channelNames inside them
    private Dictionary<ulong, InterfaceCategory> CreatedCategoriesWithChannels { get; set; }

    public Categories()
    {
        CreatedCategoriesWithChannels = new();
    }

    public Dictionary<ulong, InterfaceCategory> GetDictionaryOfCreatedCategoriesWithChannels()
    {
        Log.WriteLine("Getting list of cachedUserIDs with count of: " +
            CreatedCategoriesWithChannels.Count, LogLevel.VERBOSE);
        return CreatedCategoriesWithChannels;
    }

    public KeyValuePair<ulong, InterfaceCategory> GetCreatedCategoryWithChannelKvpWithId(
        ulong _idToSearchWith)
    {
        Log.WriteLine("Getting CategoryKvp with id: " + _idToSearchWith, LogLevel.VERBOSE);
        var FoundCategoryKvp = CreatedCategoriesWithChannels.First(x => x.Key == _idToSearchWith);
        Log.WriteLine("Found: " + FoundCategoryKvp.Value.CategoryName, LogLevel.VERBOSE);
        return FoundCategoryKvp;
    }

    public KeyValuePair<ulong, InterfaceCategory> GetCreatedCategoryWithChannelKvpByCategoryName(
        CategoryName? _categoryName)
    {
        Log.WriteLine("Getting CategoryKvp by category name: " + _categoryName, LogLevel.VERBOSE);
        var FoundCategoryKvp = CreatedCategoriesWithChannels.First(
                x => x.Value.CategoryName == _categoryName);
        Log.WriteLine("Found: " + FoundCategoryKvp.Value.CategoryName, LogLevel.VERBOSE);
        return FoundCategoryKvp;
    }

    public void AddToCreatedCategoryWithChannelWithUlongAndBaseCategory(ulong _id, BaseCategory _baseCategory)
    {
        Log.WriteLine("Adding baseCategory: " + _baseCategory.categoryName +
            "to the CreatedCategoriesWithChannels Dictionary" + " with id: " + _id, LogLevel.VERBOSE);
        CreatedCategoriesWithChannels.Add(_id, _baseCategory);
        Log.WriteLine("Done adding, count is now: " + CreatedCategoriesWithChannels.Count, LogLevel.VERBOSE);
    }

    public void RemoveFromCreatedCategoryWithChannelWithKey(ulong _id)
    {
        Log.WriteLine("Removing with id: " + _id, LogLevel.VERBOSE);
        CreatedCategoriesWithChannels.Remove(_id);
        Log.WriteLine("Done removing, count is now: " + CreatedCategoriesWithChannels.Count, LogLevel.VERBOSE);
    }
}