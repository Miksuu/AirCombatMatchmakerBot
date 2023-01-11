using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

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

    public async void CreateAMatch(InterfaceLeague _interfaceLeague, int[] _teamsToFormMatchOn)
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

        await CreateAMatchChannel(guild, newMatch, _interfaceLeague);

        await SerializationManager.SerializeDB();
    }

    public async Task CreateAMatchChannel(
        SocketGuild _guild,
        LeagueMatch _leagueMatch,
        InterfaceLeague _interfaceLeague)
    {
        // Get the category by the league category name passed in the method
        var categoryKvp =
            Database.Instance.Categories.GetCreatedCategoryWithChannelKvpByCategoryName(
                _interfaceLeague.LeagueCategoryName);

        // Prep the channel name with match id
        string overriddenMatchName = "match-" + _leagueMatch.MatchId.ToString();

        Log.WriteLine("Starting to create a new match channel: " +
            overriddenMatchName, LogLevel.VERBOSE);

        // Prepare the match with the ID of the current new match
        InterfaceChannel interfaceChannel = await categoryKvp.Value.CreateSpecificChannelFromChannelType(
                _guild, ChannelType.MATCHCHANNEL, categoryKvp.Value.SocketCategoryChannelId,
                overriddenMatchName, // Override's the channel's name with the match name with that match-[id]
                _leagueMatch.GetIdsOfThePlayersInTheMatchAsArray(_interfaceLeague));

        _leagueMatch.MatchChannelId = interfaceChannel.ChannelId;

        await interfaceChannel.PrepareChannelMessages();

        Log.WriteLine("DONE CREATING A MATCH CHANNEL!", LogLevel.VERBOSE);
    }

    public LeagueMatch? FindLeagueMatchByTheChannelId(ulong _channelId)
    {
        Log.WriteLine("Getting match by channelId: " + _channelId, LogLevel.VERBOSE);

        foreach (var item in MatchesList)
        {
            Log.WriteLine("ChannelID debug: " + item.MatchChannelId, LogLevel.DEBUG);
        }

        LeagueMatch? foundMatch = MatchesList.FirstOrDefault(x => x.MatchChannelId == _channelId);

        if (foundMatch == null) 
        {
            Log.WriteLine(nameof(foundMatch) + " was null!", LogLevel.ERROR);
            return null;
        }

        Log.WriteLine("Found Match: " + foundMatch.MatchId + " with channelId: " +
            _channelId, LogLevel.VERBOSE);

        return foundMatch;
    }
}