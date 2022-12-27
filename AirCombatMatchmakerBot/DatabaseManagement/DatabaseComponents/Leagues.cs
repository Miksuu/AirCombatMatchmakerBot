using System.Runtime.Serialization;

[DataContract]
public class Leagues
{
    [DataMember] private List<ILeague> StoredLeagues { get; set; }

    public Leagues()
    {
        StoredLeagues = new List<ILeague>();
    }

    public bool CheckIfILeagueExistsByCategoryName(CategoryName _leagueCategoryName)
    {
        bool exists = false;
        Log.WriteLine("Checking if " + _leagueCategoryName + " exists.", LogLevel.VERBOSE);
        exists = StoredLeagues.Any(x => x.LeagueCategoryName == _leagueCategoryName);
        Log.WriteLine(_leagueCategoryName + " exists: " + exists, LogLevel.VERBOSE);
        return exists;
    }

    // Might want to add a check that it exists, use the method above
    public ILeague GetILeagueByCategoryName(CategoryName? _leagueCategoryName)
    {
        Log.WriteLine("Getting ILeague by category name: " + _leagueCategoryName, LogLevel.VERBOSE);
        ILeague FoundLeague = StoredLeagues.First(x => x.LeagueCategoryName == _leagueCategoryName);
        Log.WriteLine("Found: " + FoundLeague.LeagueCategoryName, LogLevel.VERBOSE);
        return FoundLeague;
    }

    // Maybe unnecessary to get it by string
    public ILeague GetILeagueByString(string _leagueCategoryNameString)
    {
        Log.WriteLine("Getting ILeague by string: " + _leagueCategoryNameString, LogLevel.VERBOSE);
        ILeague FoundLeague = StoredLeagues.First(
            x => x.LeagueCategoryName.ToString() == _leagueCategoryNameString);
        Log.WriteLine("Found: " + FoundLeague.LeagueCategoryName, LogLevel.VERBOSE);
        return FoundLeague;
    }

    public void AddToStoredLeagues(ILeague _ILeague)
    {
        Log.WriteLine("Adding ILeague: " + _ILeague.LeagueCategoryName +
            "to the StoredLeague list", LogLevel.VERBOSE);
        StoredLeagues.Add(_ILeague);
        Log.WriteLine("Done adding, count is now: " + StoredLeagues.Count, LogLevel.VERBOSE);
    }

    public List<ILeague> GetListOfStoredLeagues()
    {
        Log.WriteLine("Getting list of ILeagues with count of: " + StoredLeagues.Count, LogLevel.VERBOSE);
        return StoredLeagues;
    }

    public async void HandleSettingTeamsInactiveThatUserWasIn(ulong _userId)
    {
        Log.WriteLine("Starting to set teams inactive that " + _userId + " was in.", LogLevel.VERBOSE);

        foreach (ILeague storedLeague in StoredLeagues)
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

            foreach (Team team in storedLeague.LeagueData.Teams.GetListOfTeams())
            {
                if (!teamFound)
                {
                    foreach (Player player in team.GetListOfPlayersInATeam())
                    {
                        Log.WriteLine("Looping through player: " + player.GetPlayerNickname() + " (" +
                            player.GetPlayerDiscordId() + ")", LogLevel.VERBOSE);
                        if (player.GetPlayerDiscordId() == _userId)
                        {
                            team.SetTheActive(false);

                            teamFound = true;
                            Log.WriteLine("Set team: " + team.GetTeamName() + " deactive in league: " +
                                storedLeague.LeagueCategoryName + " because " + player.GetPlayerNickname() +
                                " left", LogLevel.DEBUG);

                            if (storedLeagueString == null)
                            {
                                Log.WriteLine("storedLeagueString was null!", LogLevel.CRITICAL);
                                continue;
                            }

                            ILeague findLeagueCategoryType = GetILeagueByString(storedLeagueString);
                            CategoryName leagueCategoryName = findLeagueCategoryType.LeagueCategoryName;

                            var leagueInterface =
                                LeagueManager.GetLeagueInstanceWithLeagueCategoryName(
                                    leagueCategoryName);
                            Log.WriteLine("Found " + nameof(leagueInterface) + ": " +
                                leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

                            ILeague? dbLeagueInstance =
                                Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(
                                    leagueInterface);

                            if (dbLeagueInstance == null)
                            {
                                Log.WriteLine("dbLeagueInstance was null!", LogLevel.CRITICAL);
                                continue;
                            }

                            await MessageManager.ModifyLeagueRegisterationChannelMessage(
                                dbLeagueInstance);

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

    public ILeague? GetInterfaceLeagueCategoryFromTheDatabase(ILeague _leagueInterface)
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
}