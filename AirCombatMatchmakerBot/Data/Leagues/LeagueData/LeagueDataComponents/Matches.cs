using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class Matches : logClass<Matches>, InterfaceLoggableClass
{
    public ConcurrentBag<LeagueMatch> MatchesConcurrentBag
    {
        get => matchesConcurrentBag.GetValue();
        set => matchesConcurrentBag.SetValue(value);
    }

    [DataMember] private logConcurrentBag<LeagueMatch> matchesConcurrentBag = new logConcurrentBag<LeagueMatch>();

    public List<string> GetClassParameters()
    {
        return new List<string> { matchesConcurrentBag.GetLoggingClassParameters() };
    }

    public async Task CreateAMatch(InterfaceLeague _interfaceLeague, int[] _teamsToFormMatchOn)
    {
        Log.WriteLine("Creating a match with teams ids: " + _teamsToFormMatchOn[0] + " and " +
            _teamsToFormMatchOn[1], LogLevel.VERBOSE);

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return;
        }

        if (_teamsToFormMatchOn.Length != 2)
        {
            Log.WriteLine("Warning! teams Length was not 2!", LogLevel.ERROR);
        }

        LeagueMatch newMatch = new(_interfaceLeague, _teamsToFormMatchOn);

        InterfaceChannel newChannel = await CreateAMatchChannel(newMatch, _interfaceLeague, client);

        MatchesConcurrentBag.Add(newMatch);
        Log.WriteLine("Added match channel id: " + newChannel.ChannelId + " to the MatchesConcurrentBag, count is now: " +
            MatchesConcurrentBag.Count, LogLevel.VERBOSE);

        Thread secondThread = new Thread(() => InitChannelOnSecondThread(client, newChannel, _interfaceLeague.LeagueCategoryId));
        secondThread.Start();
    }

    public async Task<InterfaceChannel> CreateAMatchChannel(
        LeagueMatch _leagueMatch, InterfaceLeague _interfaceLeague, DiscordSocketClient _client)
    {
        try
        {
            // Get the category by the league category name passed in the method
            var categoryKvp =
                Database.Instance.Categories.FindInterfaceCategoryByCategoryName(
                    _interfaceLeague.LeagueCategoryName);

            string leagueMatchIdString = _leagueMatch.MatchId.ToString();

            // Prep the channel name with match id
            string overriddenMatchName = "match-" + leagueMatchIdString;

            Log.WriteLine("Starting to create a new match channel: " +
                overriddenMatchName, LogLevel.VERBOSE);

            // Prepare the match with the ID of the current new match
            InterfaceChannel interfaceChannel = await categoryKvp.CreateSpecificChannelFromChannelTypeWithoutRole(
                    ChannelType.MATCHCHANNEL, categoryKvp.SocketCategoryChannelId,
                    overriddenMatchName, // Override's the channel's name with the match name with that match-[id]
                    _leagueMatch.GetIdsOfThePlayersInTheMatchAsArray(_interfaceLeague));

            _leagueMatch.MatchChannelId = interfaceChannel.ChannelId;

            if (!Database.Instance.Categories.MatchChannelsIdWithCategoryId.ContainsKey(
                interfaceChannel.ChannelId))
            {
                Database.Instance.Categories.MatchChannelsIdWithCategoryId.TryAdd(
                    interfaceChannel.ChannelId, categoryKvp.SocketCategoryChannelId);
            }
            return interfaceChannel;
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            throw new InvalidOperationException(ex.Message);
        }
    }

    public async void InitChannelOnSecondThread(
        DiscordSocketClient _client, InterfaceChannel _interfaceChannel, ulong _leagueCategoryId)
    {
        await _interfaceChannel.PostChannelMessages(_client);

        // Schedule the match queue timeout (if the players don't accept it in the time)
        new MatchQueueAcceptEvent(60, _leagueCategoryId, _interfaceChannel.ChannelId);

        await SerializationManager.SerializeDB();

        Log.WriteLine("DONE CREATING A MATCH CHANNEL!", LogLevel.VERBOSE);
    }

    public LeagueMatch FindLeagueMatchByTheChannelId(ulong _channelId)
    {
        Log.WriteLine("Getting match by channelId: " + _channelId, LogLevel.VERBOSE);

        LeagueMatch? foundMatch = MatchesConcurrentBag.FirstOrDefault(x => x.MatchChannelId == _channelId);
        if (foundMatch == null)
        {
            Log.WriteLine(nameof(foundMatch) + " was null! (user tried to comment after the match has been done)", LogLevel.DEBUG);
            LeagueMatch? foundArchivedMatch = Database.Instance.ArchivedLeagueMatches.FirstOrDefault(
                x => x.MatchChannelId == _channelId);

            if (foundArchivedMatch != null)
            {
                Log.WriteLine("Returning archived match: " + foundArchivedMatch.MatchId, LogLevel.VERBOSE);
                return foundArchivedMatch;
            }

            Log.WriteLine(nameof(foundArchivedMatch) + " was null! (not found on active or archived matches)", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(foundArchivedMatch) + " was null!");
        }

        Log.WriteLine("Found Match: " + foundMatch.MatchId + " with channelId: " +
            _channelId, LogLevel.DEBUG);

        return foundMatch;
    }

    public Task<LeagueMatch?> FindMatchAndRemoveItFromConcurrentBag(InterfaceLeague _interfaceLeague, ulong _matchChannelId)
    {
        Log.WriteLine("Removing: " + _matchChannelId + " on: " + _interfaceLeague.LeagueCategoryName, LogLevel.VERBOSE);

        LeagueMatch tempMatch = null;
        ConcurrentBag<LeagueMatch> updatedMatchesConcurrentBag = new ConcurrentBag<LeagueMatch>();

        while (_interfaceLeague.LeagueData.Matches.MatchesConcurrentBag.TryTake(out LeagueMatch? match))
        {
            if (match.MatchChannelId == _matchChannelId)
            {
                Log.WriteLine("Removed matchId: " + match.MatchId + " on ch: " + _matchChannelId +
                    " on league: " + _interfaceLeague.LeagueCategoryName, LogLevel.VERBOSE);
                tempMatch = match;
            }
            else
            {
                updatedMatchesConcurrentBag.Add(match);
            }
        }

        _interfaceLeague.LeagueData.Matches.MatchesConcurrentBag = updatedMatchesConcurrentBag;

        return Task.FromResult(tempMatch);
    }
}