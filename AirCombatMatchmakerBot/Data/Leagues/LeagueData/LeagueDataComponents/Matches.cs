using System.Runtime.Serialization;

[DataContract]
public class Matches
{
    public List<LeagueMatch> MatchesList
    {
        get
        {
            Log.WriteLine("Getting " + nameof(matchesList) + " with count of: " +
                matchesList.Count, LogLevel.VERBOSE);
            return matchesList;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(matchesList) + matchesList
                + " to: " + value, LogLevel.VERBOSE);
            matchesList = value;
        }
    }

    [DataMember] private List<LeagueMatch> matchesList { get; set; }

    public Matches() 
    {
        matchesList = new List<LeagueMatch>();
    }

    public void CreateAMatch(InterfaceLeague _interfaceLeague, int[] _teamsToFormMatchOn)
    {
        Log.WriteLine("Creating a match with teams ids: " + _teamsToFormMatchOn[0] +
            _teamsToFormMatchOn[1], LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        if (_teamsToFormMatchOn.Length != 2)
        {
            Log.WriteLine("Warning! teams Length was not 2!", LogLevel.ERROR);
        }

        LeagueMatch newMatch = new(_teamsToFormMatchOn);

        MatchesList.Add(newMatch);

        Log.WriteLine("Added to the " + nameof(matchesList) + " count is now: " +
            matchesList.Count, LogLevel.VERBOSE);

        var categoryKvp =
            Database.Instance.Categories.GetCreatedCategoryWithChannelKvpByCategoryName(
                _interfaceLeague.LeagueCategoryName);

        categoryKvp.Value.CreateSpecificChannelFromChannelName(
            guild, ChannelName.MATCHCHANNEL, categoryKvp.Value.SocketCategoryChannelId);
    }
}