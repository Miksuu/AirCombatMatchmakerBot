using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class Leagues
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
    [DataMember] private logVar<int> leaguesMatchCounter = new logVar<int>(1);

    public bool CheckIfILeagueExistsByCategoryName(LeagueName _leagueCategoryName)
    {
        bool exists = false;
        Log.WriteLine("Checking if " + _leagueCategoryName + " exists.");
        exists = StoredLeagues.Any(x => x.LeagueCategoryName == _leagueCategoryName);
        Log.WriteLine(_leagueCategoryName + " exists: " + exists);
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
            Log.WriteLine(nameof(FoundLeague) + " was null!", LogLevel.ERROR);
            throw new InvalidOperationException(nameof(FoundLeague) + " was null!");
        }

        Log.WriteLine("Found: " + FoundLeague.LeagueCategoryName);
        return FoundLeague;
    }

    public InterfaceLeague GetILeagueByCategoryId(ulong _channelCategoryId)
    {
        Log.WriteLine("Getting ILeague by ID: " + _channelCategoryId + " with " + nameof(StoredLeagues) +
            " count: " + StoredLeagues.Count, LogLevel.DEBUG);

        InterfaceLeague? FoundLeague = StoredLeagues.FirstOrDefault(
            x => x.LeagueCategoryId == _channelCategoryId);
        if (FoundLeague == null)
        {
            Log.WriteLine(nameof(FoundLeague) + " was null!", LogLevel.ERROR);
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
            Log.WriteLine(nameof(FoundLeague) + " was null!", LogLevel.ERROR);
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
                Log.WriteLine("storedLeague was null!", LogLevel.ERROR);
                continue;
            }

            string? storedLeagueString = storedLeague.ToString();
            if (storedLeagueString == null)
            {
                Log.WriteLine("storedLeagueString was null!", LogLevel.ERROR);
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
                                ) + " deactive in league: " +
                                storedLeague.LeagueCategoryName + " because " + player.PlayerNickName +
                                " left", LogLevel.DEBUG);

                            InterfaceLeague findLeagueCategoryType;
                            InterfaceChannel interfaceChannel;
                            try
                            {
                                findLeagueCategoryType = GetILeagueByString(storedLeagueString);

                                interfaceChannel = Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryByCategoryName(
                                    CategoryType.REGISTRATIONCATEGORY).FindInterfaceChannelWithNameInTheCategory(
                                        ChannelType.LEAGUEREGISTRATION);
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLine(ex.Message, LogLevel.ERROR);
                                return;
                            }

                            var leagueRegistrationMessages = interfaceChannel.InterfaceMessagesWithIds.Where(
                                m => m.Value.MessageName == MessageName.LEAGUEREGISTRATIONMESSAGE);

                            foreach (var messageKvp in leagueRegistrationMessages)
                            {
                                messageKvp.Value.GenerateAndModifyTheMessageAsync();
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

    public void RemovePlayersFromQueuesOnceMatchIsCloseEnough(List<ulong> _playerIds, InterfaceLeague _interfaceLeague)
    {
        List<InterfaceMessage> toBeModifiedMessages = new List<InterfaceMessage>();

        foreach (var playerId in _playerIds)
        {
            Log.WriteLine("on: " + playerId);
            foreach (var league in StoredLeagues)
            {
                Log.WriteLine("on: " + league.LeagueCategoryName);
                if (!league.LeagueData.CheckIfPlayerIsParcipiatingInTheLeague(playerId))
                {
                    continue;
                }

                league.LeagueData.ChallengeStatus.RemoveChallengeFromThisLeague(playerId, _interfaceLeague);

                InterfaceMessage interfaceMessage = Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryWithCategoryId(
                    league.LeagueCategoryId).FindInterfaceChannelWithNameInTheCategory(
                        ChannelType.CHALLENGE).FindInterfaceMessageWithNameInTheChannel(
                            MessageName.CHALLENGEMESSAGE);

                if (!toBeModifiedMessages.Any(x => x.MessageId == interfaceMessage.MessageId))
                {
                    toBeModifiedMessages.Add(interfaceMessage);
                }
            }
        }

        foreach (var message in toBeModifiedMessages)
        {
            message.GenerateAndModifyTheMessageAsync();
        }
    }

    const ulong earliestOrLatestAllowedTimeBeforeAndAfterTheMatch = 1800;

    public string GetListOfTimesThatWontBeSuitableForScheduling(LeagueMatch _leagueMatch, InterfaceLeague _interfaceLeague)
    {
        string finalList = string.Empty;
        List<ulong> matchTimes = new List<ulong>();

        var listOfPlayers = _leagueMatch.GetIdsOfThePlayersInTheMatchAsArray(_interfaceLeague).ToList();

        var listOfLeagueMatches = CheckAndReturnTheListOfMatchesThatListPlayersAreIn(
            listOfPlayers, TimeService.GetCurrentUnixTime(), _interfaceLeague);

        List<int> alreadyLoopedThroughMatches = new List<int>();
        foreach (LeagueMatch leagueMatch in listOfLeagueMatches)
        {
            if (alreadyLoopedThroughMatches.Contains(leagueMatch.MatchId))
            {
                continue;
            }
            alreadyLoopedThroughMatches.Add(leagueMatch.MatchId);

            if (leagueMatch.MatchState != MatchState.PLAYERREADYCONFIRMATIONPHASE)
            {
                continue;
            }

            var matchTime = leagueMatch.MatchEventManager.GetTimeOfEventOfType(typeof(MatchQueueAcceptEvent));
            matchTimes.Add(matchTime);
        }

        var orderedMatchTimes = matchTimes.OrderBy(time => time);
        //var currentTime = TimeService.GetCurrentUnixTime();

        if (matchTimes.Count <= 0)
        {
            return "";
        }

        finalList += "\n\nYou can not schedule to these times (existing matches):";

        // Add before for the matches that's scheduled time is inside the currentTime - earliestOrLatestAllowedTimeBeforeAndAfterTheMatch
        foreach (var matchTime in orderedMatchTimes)
        {
            finalList += "\n**" + TimeService.ConvertToZuluTimeFromUnixTime(matchTime - earliestOrLatestAllowedTimeBeforeAndAfterTheMatch) + " - " +
                TimeService.ConvertToZuluTimeFromUnixTime(matchTime + earliestOrLatestAllowedTimeBeforeAndAfterTheMatch) + "**";
        }

        return finalList;
    }

    public List<LeagueMatch> GetListOfMatchesClose(List<LeagueMatch> _listOfLeagueMatches, ulong _timeOffset = 0)
    {
        var listOfMatchesClose = _listOfLeagueMatches.Where(
            x => x.MatchState == MatchState.PLAYERREADYCONFIRMATIONPHASE &&
            (x.MatchEventManager.GetTimeUntilEventOfType(typeof(MatchQueueAcceptEvent))
            <= earliestOrLatestAllowedTimeBeforeAndAfterTheMatch + _timeOffset)).ToList();

        return listOfMatchesClose;
    }

    private List<LeagueMatch> GetListOfMatchesAfter(List<LeagueMatch> _listOfLeagueMatches)
    {
        var listOfMatchesAfter = _listOfLeagueMatches.Where(
            x => x.MatchState == MatchState.PLAYERREADYCONFIRMATIONPHASE &&
            (x.MatchEventManager.GetTimeUntilEventOfType(typeof(MatchQueueAcceptEvent))
            <= earliestOrLatestAllowedTimeBeforeAndAfterTheMatch)).ToList();

        return listOfMatchesAfter;
    }

    public async Task<Response> CheckIfListOfPlayersCanJoinOrSuggestATimeForTheMatchWithTime(
        List<ulong> _playerIds, ulong _suggestedTime, ulong _suggestedByPlayerId, InterfaceLeague _interfaceLeague)
    {
        Log.WriteLine("Checking with " + _playerIds.Count);
        var listOfLeagueMatches = CheckAndReturnTheListOfMatchesThatListPlayersAreIn(
            _playerIds, _suggestedTime, _interfaceLeague);

        List<LeagueMatch> listOfMatchesCombined =
            GetListOfMatchesClose(listOfLeagueMatches).Concat(
            GetListOfMatchesAfter(listOfLeagueMatches)).ToList();

        Log.WriteLine("list: " + listOfMatchesCombined.Count);

        if (listOfMatchesCombined.Count <= 0)
        {
            return new Response("", true);
        }

        // Add some bool to determine if "Can not join queue" or "Can not schedule"
        string stringOfMatches = string.Empty;

        List<int> alreadyLoopedThroughMatchIds = new List<int>();
        foreach (LeagueMatch leagueMatch in listOfMatchesCombined)
        {
            if (alreadyLoopedThroughMatchIds.Contains(leagueMatch.MatchId))
            {
                continue;
            }
            alreadyLoopedThroughMatchIds.Add(leagueMatch.MatchId);

            ulong matchUnixTime = leagueMatch.MatchEventManager.GetEventByType(
                typeof(MatchQueueAcceptEvent)).TimeToExecuteTheEventOn;

            var playerArray = leagueMatch.GetIdsOfThePlayersInTheMatchAsArray(_interfaceLeague);
            if (playerArray.Contains(_suggestedByPlayerId))
            {
                stringOfMatches += "Your match " + await Database.GetInstance<DiscordBotDatabase>().Categories.GetMessageJumpUrl(
                    _interfaceLeague.LeagueCategoryId, leagueMatch.MatchChannelId, MessageName.MATCHSTARTMESSAGE) + " is at " +
                    TimeService.ConvertToZuluTimeFromUnixTime(matchUnixTime) + " in: " +
                    TimeService.ReturnTimeLeftAsStringFromTheTimeTheActionWillTakePlace(matchUnixTime) + ".";
            }
            else
            {
                // Add specific player here later instead of the "other player"
                stringOfMatches += "The other player has a match " + leagueMatch.MatchId + " at " +
                    TimeService.ConvertToZuluTimeFromUnixTime(matchUnixTime) + " in: " +
                    TimeService.ReturnTimeLeftAsStringFromTheTimeTheActionWillTakePlace(matchUnixTime) + ".";
            }

            // Add suggestions to schedule at earlier/later time
            // Perhaps replace with just earliest/latest time.
            if ((leagueMatch.MatchEventManager.GetTimeOfEventOfType(typeof(MatchQueueAcceptEvent)) - _suggestedTime)
                 <= earliestOrLatestAllowedTimeBeforeAndAfterTheMatch)
            {
                stringOfMatches += " Matches must take place " + TimeService.ReturnTimeLeftAsStringFromTheTimeTheActionWillTakePlaceWithTimeLeft(
                    earliestOrLatestAllowedTimeBeforeAndAfterTheMatch) + " before the scheduled time.";
            }
            else if ((_suggestedTime - leagueMatch.MatchEventManager.GetEventByType(
                typeof(MatchQueueAcceptEvent)).TimeToExecuteTheEventOn) <= earliestOrLatestAllowedTimeBeforeAndAfterTheMatch)
            {
                stringOfMatches += " Matches must take place " + TimeService.ReturnTimeLeftAsStringFromTheTimeTheActionWillTakePlaceWithTimeLeft(
                    earliestOrLatestAllowedTimeBeforeAndAfterTheMatch) + " after the scheduled time.";
            }
        }

        return new Response(stringOfMatches, false);
    }

    public List<LeagueMatch> CheckAndReturnTheListOfMatchesThatListPlayersAreIn(
        List<ulong> _playersIds, ulong _suggestedTime, InterfaceLeague _interfaceLeague)
    {
        try
        {
            // Temp, replace with logList when done
            List<LeagueMatch> matchesThatThePlayersAreIn = new List<LeagueMatch>();

            //Log.WriteLine("on: " + _match.MatchId);

            //var players = _match.GetIdsOfThePlayersInTheMatchAsArray().ToList();

            Log.WriteLine("Players count: " + _playersIds.Count);

            foreach (var playerId in _playersIds)
            {
                Log.WriteLine("on: " + playerId);
                foreach (var league in StoredLeagues)
                {
                    Log.WriteLine("on: " + league.LeagueCategoryName);
                    if (!league.LeagueData.CheckIfPlayerIsParcipiatingInTheLeague(playerId))
                    {
                        continue;
                    }

                    foreach (LeagueMatch leagueMatch in league.LeagueData.Matches.MatchesConcurrentBag)
                    {
                        Log.WriteLine("on match: " + leagueMatch.MatchId + " with state: " + leagueMatch.MatchState);
                        //if (leagueMatch.MatchId == _match.MatchId)
                        //{
                        //    Log.WriteLine(leagueMatch.MatchId + "is the same match!");
                        //    continue;
                        //}

                        if (leagueMatch.MatchState != MatchState.PLAYERREADYCONFIRMATIONPHASE)
                        {
                            Log.WriteLine("State was not correct one, continuing");
                            continue;
                        }

                        Log.WriteLine("State: " + leagueMatch.MatchState);

                        var playersOnTheLeagueMatch = leagueMatch.GetIdsOfThePlayersInTheMatchAsArray(_interfaceLeague).ToList();
                        if (!playersOnTheLeagueMatch.Contains(playerId))
                        {
                            Log.WriteLine(leagueMatch.MatchId + " does not contain: " + playerId);
                            continue;
                        }

                        Log.WriteLine(leagueMatch.MatchId + " contains: " + playerId);


                        //if (matchQueueAcceptEvent.TimeToExecuteTheEventOn - _suggestedTime > 900)
                        //{
                        //    Log.WriteLine("Match's " + leagueMatch.MatchId + " scheduled time: " +
                        //        matchQueueAcceptEvent.TimeToExecuteTheEventOn + " is fine with suggested time: " + _suggestedTime);
                        //    continue;
                        //}

                        Log.WriteLine("Match's " + leagueMatch.MatchId + " scheduled time: " + leagueMatch.MatchEventManager.GetTimeOfEventOfType(typeof(MatchQueueAcceptEvent)) +
                            " was not fine with suggested time: " + _suggestedTime, LogLevel.DEBUG);

                        matchesThatThePlayersAreIn.Add(leagueMatch);
                    }
                }
            }

            // Temp, replace with logList when done
            foreach (var item in matchesThatThePlayersAreIn)
            {
                Log.WriteLine(item.MatchId + "'s time was not fine with: " +// _match.MatchId +
                    " with time: " + _suggestedTime, LogLevel.DEBUG);
            }

            return matchesThatThePlayersAreIn;
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message);
            throw;
        }
    }
}