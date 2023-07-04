using Discord.WebSocket;

public static class CategoryAndChannelManager
{
    private static readonly List<CategoryType> categoriesThatWontGetGenerated = new List<CategoryType>
    {
        CategoryType.LEAGUETEMPLATE
    };

    public static async Task CreateCategoriesAndChannelsForTheDiscordServer()
    {
        Log.WriteLine("Starting to create categories and channels for the discord server");

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return;
        }

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        await GenerateLeagueCategories(client, guild);
        await GenerateRegularCategories(client, guild);

        Log.WriteLine("Done looping through the category names.");
    }

    private static async Task GenerateLeagueCategories(DiscordSocketClient _client, SocketGuild _guild)
    {
        foreach (LeagueName categoryName in Enum.GetValues(typeof(LeagueName)))
        {
            Log.WriteLine("Looping on league category name: " + categoryName);
            await GenerateCategory(_client, _guild, CategoryType.LEAGUETEMPLATE, categoryName);
        }
    }

    private static async Task GenerateRegularCategories(DiscordSocketClient _client, SocketGuild _guild)
    {
        foreach (CategoryType categoryName in Enum.GetValues(typeof(CategoryType)))
        {
            Log.WriteLine("Looping on category name: " + categoryName);

            if (categoriesThatWontGetGenerated.Contains(categoryName))
            {
                Log.WriteLine("category name is: " + categoryName +
                    " skipping creation for this type of category");
                continue;
            }

            await GenerateCategory(_client, _guild, categoryName, categoryName);
        }
    }

    private static async Task GenerateCategory(DiscordSocketClient _client, SocketGuild _guild, CategoryType _categoryType, Enum _categoryName)
    {
        try
        {
            Log.WriteLine("Generating category named: " + _categoryType);

            InterfaceCategory interfaceCategory = GetCategoryInstance(_categoryType);
            if (interfaceCategory == null)
            {
                Log.WriteLine(nameof(interfaceCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("interfaceCategory name: " + interfaceCategory.CategoryType, LogLevel.DEBUG);

            string finalCategoryName = EnumExtensions.GetEnumMemberAttrValue(_categoryName);
            Log.WriteLine("Category name is: " + interfaceCategory.CategoryType);

            SocketCategoryChannel? socketCategoryChannel = FindOrCreateSocketCategoryChannel(_guild, interfaceCategory, finalCategoryName);
            if (socketCategoryChannel == null)
            {
                Log.WriteLine(nameof(socketCategoryChannel).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            InterfaceLeague? interfaceLeague = null;
            if (_categoryType == CategoryType.LEAGUETEMPLATE)
            {
                InterfaceLeague leagueInterface = GetLeagueInstance((LeagueName)_categoryName);
                if (leagueInterface == null)
                {
                    Log.WriteLine(nameof(leagueInterface).ToString() + " was null!", LogLevel.CRITICAL);
                    return;
                }

                interfaceLeague = GetOrCreateLeague(leagueInterface, socketCategoryChannel.Id);
                if (interfaceLeague == null)
                {
                    Log.WriteLine(nameof(interfaceLeague).ToString() + " was null!", LogLevel.CRITICAL);
                    return;
                }
            }

            SocketRole? role = await RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(_guild, finalCategoryName);
            if (role == null)
            {
                Log.WriteLine(nameof(role).ToString() + " was null!", LogLevel.CRITICAL);
                return;
            }

            await interfaceCategory.CreateChannelsForTheCategory(socketCategoryChannel.Id, _client, role);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
        }
    }

    private static SocketCategoryChannel? FindOrCreateSocketCategoryChannel(
        SocketGuild _guild, InterfaceCategory _interfaceCategory, string _finalCategoryName)
    {
        bool contains = false;

        foreach (var ct in Database.Instance.Categories.CreatedCategoriesWithChannels)
        {
            // Checks specifically for league categories
            if (ct.Value.CategoryType == CategoryType.LEAGUETEMPLATE)
            {
                contains = CategoryRestore.CheckIfLeagueCategoryHasBeenDeletedAndRestoreForCategory(ct.Key, _guild, _finalCategoryName);
                if (contains)
                {
                    break;
                }
                continue;
            }

            if (ct.Value.CategoryType == _interfaceCategory.CategoryType)
            {
                contains = CategoryRestore.CheckIfCategoryHasBeenDeletedAndRestoreForCategory(ct.Key, _guild);
                if (contains)
                {
                    break;
                }
            }
        }

        if (contains)
        {
            InterfaceCategory? dbCategory = Database.Instance.Categories.FindInterfaceCategoryByCategoryName(_interfaceCategory.CategoryType);
            if (dbCategory == null)
            {
                Log.WriteLine(nameof(dbCategory).ToString() + " was null!", LogLevel.CRITICAL);
                return null;
            }

            SocketCategoryChannel? socketCategoryChannel = _guild.GetCategoryChannel(dbCategory.SocketCategoryChannelId);
            if (socketCategoryChannel == null)
            {
                Log.WriteLine(nameof(socketCategoryChannel).ToString() + " was null!", LogLevel.CRITICAL);
                return null;
            }

            return socketCategoryChannel;
        }
        else
        {
            SocketRole? role = RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(_guild, _finalCategoryName).Result;
            if (role == null)
            {
                Log.WriteLine(nameof(role).ToString() + " was null!", LogLevel.CRITICAL);
                return null;
            }

            SocketCategoryChannel? socketCategoryChannel =
                _interfaceCategory.CreateANewSocketCategoryChannelAndReturnIt(_guild, _finalCategoryName, role).Result;
            if (socketCategoryChannel == null)
            {
                Log.WriteLine(nameof(socketCategoryChannel).ToString() + " was null!", LogLevel.CRITICAL);
                return null;
            }

            Database.Instance.Categories.AddToCreatedCategoryWithChannelWithUlongAndInterfaceCategory(socketCategoryChannel.Id, _interfaceCategory);
            return socketCategoryChannel;
        }
    }

    private static InterfaceCategory? GetCategoryInstance(CategoryType _categoryType)
    {
        try
        {
            return (InterfaceCategory)EnumExtensions.GetInstance(_categoryType.ToString());
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            throw new InvalidOperationException(ex.Message);
        }
    }

    private static InterfaceLeague? GetLeagueInstance(LeagueName leagueCategoryName)
    {
        try
        {
            return (InterfaceLeague)EnumExtensions.GetInstance(leagueCategoryName.ToString());
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            throw new InvalidOperationException(ex.Message);
        }
    }

    private static InterfaceLeague? GetOrCreateLeague(InterfaceLeague _leagueInterface, ulong _socketCategoryChannelId)
    {
        if (Database.Instance.Leagues.CheckIfILeagueExistsByCategoryName(_leagueInterface.LeagueCategoryName))
        {
            InterfaceLeague interfaceLeague = Database.Instance.Leagues.GetILeagueByCategoryName(_leagueInterface.LeagueCategoryName);
            if (interfaceLeague == null)
            {
                Log.WriteLine(nameof(interfaceLeague).ToString() + " was null!", LogLevel.CRITICAL);
                return null;
            }

            interfaceLeague.LeagueData.ChallengeStatus.TeamsInTheQueue.Clear();
            var message = Database.Instance.Categories.FindInterfaceCategoryWithId(interfaceLeague.LeagueCategoryId)
                .FindInterfaceChannelWithNameInTheCategory(ChannelType.CHALLENGE)
                .FindInterfaceMessageWithNameInTheChannel(MessageName.CHALLENGEMESSAGE);
            message.GenerateAndModifyTheMessage();

            interfaceLeague.LeagueUnits = _leagueInterface.LeagueUnits;
            return interfaceLeague;
        }
        else
        {
            _leagueInterface.LeagueCategoryId = _socketCategoryChannelId;
            Database.Instance.Leagues.AddToStoredLeagues(_leagueInterface);
            _leagueInterface.LeagueData.SetReferences(_leagueInterface);
            return _leagueInterface;
        }
    }
}
