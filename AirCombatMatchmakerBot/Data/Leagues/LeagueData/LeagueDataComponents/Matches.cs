using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

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

    public Task CreateAMatch(InterfaceLeague _interfaceLeague, int[] _teamsToFormMatchOn)
    {
        Log.WriteLine("Creating a match with teams ids: " + _teamsToFormMatchOn[0] + " and " +
            _teamsToFormMatchOn[1], LogLevel.VERBOSE);

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return Task.CompletedTask;
        }

        if (_teamsToFormMatchOn.Length != 2)
        {
            Log.WriteLine("Warning! teams Length was not 2!", LogLevel.ERROR);
        }

        LeagueMatch newMatch = new(_interfaceLeague, _teamsToFormMatchOn);
        MatchesConcurrentBag.Add(newMatch);
        Log.WriteLine("Added to the " + nameof(MatchesConcurrentBag) + " count is now: " +
            MatchesConcurrentBag.Count, LogLevel.VERBOSE);

        CreateAMatchChannel(newMatch, _interfaceLeague, client);

        return Task.CompletedTask;
    }

    public void CreateAMatchChannel(
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
            InterfaceChannel interfaceChannel = categoryKvp.CreateSpecificChannelFromChannelTypeWithoutRole(
                    ChannelType.MATCHCHANNEL, categoryKvp.SocketCategoryChannelId,
                    overriddenMatchName, // Override's the channel's name with the match name with that match-[id]
                    _leagueMatch.GetIdsOfThePlayersInTheMatchAsArray(_interfaceLeague)).Result;

            // Schedule the match queue timeout (if the players don't accept it in the time)
            var newEvent = new MatchQueueAcceptEvent(60, _interfaceLeague.LeagueCategoryId, interfaceChannel.ChannelId);
            Database.Instance.EventScheduler.ScheduledEvents.Add(newEvent);

            _leagueMatch.MatchChannelId = interfaceChannel.ChannelId;

            if (!Database.Instance.Categories.MatchChannelsIdWithCategoryId.ContainsKey(
                interfaceChannel.ChannelId))
            {
                Database.Instance.Categories.MatchChannelsIdWithCategoryId.TryAdd(
                    interfaceChannel.ChannelId, categoryKvp.SocketCategoryChannelId);
            }

            Thread secondThread = new Thread(() => InitChannelOnSecondThread(_client, interfaceChannel, newEvent));
            secondThread.Start();
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return;
        }
    }

    public async void InitChannelOnSecondThread(
        DiscordSocketClient _client, InterfaceChannel _interfaceChannel, MatchQueueAcceptEvent _createdMatchQueueAcceptEvent)
    {
        await _interfaceChannel.PostChannelMessages(_client);

        // The event can be checked only after the messages have been posted
        _createdMatchQueueAcceptEvent.ChannelIsReady = true;

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

    public LeagueMatch FindMatchAndRemoveItFromConcurrentBag(InterfaceLeague _interfaceLeague, ulong _matchChannelId)
    {
        Log.WriteLine("Removing: " + _matchChannelId + " on: " + _interfaceLeague.LeagueCategoryName,LogLevel.VERBOSE);

        LeagueMatch tempMatch = _interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(_matchChannelId);

        foreach (var item in _interfaceLeague.LeagueData.Matches.MatchesConcurrentBag.Where(
            m => m.MatchId == tempMatch.MatchId))
        {
            _interfaceLeague.LeagueData.Matches.MatchesConcurrentBag.TryTake(out LeagueMatch? _leagueMatch);
            Log.WriteLine("Removed match " + item.MatchId, LogLevel.DEBUG);
        }

        Log.WriteLine("Removed matchId: " + tempMatch.MatchId + " on ch: " + _matchChannelId +
            " on league: " + _interfaceLeague.LeagueCategoryName, LogLevel.VERBOSE);

        return tempMatch;
    }
}