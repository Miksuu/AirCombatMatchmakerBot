using System.Runtime.Serialization;

[DataContract]
public class Leagues
{
    public List<InterfaceLeague> StoredLeagues
    {
        get
        {
            Log.WriteLine("Getting " + nameof(storedLeagues) + " with count of: " +
                storedLeagues.Count, LogLevel.VERBOSE);
            return storedLeagues;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(storedLeagues)
                + " to: " + value, LogLevel.VERBOSE);
            storedLeagues = value;
        }
    }

    public int LeaguesMatchCounter
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leaguesMatchCounter)
                + ": " + leaguesMatchCounter, LogLevel.VERBOSE);
            return leaguesMatchCounter;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leaguesMatchCounter) +
                leaguesMatchCounter + " to: " + value, LogLevel.VERBOSE);
            leaguesMatchCounter = value;
        }
    }

    [DataMember] private List<InterfaceLeague> storedLeagues { get; set; }
    [DataMember] private int leaguesMatchCounter { get; set; }

    public Leagues()
    {
        storedLeagues = new List<InterfaceLeague>();
        leaguesMatchCounter = 1;
    }

    public bool CheckIfILeagueExistsByCategoryName(CategoryType _leagueCategoryName)
    {
        bool exists = false;
        Log.WriteLine("Checking if " + _leagueCategoryName + " exists.", LogLevel.VERBOSE);
        exists = StoredLeagues.Any(x => x.LeagueCategoryName == _leagueCategoryName);
        Log.WriteLine(_leagueCategoryName + " exists: " + exists, LogLevel.VERBOSE);
        return exists;
    }

    // Might want to add a check that it exists, use the method above
    public InterfaceLeague? GetILeagueByCategoryName(CategoryType? _leagueCategoryName)
    {
        Log.WriteLine("Getting ILeague by category name: " + _leagueCategoryName, LogLevel.VERBOSE);
        InterfaceLeague? FoundLeague = StoredLeagues.FirstOrDefault(x => x.LeagueCategoryName == _leagueCategoryName);

        if (FoundLeague == null)
        {
            Log.WriteLine(nameof(FoundLeague) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found: " + FoundLeague.LeagueCategoryName, LogLevel.VERBOSE);
        return FoundLeague;
    }

    // Maybe unnecessary to get it by string
    public InterfaceLeague? GetILeagueByString(string _leagueCategoryNameString)
    {
        Log.WriteLine("Getting ILeague by string: " + _leagueCategoryNameString, LogLevel.VERBOSE);
        InterfaceLeague? FoundLeague = StoredLeagues.FirstOrDefault(
            x => x.LeagueCategoryName.ToString() == _leagueCategoryNameString);

        if (FoundLeague == null)
        {
            Log.WriteLine(nameof(FoundLeague) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found: " + FoundLeague.LeagueCategoryName, LogLevel.VERBOSE);
        return FoundLeague;
    }

    public InterfaceLeague? GetILeagueByCategoryId(ulong _leagueCategoryId)
    {
        Log.WriteLine("Getting ILeague by ID: " + _leagueCategoryId, LogLevel.VERBOSE);
        InterfaceLeague? FoundLeague = StoredLeagues.FirstOrDefault(
            x => x.DiscordLeagueReferences.LeagueCategoryId == _leagueCategoryId);

        if (FoundLeague == null)
        {
            Log.WriteLine(nameof(FoundLeague) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Found: " + FoundLeague.LeagueCategoryName, LogLevel.VERBOSE);
        return FoundLeague;
    }

    public void AddToStoredLeagues(InterfaceLeague _ILeague)
    {
        Log.WriteLine("Adding ILeague: " + _ILeague.LeagueCategoryName +
            "to the StoredLeague list", LogLevel.VERBOSE);
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

            foreach (Team team in storedLeague.LeagueData.Teams.TeamsList)
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

                            if (storedLeagueString == null)
                            {
                                Log.WriteLine("storedLeagueString was null!", LogLevel.CRITICAL);
                                continue;
                            }

                            InterfaceLeague? findLeagueCategoryType = GetILeagueByString(storedLeagueString);

                            if (findLeagueCategoryType == null)
                            {
                                Log.WriteLine(nameof(findLeagueCategoryType) + " was null!", LogLevel.CRITICAL);
                                return;
                            }

                            CategoryType leagueCategoryName = findLeagueCategoryType.LeagueCategoryName;

                            var leagueInterface =
                                LeagueManager.GetLeagueInstanceWithLeagueCategoryName(
                                    leagueCategoryName);
                            Log.WriteLine("Found " + nameof(leagueInterface) + ": " +
                                leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

                            InterfaceLeague? dbLeagueInstance =
                                Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(
                                    leagueInterface);

                            if (dbLeagueInstance == null)
                            {
                                Log.WriteLine("dbLeagueInstance was null!", LogLevel.CRITICAL);
                                continue;
                            }

                            dbLeagueInstance.ModifyLeagueRegisterationChannelMessage();

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

    public InterfaceLeague? GetInterfaceLeagueCategoryFromTheDatabase(InterfaceLeague _leagueInterface)
    {
        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Checking if " + _leagueInterface.LeagueCategoryName +
            " has _leagueInterface in the database", LogLevel.VERBOSE);

        if (CheckIfILeagueExistsByCategoryName(_leagueInterface.LeagueCategoryName))
        {
            Log.WriteLine(_leagueInterface.LeagueCategoryName +
                " exists in the database!", LogLevel.DEBUG);

            var newInterfaceLeagueCategory = GetILeagueByCategoryName(_leagueInterface.LeagueCategoryName);

            if (newInterfaceLeagueCategory == null)
            {
                Log.WriteLine(nameof(newInterfaceLeagueCategory) + " was null!", LogLevel.CRITICAL);
                return null;
            }

            Log.WriteLine("found result: " +
                newInterfaceLeagueCategory.LeagueCategoryName, LogLevel.DEBUG);
            return newInterfaceLeagueCategory;
        }
        else
        {
            Log.WriteLine(_leagueInterface.LeagueCategoryName + " does not exist in the database," +
                " creating a new LeagueData for it", LogLevel.DEBUG);

            _leagueInterface.LeagueData = new LeagueData();
            _leagueInterface.DiscordLeagueReferences = new DiscordLeagueReferences();

            return _leagueInterface;
        }
    }
    public InterfaceLeague FindLeagueInterfaceWithLeagueCategoryId(
        ulong _leagueCategoryId)
    {
        Log.WriteLine("Starting to find Ileague from db with: " +
            _leagueCategoryId, LogLevel.VERBOSE);

        KeyValuePair<ulong, InterfaceCategory> findLeagueCategoryType =
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _leagueCategoryId);
        CategoryType leagueCategoryName = findLeagueCategoryType.Value.CategoryType;

        Log.WriteLine("found: " + nameof(leagueCategoryName) + ": " +
            leagueCategoryName.ToString(), LogLevel.VERBOSE);

        var leagueInterface =
            LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueCategoryName);

        Log.WriteLine(
            "Found interface " + nameof(leagueInterface) + ": " +
            leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

        InterfaceLeague? dbLeagueInstance =
            Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(leagueInterface);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine(nameof(dbLeagueInstance) + " db: " +
            dbLeagueInstance.LeagueCategoryName, LogLevel.VERBOSE);
        return dbLeagueInstance;
    }
}