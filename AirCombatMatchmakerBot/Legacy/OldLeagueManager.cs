//leagueRegistrationMessageExists = CheckIfLeagueRegisterationMessageExists(leagueInterface);

/*
leagueChannelCategoryExists = CategoryManager.CheckIfLeagueCategoryExists(
    leagueInterface.DiscordLeagueReferences.leagueCategoryId).Result;

newTypesOfLeagueChannels = Enum.GetValues(typeof(LeagueChannelName)).Length >
    leagueInterface.DiscordLeagueReferences.leagueChannels.Count ? true : false; 

Log.WriteLine(nameof(newTypesOfLeagueChannels) + ": " + newTypesOfLeagueChannels, LogLevel.VERBOSE);

if (leagueRegistrationMessageExists && leagueChannelCategoryExists && !newTypesOfLeagueChannels)
{
    Log.WriteLine(nameof(leagueRegistrationMessageExists) + " and " +
        nameof(leagueChannelCategoryExists) + " true, returning.", LogLevel.DEBUG);
    return;
}*/

/*
if (_leagueInterface.DiscordLeagueReferences.leagueRegistrationChannelMessageId == 0)
{
    Log.WriteLine(nameof(_leagueInterface.DiscordLeagueReferences.leagueRegistrationChannelMessageId) +
    " was 0, channel not created succesfully?", LogLevel.CRITICAL);
    return;
}*/

/*
Log.WriteLine(nameof(leagueRegistrationMessageExists) + ": " + leagueRegistrationMessageExists +
    " | " + nameof(leagueChannelCategoryExists) + ": " + leagueChannelCategoryExists, LogLevel.DEBUG);*/

//leagueInterface = CreateALeagueJoinButton(_channel, leagueInterface).Result;

/*
if (!leagueChannelCategoryExists || newTypesOfLeagueChannels)
{
    /*
    // Category was deleted or does not exist, clear channelNames in the db and generate a new category
    if (!newTypesOfLeagueChannels)
    {
        Log.WriteLine("name: " + _leagueName.ToString() +
            " was not found, creating a category for it", LogLevel.DEBUG);

        leagueInterface.DiscordLeagueReferences.leagueChannels.Clear();
        //LeagueChannelManager.CreateCategoryAndChannelsForALeague(leagueInterface);
    }
    // New channelNames detected in the code, create them
    else
    {
        Log.WriteLine("New types of league channels detected in the code," +
            " fixing that for league: " + _leagueName.ToString(), LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        //LeagueChannelManager.CreateChannelsForTheLeagueCategory(leagueInterface, guild);
    } 
}*/

/*
    // To avoid duplicates in the db
    if (!leagueExistsInTheDatabase)
    {
        Log.WriteLine("The league doesn't exist in the database, storing it", LogLevel.DEBUG);
        if (leagueInterface == null)
        {
            Log.WriteLine("leagueInterface for channel: " + _channel.Id + " and _leagueName: " +
                _leagueName.ToString() + " was null!", LogLevel.CRITICAL);
            return;
        }


    } */

//StoreTheLeague(leagueInterface);

/*
public static void CreateALeague(ITextChannel _channel, LeagueCategoryName _leagueName)
{
bool leagueExistsInTheDatabase = false;
bool leagueRegistrationMessageExists = false;
bool leagueChannelCategoryExists = false;
bool newTypesOfLeagueChannels = false;

Log.WriteLine("Looping on leagueName: " + _leagueName.ToString(), LogLevel.VERBOSE);

InterfaceLeagueCategory? leagueInterface = GetLeagueInstanceWithLeagueCategoryName(_leagueName.ToString());

Log.WriteLine("Made a " + nameof(leagueInterface) + " named: " +
 leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

if (Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.Values == null)
{
Log.WriteLine(nameof(Database.Instance.StoredLeagueCategoriesWithChannelsCategoriesWithChannels.Values) + " was null!", LogLevel.CRITICAL);
return;
}

if (CheckIfALeagueCategoryNameExistsInDatabase(_leagueName))
{
Log.WriteLine("name: " + _leagueName.ToString() +
    " was already in the database list", LogLevel.VERBOSE);

leagueExistsInTheDatabase = true;

leagueInterface = GetInterfaceLeagueCategoryFromTheDatabase(leagueInterface);

if (leagueInterface == null)
{
    Log.WriteLine("leagueInterface with " + _leagueName.ToString() +
        " was null!", LogLevel.CRITICAL);
    return;
}

leagueRegistrationMessageExists = CheckIfLeagueRegisterationMessageExists(leagueInterface);

leagueChannelCategoryExists = CategoryManager.CheckIfLeagueCategoryExists(
    leagueInterface.DiscordLeagueReferences.leagueCategoryId).Result;

newTypesOfLeagueChannels = Enum.GetValues(typeof(LeagueChannelName)).Length >
    leagueInterface.DiscordLeagueReferences.leagueChannels.Count ? true : false;

Log.WriteLine(nameof(newTypesOfLeagueChannels) + ": " + newTypesOfLeagueChannels, LogLevel.VERBOSE);

if (leagueRegistrationMessageExists && leagueChannelCategoryExists && !newTypesOfLeagueChannels)
{
    Log.WriteLine(nameof(leagueRegistrationMessageExists) + " and " +
        nameof(leagueChannelCategoryExists) + " true, returning.", LogLevel.DEBUG);
    return;
}
}

Log.WriteLine(nameof(leagueRegistrationMessageExists) + ": " + leagueRegistrationMessageExists + 
" | " + nameof(leagueChannelCategoryExists) + ": " + leagueChannelCategoryExists, LogLevel.DEBUG);

if (!leagueRegistrationMessageExists)
{
Log.WriteLine("name: " + _leagueName.ToString() +
    " was not found, creating a button for it", LogLevel.DEBUG);
leagueInterface = CreateALeagueJoinButton(_channel, leagueInterface).Result;
}

if (!leagueChannelCategoryExists || newTypesOfLeagueChannels)
{
// Category was deleted or does not exist, clear channelNames in the db and generate a new category
if (!newTypesOfLeagueChannels)
{
    Log.WriteLine("name: " + _leagueName.ToString() +
        " was not found, creating a category for it", LogLevel.DEBUG);

    leagueInterface.DiscordLeagueReferences.leagueChannels.Clear();
    //LeagueChannelManager.CreateCategoryAndChannelsForALeague(leagueInterface);
}
// New channelNames detected in the code, create them
else
{
    Log.WriteLine("New types of league channels detected in the code," +
        " fixing that for league: " + _leagueName.ToString(), LogLevel.DEBUG);

    var guild = BotReference.GetGuildRef();
    if (guild == null)
    {
        Exceptions.BotGuildRefNull();
        return;
    }

    //LeagueChannelManager.CreateChannelsForTheLeagueCategory(leagueInterface, guild);
}
}

// To avoid duplicates in the db
if (!leagueExistsInTheDatabase)
{
Log.WriteLine("The league doesn't exist in the database, storing it", LogLevel.DEBUG);
if (leagueInterface == null)
{
    Log.WriteLine("leagueInterface for channel: " + _channel.Id + " and _leagueName: " +
        _leagueName.ToString() + " was null!", LogLevel.CRITICAL);
    return;
}

StoreTheLeague(leagueInterface);
}
} */