using Discord.WebSocket;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class Leagues : logClass<Leagues>
{
    public ConcurrentBag<InterfaceLeague> StoredLeagues
    {
        get => storedLeagues.GetValue();
        set => storedLeagues.SetValue(value);
    }

    public int LeaguesMatchCounter
    {
        get => leaguesMatchCounter.GetValue();
        set => leaguesMatchCounter.SetValue(value);
    }

    [DataMember] private logConcurrentBag<InterfaceLeague> storedLeagues = new logConcurrentBag<InterfaceLeague>();
    [DataMember] private logClass<int> leaguesMatchCounter = new logClass<int>(1);

    public bool CheckIfILeagueExistsByCategoryName(LeagueName _leagueCategoryName)
    {
        bool exists = false;
        Log.WriteLine("Checking if " + _leagueCategoryName + " exists.", LogLevel.VERBOSE);
        exists = StoredLeagues.Any(x => x.LeagueCategoryName == _leagueCategoryName);
        Log.WriteLine(_leagueCategoryName + " exists: " + exists, LogLevel.VERBOSE);
        return exists;
    }

    // Might want to add a check that it exists, use the method above
    public InterfaceLeague GetILeagueByCategoryName(LeagueName _leagueCategoryName)
    {
        Log.WriteLine("Getting ILeague by category name: " + _leagueCategoryName, LogLevel.VERBOSE);

        InterfaceLeague? FoundLeague = 
            StoredLeagues.FirstOrDefault(x => x.LeagueCategoryName == _leagueCategoryName);
        if (FoundLeague == null)
        {
            Log.WriteLine(nameof(FoundLeague) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(FoundLeague) + " was null!");
        }

        Log.WriteLine("Found: " + FoundLeague.LeagueCategoryName, LogLevel.VERBOSE);
        return FoundLeague;
    }

    public InterfaceLeague GetILeagueByCategoryId(ulong _leagueCategoryId)
    {
        Log.WriteLine("Getting ILeague by ID: " + _leagueCategoryId + " with " + nameof(StoredLeagues) +
            " count: " + StoredLeagues.Count, LogLevel.DEBUG);

        InterfaceLeague? FoundLeague = StoredLeagues.FirstOrDefault(
            x => x.LeagueCategoryId == _leagueCategoryId);
        if (FoundLeague == null)
        {
            Log.WriteLine(nameof(FoundLeague) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(FoundLeague) + " was null!");
        }

        Log.WriteLine("Found: " + FoundLeague.LeagueCategoryName, LogLevel.VERBOSE);
        return FoundLeague;
    }

    // Maybe unnecessary to get it by string
    public InterfaceLeague GetILeagueByString(string _leagueCategoryNameString)
    {
        Log.WriteLine("Getting ILeague by string: " + _leagueCategoryNameString, LogLevel.VERBOSE);

        InterfaceLeague? FoundLeague = StoredLeagues.FirstOrDefault(
            x => x.LeagueCategoryName.ToString() == _leagueCategoryNameString);
        if (FoundLeague == null)
        {
            Log.WriteLine(nameof(FoundLeague) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(FoundLeague) + " was null!");
        }

        Log.WriteLine("Found: " + FoundLeague.LeagueCategoryName, LogLevel.VERBOSE);
        return FoundLeague;
    }

    public void AddToStoredLeagues(InterfaceLeague _ILeague)
    {
        Log.WriteLine("Adding ILeague: " + _ILeague.LeagueCategoryName +
            "to the StoredLeague ConcurrentBag", LogLevel.VERBOSE);
        StoredLeagues.Add(_ILeague);
        Log.WriteLine("Done adding, count is now: " + StoredLeagues.Count, LogLevel.VERBOSE);
    }

    public void HandleSettingTeamsInactiveThatUserWasIn(ulong _userId)
    {
        Log.WriteLine("Starting to set teams inactive that " + _userId + " was in.", LogLevel.VERBOSE);

        foreach (InterfaceLeague storedLeague in StoredLeagues)
        {
            Log.WriteLine("Looping through league: " +
                storedLeague.LeagueCategoryName, LogLevel.VERBOSE);

            bool teamFound = false;

            if (storedLeague == null)
            {
                Log.WriteLine("storedLeague was null!", LogLevel.CRITICAL);
                continue;
            }

            string? storedLeagueString = storedLeague.ToString();
            if (storedLeagueString == null)
            {
                Log.WriteLine("storedLeagueString was null!", LogLevel.CRITICAL);
                return;
            }


            foreach (Team team in storedLeague.LeagueData.Teams.TeamsConcurrentBag)
            {
                if (!teamFound)
                {
                    foreach (Player player in team.Players)
                    {
                        Log.WriteLine("Looping through player: " + player.PlayerNickName + " (" +
                            player.PlayerDiscordId + ")", LogLevel.VERBOSE);
                        if (player.PlayerDiscordId == _userId)
                        {
                            team.TeamActive = false;

                            teamFound = true;
                            Log.WriteLine("Set team: " + team.GetTeamName(
                                storedLeague.LeaguePlayerCountPerTeam) + " deactive in league: " +
                                storedLeague.LeagueCategoryName + " because " + player.PlayerNickName +
                                " left", LogLevel.DEBUG);

                            InterfaceLeague findLeagueCategoryType;
                            InterfaceChannel interfaceChannel;
                            try
                            {
                                findLeagueCategoryType = GetILeagueByString(storedLeagueString);

                                interfaceChannel = Database.Instance.Categories.FindInterfaceCategoryByCategoryName(
                                    CategoryType.REGISTRATIONCATEGORY).FindInterfaceChannelWithNameInTheCategory(
                                        ChannelType.LEAGUEREGISTRATION);
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                                return;
                            }

                            var leagueRegistrationMessages = interfaceChannel.InterfaceMessagesWithIds.Where(
                                m => m.Value.MessageName == MessageName.LEAGUEREGISTRATIONMESSAGE);

                            foreach (var messageKvp in leagueRegistrationMessages)
                            {
                                messageKvp.Value.GenerateAndModifyTheMessage();
                            }
                                /*
                                CategoryType leagueCategoryName = findLeagueCategoryType.LeagueCategoryName;


                                var leagueInterface =
                                    LeagueManager.GetLeagueInstanceWithLeagueCategoryName(
                                        leagueCategoryName);
                                Log.WriteLine("Found " + nameof(leagueInterface) + ": " +
                                    leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

                                InterfaceLeague dbLeagueInstance =
                                    Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(
                                        leagueInterface);
                                    */

                                //dbLeagueInstance.ModifyLeagueRegisterationChannelMessage();

                            break;
                        }
                    }
                }
                else
                {
                    Log.WriteLine("The team was already found in the league, breaking and proceeding" +
                        " to the next one.", LogLevel.VERBOSE);
                    break;
                }
            }
        }
    }

    public InterfaceLeague GetInterfaceLeagueCategoryFromTheDatabase(InterfaceLeague _leagueInterface)
    {
        Log.WriteLine("Checking if " + _leagueInterface.LeagueCategoryName +
            " has _leagueInterface in the database", LogLevel.VERBOSE);

        if (CheckIfILeagueExistsByCategoryName(_leagueInterface.LeagueCategoryName))
        {
            Log.WriteLine(_leagueInterface.LeagueCategoryName +
                " exists in the database!", LogLevel.DEBUG);

            InterfaceLeague? interfaceLeague = null;
            try
            {
                interfaceLeague = GetILeagueByCategoryName(_leagueInterface.LeagueCategoryName);

                Log.WriteLine("found result: " +
                    interfaceLeague.LeagueCategoryName, LogLevel.DEBUG);
                return interfaceLeague;
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                return interfaceLeague ?? _leagueInterface;
            }
        }
        else
        {
            Log.WriteLine(_leagueInterface.LeagueCategoryName + " does not exist in the database," +
                " creating a new LeagueData for it", LogLevel.DEBUG);

            _leagueInterface.LeagueData = new LeagueData(_leagueInterface);

            return _leagueInterface;
        }
    }
    public InterfaceLeague FindLeagueInterfaceWithLeagueCategoryId(
        ulong _leagueCategoryId)
    {
        Log.WriteLine("Starting to find Ileague from db with: " +
            _leagueCategoryId, LogLevel.VERBOSE);

        //var findLeagueCategoryType =
        //    Database.Instance.Categories.FindInterfaceCategoryWithId(_leagueCategoryId);
        //LeagueName leagueCategoryName = findLeagueCategoryType.CategoryType;

        /*
        Log.WriteLine("found: " + nameof(leagueCategoryName) + ": " +
            leagueCategoryName.ToString(), LogLevel.VERBOSE);
        */

        var leagueCategoryName = Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(_leagueCategoryId);

        var leagueInterface =
            LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueCategoryName.LeagueCategoryName);

        Log.WriteLine(
            "Found interface " + nameof(leagueInterface) + ": " +
            leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

        InterfaceLeague dbLeagueInstance =
            Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(leagueInterface);

        Log.WriteLine(nameof(dbLeagueInstance) + " db: " +
            dbLeagueInstance.LeagueCategoryName, LogLevel.VERBOSE);
        return dbLeagueInstance;
    }

    public InterfaceLeague FindLeagueInterfaceWithLeagueCategoryName(
        string _categoryName)
    {
        Log.WriteLine("Starting to find Ileague from db with: " +
            _categoryName, LogLevel.VERBOSE);

        LeagueName categoryType = (LeagueName)EnumExtensions.GetInstance(_categoryName);

        Log.WriteLine("found: " + nameof(categoryType) + ": " +
            categoryType.ToString(), LogLevel.VERBOSE);

        var leagueInterface =
            LeagueManager.GetLeagueInstanceWithLeagueCategoryName(categoryType);

        Log.WriteLine(
            "Found interface " + nameof(leagueInterface) + ": " +
            leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

        InterfaceLeague dbLeagueInstance =
            Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(leagueInterface);

        Log.WriteLine(nameof(dbLeagueInstance) + " db: " +
            dbLeagueInstance.LeagueCategoryName, LogLevel.VERBOSE);
        return dbLeagueInstance;
    }

    public (InterfaceLeague?, LeagueMatch?) FindLeagueInterfaceAndLeagueMatchWithChannelId(ulong _channelId)
    {
        Log.WriteLine("Trying to find the league interface with the league match with channel id: " +
            _channelId, LogLevel.VERBOSE);

        LeagueMatch? foundMatch = null;
        InterfaceLeague? interfaceLeague = null;
        try
        {
            // Find the league with the cached category ID
            interfaceLeague =
                Database.Instance.Leagues.GetILeagueByCategoryId(
                    Database.Instance.Categories.MatchChannelsIdWithCategoryId[_channelId]);
            foundMatch = interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(_channelId);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return (interfaceLeague, foundMatch);
        }
        Log.WriteLine("Returning: " + interfaceLeague.LeagueCategoryName +
            " | " + foundMatch.MatchId, LogLevel.DEBUG);

        return (interfaceLeague, foundMatch);
    }

    public (InterfaceLeague?, LeagueMatch?) FindMatchAndItsInterfaceLeagueByCategoryAndChannelId(
        ulong _messageCategoryId, ulong _messageChannelId)
    {
        Log.WriteLine("Finding the match and its interface league with category: " + _messageCategoryId +
            " and channel id:" + _messageChannelId, LogLevel.VERBOSE);

        InterfaceLeague interfaceLeague = FindLeagueInterfaceWithLeagueCategoryId(_messageCategoryId);

        LeagueMatch? leagueMatch = null;
        try
        {
            leagueMatch = interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(_messageChannelId);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return (interfaceLeague, leagueMatch);
        }

        return (interfaceLeague, leagueMatch);
    }
}