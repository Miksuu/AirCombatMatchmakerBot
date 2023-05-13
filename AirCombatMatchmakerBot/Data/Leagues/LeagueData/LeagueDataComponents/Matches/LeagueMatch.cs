using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using Discord;
using System.Diagnostics;

[DataContract]
public class LeagueMatch : logClass<LeagueMatch>, InterfaceLoggableClass
{
    public ConcurrentDictionary<int, string> TeamsInTheMatch
    {
        get => teamsInTheMatch.GetValue();
        set => teamsInTheMatch.SetValue(value);
    }

    public int MatchId
    {
        get => matchId.GetValue();
        set => matchId.SetValue(value);
    }

    public ulong MatchChannelId
    {
        get => matchChannelId.GetValue();
        set => matchChannelId.SetValue(value);
    }

    public MatchReporting MatchReporting
    {
        get => matchReporting.GetValue();
        set => matchReporting.SetValue(value);
    }

    public CategoryType MatchLeague
    {
        get
        {
            Log.WriteLine("Getting " + nameof(matchLeague)
                + ": " + matchLeague, LogLevel.VERBOSE);
            return matchLeague;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(matchLeague) +
                matchLeague + " to: " + value, LogLevel.VERBOSE);
            matchLeague = value;
        }
    }

    [DataMember] private logConcurrentDictionary<int, string> teamsInTheMatch = new logConcurrentDictionary<int, string>();
    [DataMember] private logClass<int> matchId = new logClass<int>();
    [DataMember] private logClass<ulong> matchChannelId = new logClass<ulong>();
    [DataMember] private logClass<MatchReporting> matchReporting = new logClass<MatchReporting>(new MatchReporting());
    [DataMember] private CategoryType matchLeague { get; set; }

    public List<string> GetClassParameters()
    {
        return new List<string> { teamsInTheMatch.GetLoggingClassParameters<int, string>(), matchId.GetParameter(),
            matchChannelId.GetParameter(), matchReporting.GetParameter(), matchLeague.ToString() };
    }

    public LeagueMatch() { }

    public LeagueMatch(InterfaceLeague _interfaceLeague, int[] _teamsToFormMatchOn)
    {
        TeamsInTheMatch = new ConcurrentDictionary<int, string>();
        int leagueTeamSize = _interfaceLeague.LeaguePlayerCountPerTeam;
        matchLeague = _interfaceLeague.LeagueCategoryName;

        Log.WriteLine("Teams to from the match on: " + _teamsToFormMatchOn[0] +
            ", " + _teamsToFormMatchOn[1], LogLevel.DEBUG);

        // Add the team's name to the ConcurrentDictionary as a value
        foreach (int teamId in _teamsToFormMatchOn)
        {
            Team foundTeam =
                _interfaceLeague.LeagueData.Teams.FindTeamById(
                    leagueTeamSize, teamId);

            Log.WriteLine("Found team: " + foundTeam.TeamId, LogLevel.DEBUG);

            TeamsInTheMatch.TryAdd(teamId, foundTeam.GetTeamInAString(false, leagueTeamSize));

            Log.WriteLine("Count is now: " + TeamsInTheMatch.Count, LogLevel.DEBUG);
        }

        foreach (var item in TeamsInTheMatch)
        {
            Log.WriteLine("final teamsInTheMatch: " + item.Key, LogLevel.DEBUG);
        }

        MatchId = Database.Instance.Leagues.LeaguesMatchCounter;
        Database.Instance.Leagues.LeaguesMatchCounter++;

        MatchReporting = new MatchReporting(TeamsInTheMatch);

        Log.WriteLine("Constructed a new match with teams ids: " + TeamsInTheMatch.ElementAt(0) +
            TeamsInTheMatch.ElementAt(1) + " with matchId of: " + MatchId, LogLevel.DEBUG);
    }

    public ulong[] GetIdsOfThePlayersInTheMatchAsArray(InterfaceLeague _interfaceLeague)
    {
        int playerCounter = 0;

        // Calculate how many users need to be granted roles
        int userAmountToGrantRolesTo = _interfaceLeague.LeaguePlayerCountPerTeam * 2;
        ulong[] allowedUserIds = new ulong[userAmountToGrantRolesTo];

        Log.WriteLine(nameof(allowedUserIds) + " length: " +
            allowedUserIds.Length, LogLevel.VERBOSE);

        foreach (var teamKvp in TeamsInTheMatch)
        {
            Log.WriteLine("Looping on team id: " + teamKvp.Key, LogLevel.VERBOSE);
            Team foundTeam = _interfaceLeague.LeagueData.Teams.FindTeamById(
                _interfaceLeague.LeaguePlayerCountPerTeam, teamKvp.Key);

            if (foundTeam == null)
            {
                Log.WriteLine(nameof(foundTeam) + " was null!", LogLevel.ERROR);
                continue;
            }

            foreach (Player player in foundTeam.Players)
            {
                allowedUserIds[playerCounter] = player.PlayerDiscordId;
                Log.WriteLine("Added " + allowedUserIds[playerCounter] + " to: " +
                    nameof(allowedUserIds) + ". " + nameof(playerCounter) + " is now: " +
                    playerCounter + 1 + " out of: " + (allowedUserIds.Length - 1).ToString(), LogLevel.VERBOSE);

                playerCounter++;
            }
        }

        return allowedUserIds;
    }

    public async void FinishTheMatch(InterfaceLeague _interfaceLeague)
    {
        MatchReporting.MatchDone = true;

        Log.WriteLine("Finishing match: " + MatchId, LogLevel.DEBUG);
        EloSystem.CalculateFinalEloForBothTeams(
            _interfaceLeague, MatchReporting.FindTeamsInTheMatch(_interfaceLeague),
            MatchReporting.TeamIdsWithReportData);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        /*
        if (!_removingPlayerFromDatabase)
        {
            Log.WriteLine("Final result for the confirmation was null!", LogLevel.CRITICAL);
            return;
        }*/

        Log.WriteLine("Final result for the confirmation was null, but during player removal", LogLevel.DEBUG);

        InterfaceChannel interfaceChannel = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            _interfaceLeague.LeagueCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                MatchChannelId);

        var interfaceMessage = await interfaceChannel.CreateAMessageForTheChannelFromMessageName(
                MessageName.MATCHFINALRESULTMESSAGE, false);
        if (interfaceMessage == null)
        {
            Log.WriteLine(nameof(interfaceMessage) + " was null!", LogLevel.ERROR);
            return;
        }

        var matchFinalResultMessage = interfaceMessage as MATCHFINALRESULTMESSAGE;
        if (matchFinalResultMessage == null)
        {
            Log.WriteLine(nameof(matchFinalResultMessage) + " was null!", LogLevel.ERROR);
            return;
        }

        Log.WriteLine("altMsg: " + matchFinalResultMessage.AlternativeMessage, LogLevel.DEBUG);

        MatchReporting.FinalResultForConfirmation = interfaceMessage.MessageDescription;
        MatchReporting.FinalMessageForMatchReportingChannel = matchFinalResultMessage.AlternativeMessage;
        MatchReporting.FinalResultTitleForConfirmation = interfaceMessage.MessageEmbedTitle;

        AttachmentData[] attachmentDatas = TacviewManager.FindTacviewAttachmentsForACertainMatch(
            MatchId, _interfaceLeague).Result;

        /*
        foreach (var item in attachmentDatas)
        {
            Log.WriteLine("attachmentData: " + item.attachmentName + " | " + item.attachmentLink, LogLevel.DEBUG);
        }*/

        if (MatchReporting.FinalMessageForMatchReportingChannel == null)
        {
            Log.WriteLine(nameof(MatchReporting) + " FinalMessageForMatchReportingChannel was null!", LogLevel.ERROR);
            return;
        }

        if (MatchReporting.FinalResultTitleForConfirmation == null)
        {
            Log.WriteLine(nameof(MatchReporting) + " matchReporting.FinalResultTitleForConfirmation was null!", LogLevel.ERROR);
            return;
        }

        Log.WriteLine("finalMsg: " + MatchReporting.FinalMessageForMatchReportingChannel, LogLevel.DEBUG);

        await _interfaceLeague.PostMatchReport(
            MatchReporting.FinalMessageForMatchReportingChannel, MatchReporting.FinalResultTitleForConfirmation, attachmentDatas);

        LeagueMatch? tempMatch = _interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(MatchChannelId);

        // Perhaps search within category for a faster operation
        var channel = guild.Channels.FirstOrDefault(
            c => c.Id == MatchChannelId &&
                c.Name.Contains("match"));// Just in case

        if (channel == null)
        {
            Log.WriteLine(nameof(channel) + " was null!", LogLevel.ERROR);
            return;
        }

        await channel.DeleteAsync();

        if (tempMatch == null)
        {
            Log.WriteLine(nameof(tempMatch) + " was null!", LogLevel.ERROR);
            return;
        }

        Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            _interfaceLeague.LeagueCategoryId).Value.InterfaceChannels.TryRemove(
                MatchChannelId, out InterfaceChannel? _ic);
        Database.Instance.Categories.MatchChannelsIdWithCategoryId.TryRemove(MatchChannelId, out ulong _id);

        int matchIdTemp = MatchId;

        Database.Instance.ArchivedLeagueMatches.Add(tempMatch);
        Log.WriteLine("Added " + matchIdTemp + " to the archive, count is now: " +
            Database.Instance.ArchivedLeagueMatches.Count, LogLevel.DEBUG);

        foreach (var item in _interfaceLeague.LeagueData.Matches.MatchesConcurrentBag.Where(
            m => m.MatchId == tempMatch.MatchId))
        {
            _interfaceLeague.LeagueData.Matches.MatchesConcurrentBag.TryTake(out LeagueMatch? _leagueMatch);
            Log.WriteLine("Removed match " + item.MatchId, LogLevel.DEBUG);
        }

        // When removing the player from the database, no need for this because it's done after he is gone from the league
        //if (!_removingPlayerFromDatabase)
        //{
            _interfaceLeague.UpdateLeagueLeaderboard();
        //}

        await SerializationManager.SerializeDB();
    }
}