using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class Matches
{
    public ConcurrentBag<LeagueMatch> MatchesConcurrentBag
    {
        get
        {
            Log.WriteLine("Getting " + nameof(matchesConcurrentBag) + " with count of: " +
                matchesConcurrentBag.Count, LogLevel.VERBOSE);
            return matchesConcurrentBag;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(matchesConcurrentBag) + matchesConcurrentBag
                + " to: " + value, LogLevel.VERBOSE);
            matchesConcurrentBag = value;
        }
    }

    [DataMember] private ConcurrentBag<LeagueMatch> matchesConcurrentBag { get; set; }

    public Matches() 
    {
        matchesConcurrentBag = new ConcurrentBag<LeagueMatch>();
    }

    public async void CreateAMatch(InterfaceLeague _interfaceLeague, int[] _teamsToFormMatchOn)
    {
        Log.WriteLine("Creating a match with teams ids: " + _teamsToFormMatchOn[0] + " and " +
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

        LeagueMatch newMatch = new(_interfaceLeague, _teamsToFormMatchOn);
        MatchesConcurrentBag.Add(newMatch);
        Log.WriteLine("Added to the " + nameof(matchesConcurrentBag) + " count is now: " +
            matchesConcurrentBag.Count, LogLevel.VERBOSE);

        await CreateAMatchChannel(guild, newMatch, _interfaceLeague);

        await SerializationManager.SerializeDB();
    }

    public async Task<ulong> CreateAMatchChannel(
        SocketGuild _guild,
        LeagueMatch _leagueMatch,
        InterfaceLeague _interfaceLeague)
    {
        // Get the category by the league category name passed in the method
        var categoryKvp =
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpByCategoryName(
                _interfaceLeague.LeagueCategoryName);

        // Prep the channel name with match id
        string overriddenMatchName = "match-" + _leagueMatch.MatchId.ToString();

        Log.WriteLine("Starting to create a new match channel: " +
            overriddenMatchName, LogLevel.VERBOSE);

        // Prepare the match with the ID of the current new match
        InterfaceChannel? interfaceChannel = await categoryKvp.Value.CreateSpecificChannelFromChannelType(
                _guild, ChannelType.MATCHCHANNEL, categoryKvp.Value.SocketCategoryChannelId,
                overriddenMatchName, // Override's the channel's name with the match name with that match-[id]
                _leagueMatch.GetIdsOfThePlayersInTheMatchAsArray(_interfaceLeague));

        if (interfaceChannel == null)
        {
            Log.WriteLine(nameof(interfaceChannel) + " was null!", LogLevel.ERROR);
            return 0;
        }


        _leagueMatch.MatchChannelId = interfaceChannel.ChannelId;

        if (!Database.Instance.Categories.MatchChannelsIdWithCategoryId.ContainsKey(
            interfaceChannel.ChannelId))
        {
            Database.Instance.Categories.MatchChannelsIdWithCategoryId.TryAdd(
                interfaceChannel.ChannelId, categoryKvp.Value.SocketCategoryChannelId);
        }

        await interfaceChannel.PostChannelMessages(_guild);

        Log.WriteLine("DONE CREATING A MATCH CHANNEL!", LogLevel.VERBOSE);

        return _leagueMatch.MatchChannelId;
    }

    public LeagueMatch? FindLeagueMatchByTheChannelId(ulong _channelId)
    {
        Log.WriteLine("Getting match by channelId: " + _channelId, LogLevel.VERBOSE);

        /*
        foreach (var item in MatchesConcurrentBag)
        {
            Log.WriteLine("ChannelID debug: " + item.MatchChannelId, LogLevel.DEBUG);
        }*/

        LeagueMatch? foundMatch = MatchesConcurrentBag.FirstOrDefault(x => x.MatchChannelId == _channelId);

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