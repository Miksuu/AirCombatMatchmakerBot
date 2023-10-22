using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class Matches
{
    public ConcurrentBag<LeagueMatch> MatchesConcurrentBag
    {
        get => matchesConcurrentBag.GetValue();
        set => matchesConcurrentBag.SetValue(value);
    }

    [DataMember] private logConcurrentBag<LeagueMatch> matchesConcurrentBag = new logConcurrentBag<LeagueMatch>();

    public Matches() { }

    public async Task CreateAMatch(
        int[] _teamsToFormMatchOn, MatchState _matchState, InterfaceLeague _interfaceLeague,
        bool _attemptToPutTeamsBackToQueueAfterTheMatch = false)
    {
        Log.WriteLine("Creating a match with teams ids: " + _teamsToFormMatchOn[0] + " and " +
            _teamsToFormMatchOn[1]);

        if (_teamsToFormMatchOn.Length != 2)
        {
            Log.WriteLine("Warning! teams Length was not 2!", LogLevel.ERROR);
        }

        LeagueMatch newMatch = new(
            _interfaceLeague, _teamsToFormMatchOn, _matchState, _attemptToPutTeamsBackToQueueAfterTheMatch);

        MatchesConcurrentBag.Add(newMatch);
        Log.WriteLine("Added match channel id: " + newMatch.MatchId + " to the MatchesConcurrentBag, count is now: " +
            MatchesConcurrentBag.Count);

        //if (_matchScheduler != null)
        //{
        //    while (_matchScheduler.AddedTeamsToTheMatches.TryTake(out int element) &&
        //        (!element.Equals(_teamsToFormMatchOn.ElementAt(0)) && !element.Equals(_teamsToFormMatchOn.ElementAt(1))))
        //    {
        //        _matchScheduler.AddedTeamsToTheMatches.Add(element);
        //    }
        //}

        InterfaceChannel newChannel = await CreateAMatchChannel(newMatch, _matchState, _interfaceLeague);

        Thread secondThread = new Thread(() => InitChannelOnSecondThread(newMatch, newChannel));
        secondThread.Start();
    }

    public async Task<InterfaceChannel> CreateAMatchChannel(
        LeagueMatch _leagueMatch, MatchState _matchState, InterfaceLeague _interfaceLeague)
    {
        try
        {
            // Get the category by the league category name passed in the method
            var categoryKvp =
                Database.GetInstance<ApplicationDatabase>().Leagues.GetILeagueByCategoryName(_leagueMatch.MatchLeague);

            string leagueMatchIdString = _leagueMatch.MatchId.ToString();

            // Prep the channel name with match id
            string overriddenMatchName = "match-" + leagueMatchIdString;

            Log.WriteLine("Starting to create a new match channel: " +
                overriddenMatchName);

            var dbRegularCategory = Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryWithCategoryId(categoryKvp.LeagueCategoryId);

            // Prepare the match with the ID of the current new match
            InterfaceChannel interfaceChannel = await dbRegularCategory.CreateSpecificChannelFromChannelTypeWithoutRole(
                    ChannelType.MATCHCHANNEL, categoryKvp.LeagueCategoryId,
                    overriddenMatchName, // Override's the channel's name with the match name with that match-[id]
                    _leagueMatch.GetIdsOfThePlayersInTheMatchAsArray(_interfaceLeague));

            _leagueMatch.MatchChannelId = interfaceChannel.ChannelId;

            if (!Database.GetInstance<ApplicationDatabase>().MatchChannelsIdWithCategoryId.ContainsKey(
                interfaceChannel.ChannelId))
            {
                Database.GetInstance<ApplicationDatabase>().MatchChannelsIdWithCategoryId.TryAdd(
                    interfaceChannel.ChannelId, categoryKvp.LeagueCategoryId);
            }

            // Override the default constructor before posting the message if the channel is scheduled one
            // TODO: Perhaps create proper base class out of this one
            if (_matchState == MatchState.SCHEDULINGPHASE)
            {
                interfaceChannel.ChannelMessages = new ConcurrentDictionary<MessageName, bool>(
                    new ConcurrentBag<KeyValuePair<MessageName, bool>>()
                    {
                        new KeyValuePair<MessageName, bool>(MessageName.MATCHSTARTMESSAGE, false),
                        new KeyValuePair<MessageName, bool>(MessageName.MATCHSCHEDULINGMESSAGE, false),
                    });
            }

            // Schedule the match queue timeout (if the players don't accept it in the time), only if the match is in the
            if (_leagueMatch.MatchState == MatchState.PLAYERREADYCONFIRMATIONPHASE)
            {
                Log.WriteLine(_leagueMatch.MatchEventManager.ClassScheduledEvents.Count.ToString());

                new MatchQueueAcceptEvent(
                    30, _interfaceLeague.LeagueCategoryId,
                    interfaceChannel.ChannelId, _leagueMatch.MatchEventManager.ClassScheduledEvents);
            }

            return interfaceChannel;
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            throw new InvalidOperationException(ex.Message);
        }
    }

    public async void InitChannelOnSecondThread(LeagueMatch _leagueMatch, InterfaceChannel _interfaceChannel)
    {
        await _interfaceChannel.PostChannelMessages();

        await SerializationManager.SerializeDB();

        Log.WriteLine("DONE CREATING A MATCH CHANNEL!");
    }

    public LeagueMatch FindLeagueMatchByTheChannelId(ulong _channelId)
    {
        Log.WriteLine("Getting match by channelId: " + _channelId);

        LeagueMatch? foundMatch = MatchesConcurrentBag.FirstOrDefault(x => x.MatchChannelId == _channelId);
        if (foundMatch == null)
        {
            Log.WriteLine(nameof(foundMatch) + " was null! (user tried to comment after the match has been done)", LogLevel.DEBUG);
            LeagueMatch? foundArchivedMatch = Database.GetInstance<ApplicationDatabase>().ArchivedLeagueMatches.FirstOrDefault(
                x => x.MatchChannelId == _channelId);

            if (foundArchivedMatch != null)
            {
                Log.WriteLine("Returning archived match: " + foundArchivedMatch.MatchId);
                return foundArchivedMatch;
            }

            Log.WriteLine(nameof(foundArchivedMatch) + " was null! (not found on active or archived matches)", LogLevel.ERROR);
            throw new InvalidOperationException(nameof(foundArchivedMatch) + " was null!");
        }

        Log.WriteLine("Found Match: " + foundMatch.MatchId + " with channelId: " +
            _channelId, LogLevel.DEBUG);

        return foundMatch;
    }

    public Task<LeagueMatch?> FindMatchAndRemoveItFromConcurrentBag(ulong _matchChannelId, InterfaceLeague _interfaceLeague)
    {
        Log.WriteLine("Removing: " + _matchChannelId + " on: " + _interfaceLeague.LeagueCategoryName);

        LeagueMatch tempMatch = null;
        ConcurrentBag<LeagueMatch> updatedMatchesConcurrentBag = new ConcurrentBag<LeagueMatch>();

        while (_interfaceLeague.LeagueData.Matches.MatchesConcurrentBag.TryTake(out LeagueMatch? match))
        {
            if (match.MatchChannelId == _matchChannelId)
            {
                Log.WriteLine("Removed matchId: " + match.MatchId + " on ch: " + _matchChannelId +
                    " on league: " + _interfaceLeague.LeagueCategoryName);
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