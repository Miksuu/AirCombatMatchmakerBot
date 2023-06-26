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
        Log.WriteLine("Checking if " + _leagueCategoryName + " exists.");
        exists = StoredLeagues.Any(x => x.LeagueCategoryName == _leagueCategoryName);
        Log.WriteLine(_leagueCategoryName + " exists: " + exists);
        return exists;
    }

    public bool CheckIfILeagueExistsByCategoryId(ulong _categoryId)
    {
        bool exists = false;
        Log.WriteLine("Checking if " + _categoryId + " exists.");
        exists = StoredLeagues.Any(x => x.LeagueCategoryId == _categoryId);
        Log.WriteLine(_categoryId + " exists: " + exists);
        return exists;
    }

    // Might want to add a check that it exists, use the method above
    public InterfaceLeague GetILeagueByCategoryName(LeagueName _leagueCategoryName)
    {
        Log.WriteLine("Getting ILeague by category name: " + _leagueCategoryName);

        InterfaceLeague? FoundLeague = 
            StoredLeagues.FirstOrDefault(x => x.LeagueCategoryName == _leagueCategoryName);
        if (FoundLeague == null)
        {
            Log.WriteLine(nameof(FoundLeague) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(FoundLeague) + " was null!");
        }

        Log.WriteLine("Found: " + FoundLeague.LeagueCategoryName);
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

        Log.WriteLine("Found: " + FoundLeague.LeagueCategoryName);
        return FoundLeague;
    }

    // Maybe unnecessary to get it by string
    public InterfaceLeague GetILeagueByString(string _leagueCategoryNameString)
    {
        Log.WriteLine("Getting ILeague by string: " + _leagueCategoryNameString);

        InterfaceLeague? FoundLeague = StoredLeagues.FirstOrDefault(
            x => x.LeagueCategoryName.ToString() == _leagueCategoryNameString);
        if (FoundLeague == null)
        {
            Log.WriteLine(nameof(FoundLeague) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(FoundLeague) + " was null!");
        }

        Log.WriteLine("Found: " + FoundLeague.LeagueCategoryName);
        return FoundLeague;
    }

    public void AddToStoredLeagues(InterfaceLeague _ILeague)
    {
        Log.WriteLine("Adding ILeague: " + _ILeague.LeagueCategoryName +
            "to the StoredLeague ConcurrentBag");
        StoredLeagues.Add(_ILeague);
        Log.WriteLine("Done adding, count is now: " + StoredLeagues.Count);
    }

    public void HandleSettingTeamsInactiveThatUserWasIn(ulong _userId)
    {
        Log.WriteLine("Starting to set teams inactive that " + _userId + " was in.");

        foreach (InterfaceLeague storedLeague in StoredLeagues)
        {
            Log.WriteLine("Looping through league: " +
                storedLeague.LeagueCategoryName);

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
                            player.PlayerDiscordId + ")");
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

                            break;
                        }
                    }
                }
                else
                {
                    Log.WriteLine("The team was already found in the league, breaking and proceeding" +
                        " to the next one.");
                    break;
                }
            }
        }
    }
}