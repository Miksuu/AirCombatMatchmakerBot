using System.Runtime.Serialization;

[DataContract]
public abstract class BaseChallengeChannelButton : BaseButton, InterfaceButton
{
    protected InterfaceLeague? interfaceLeagueCached { get; set; }

    protected void FindInterfaceLeagueAndCacheIt(ulong _componentChannelId)
    {
        Log.WriteLine("Starting to find dbLeagueInstace with: " + _componentChannelId, LogLevel.VERBOSE);

        //InterfaceLeague? dbLeagueInstance = null;

        // Find the category of the given button, temp, could optimise it here
        CategoryType ? categoryName = null;
        foreach (var interfaceCategoryKvp in
            Database.Instance.Categories.CreatedCategoriesWithChannels)
        {
            Log.WriteLine("Loop on: " + interfaceCategoryKvp.Key + " | " +
                interfaceCategoryKvp.Value.CategoryType, LogLevel.VERBOSE);
            if (interfaceCategoryKvp.Value.InterfaceChannels.Any(
                x => x.Value.ChannelId == _componentChannelId))
            {
                Log.WriteLine("Found category: " +
                    interfaceCategoryKvp.Value.CategoryType, LogLevel.DEBUG);
                categoryName = interfaceCategoryKvp.Value.CategoryType;
                break;
            }
        }
        if (categoryName == null)
        {
            string errorMsg = nameof(categoryName) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("after foreach with: " + categoryName.Value, LogLevel.VERBOSE);

        string? categoryNameString = categoryName.ToString();
        if (categoryNameString == null)
        {
            string errorMsg = nameof(categoryName) + "String was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL); ;
            return;
        }

        Log.WriteLine("categoryNameString: " + categoryNameString, LogLevel.VERBOSE);

        interfaceLeagueCached =
            Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(buttonCategoryId);

        if (interfaceLeagueCached == null)
        {
            string errorMsg = "Error adding to the queue! Could not find the league" +
                nameof(interfaceLeagueCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return;
        }
        Log.WriteLine("Found: " + nameof(interfaceLeagueCached) + interfaceLeagueCached.LeagueCategoryName +
            " with channelID: " + _componentChannelId + ", returning it", LogLevel.VERBOSE);
    }
}