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
                    foreach (Player player in team.players)
                    {
                        Log.WriteLine("Looping through player: " + player.GetPlayerNickname() + " (" +
                            player.GetPlayerDiscordId() + ")", LogLevel.VERBOSE);
                        if (player.GetPlayerDiscordId() == _userId)
                        {
                            team.teamActive = false;

                            teamFound = true;
                            Log.WriteLine("Set team: " + team.teamName + " deactive in league: " +
                                storedLeague.LeagueCategoryName + " because " + player.GetPlayerNickname() +
                                " left", LogLevel.DEBUG);

                            if (storedLeagueString == null)
                            {
                                Log.WriteLine("storedLeagueString was null!", LogLevel.CRITICAL);
                                continue;
                            }

                            var findLeagueCategoryType = GetILeagueByString(storedLeagueString);
                            CategoryName leagueCategoryName = findLeagueCategoryType.LeagueCategoryName;

                            var leagueInterface =
                                LeagueManager.GetLeagueInstanceWithLeagueCategoryName(
                                    leagueCategoryName);
                            Log.WriteLine("Found " + nameof(leagueInterface) + ": " +
                                leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

                            ILeague? dbLeagueInstance =
                                LeagueManager.FindLeagueAndReturnInterfaceFromDatabase(
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
}