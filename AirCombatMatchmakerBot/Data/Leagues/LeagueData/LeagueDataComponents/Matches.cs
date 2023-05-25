using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;
using System.Text.RegularExpressions;

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

            _leagueMatch.MatchChannelId = interfaceChannel.ChannelId;

            if (!Database.Instance.Categories.MatchChannelsIdWithCategoryId.ContainsKey(
                interfaceChannel.ChannelId))
            {
                Database.Instance.Categories.MatchChannelsIdWithCategoryId.TryAdd(
                    interfaceChannel.ChannelId, categoryKvp.SocketCategoryChannelId);
            }

            Thread secondThread = new Thread(() => InitChannelOnSecondThread(_client, interfaceChannel));
            secondThread.Start();
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return;
        }
    }

    public void InitChannelOnSecondThread(
        DiscordSocketClient _client, InterfaceChannel _interfaceChannel)
    {
        _interfaceChannel.PostChannelMessages(_client);

        Log.WriteLine("DONE CREATING A MATCH CHANNEL!", LogLevel.VERBOSE);
    }

    public LeagueMatch FindLeagueMatchByTheChannelId(ulong _channelId)
    {
        Log.WriteLine("Getting match by channelId: " + _channelId, LogLevel.VERBOSE);

        LeagueMatch? foundMatch = MatchesConcurrentBag.FirstOrDefault(x => x.MatchChannelId == _channelId);
        if (foundMatch == null) 
        {
            Log.WriteLine(nameof(foundMatch) + " was null!", LogLevel.ERROR);
            throw new InvalidOperationException(nameof(foundMatch) + " was null!");
        }

        Log.WriteLine("Found Match: " + foundMatch.MatchId + " with channelId: " +
            _channelId, LogLevel.VERBOSE);

        return foundMatch;
    }
}