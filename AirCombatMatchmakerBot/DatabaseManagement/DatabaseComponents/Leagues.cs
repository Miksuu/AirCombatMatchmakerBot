[Serializable]
public class Leagues
{
    private List<ILeague> StoredLeagues { get; set; }

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

    public List<ILeague> GetListOfStoredLeagues()
    {
        Log.WriteLine("Getting list of ILeagues with count of: " + StoredLeagues.Count, LogLevel.VERBOSE);
        return StoredLeagues;
    }

    public void AddToStoredLeagues(ILeague _ILeague)
    {
        Log.WriteLine("Adding ILeague: " + _ILeague.LeagueCategoryName +
            "to the StoredLeague list", LogLevel.VERBOSE);
        StoredLeagues.Add(_ILeague);
        Log.WriteLine("Done adding, count is now: " + StoredLeagues.Count, LogLevel.VERBOSE);
    }
}