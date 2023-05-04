﻿using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using Discord;
using System.Diagnostics;

[DataContract]
public class LeagueMatch
{
    public ConcurrentDictionary<int, string> TeamsInTheMatch
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamsInTheMatch) + " with count of: " +
                teamsInTheMatch.Count, LogLevel.VERBOSE);
            return teamsInTheMatch;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamsInTheMatch) + teamsInTheMatch
                + " to: " + value, LogLevel.VERBOSE);
            teamsInTheMatch = value;
        }
    }

    public int MatchId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(matchId)
                + ": " + matchId, LogLevel.VERBOSE);
            return matchId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(matchId) +
                matchId + " to: " + value, LogLevel.VERBOSE);
            matchId = value;
        }
    }

    public ulong MatchChannelId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(matchChannelId)
                + ": " + matchChannelId, LogLevel.VERBOSE);
            return matchChannelId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(matchChannelId) +
                matchChannelId + " to: " + value, LogLevel.VERBOSE);
            matchChannelId = value;
        }
    }

    public MatchReporting MatchReporting
    {
        get
        {
            Log.WriteLine("Getting " + nameof(matchReporting)
                + ": " + matchReporting, LogLevel.VERBOSE);
            return matchReporting;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(matchReporting) +
                matchReporting + " to: " + value, LogLevel.VERBOSE);
            matchReporting = value;
        }
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

    [DataMember] private ConcurrentDictionary<int, string> teamsInTheMatch { get; set; }
    [DataMember] private int matchId { get; set; }
    [DataMember] private ulong matchChannelId { get; set; }
    [DataMember] private MatchReporting matchReporting { get; set; }
    [DataMember] private CategoryType matchLeague { get; set; }

    public LeagueMatch()
    {
        teamsInTheMatch = new ConcurrentDictionary<int, string>();
        matchReporting = new MatchReporting();
    }

    public LeagueMatch(InterfaceLeague _interfaceLeague, int[] _teamsToFormMatchOn)
    {
        teamsInTheMatch = new ConcurrentDictionary<int, string>();
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

            teamsInTheMatch.TryAdd(teamId, foundTeam.GetTeamInAString(false, leagueTeamSize));

            Log.WriteLine("Count is now: " + teamsInTheMatch.Count, LogLevel.DEBUG);
        }

        foreach (var item in teamsInTheMatch)
        {
            Log.WriteLine("final teamsInTheMatch: " + item.Key, LogLevel.DEBUG);
        }

        matchId = Database.Instance.Leagues.LeaguesMatchCounter;
        Database.Instance.Leagues.LeaguesMatchCounter++;

        matchReporting = new MatchReporting(teamsInTheMatch);

        Log.WriteLine("Constructed a new match with teams ids: " + teamsInTheMatch.ElementAt(0) +
            teamsInTheMatch.ElementAt(1) + " with matchId of: " + matchId, LogLevel.DEBUG);
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

    public async void FinishTheMatch(InterfaceLeague _interfaceLeague, bool _removingPlayerFromDatabase = false)
    {
        matchReporting.MatchDone = true;

        Log.WriteLine("Finishing match: " + matchId, LogLevel.DEBUG);
        matchReporting.EloSystem.CalculateFinalEloForBothTeams(
            _interfaceLeague, matchReporting.FindTeamsInTheMatch(_interfaceLeague),
            matchReporting.TeamIdsWithReportData);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        if (matchReporting.FinalResultForConfirmation == null)
        {
            if (!_removingPlayerFromDatabase)
            {
                Log.WriteLine("Final result for the confirmation was null!", LogLevel.CRITICAL);
                return;
            }

            Log.WriteLine("Final result for the confirmation was null, but during player removal", LogLevel.DEBUG);

            InterfaceChannel interfaceChannel = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                _interfaceLeague.DiscordLeagueReferences.LeagueCategoryId).Value.FindInterfaceChannelWithIdInTheCategory(
                    matchChannelId);

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

            matchReporting.FinalResultForConfirmation = interfaceMessage.MessageDescription;
            matchReporting.FinalMessageForMatchReportingChannel = matchFinalResultMessage.AlternativeMessage;

            matchReporting.FinalResultTitleForConfirmation = interfaceMessage.MessageEmbedTitle;
        }

        AttachmentData[] attachmentDatas = TacviewManager.FindTacviewAttachmentsForACertainMatch(
            matchId, _interfaceLeague).Result;

        /*
        foreach (var item in attachmentDatas)
        {
            Log.WriteLine("attachmentData: " + item.attachmentName + " | " + item.attachmentLink, LogLevel.DEBUG);
        }*/

        if (matchReporting.FinalMessageForMatchReportingChannel == null)
        {
            Log.WriteLine(nameof(matchReporting) + " FinalMessageForMatchReportingChannel was null!", LogLevel.ERROR);
            return;
        }

        if (matchReporting.FinalResultTitleForConfirmation == null)
        {
            Log.WriteLine(nameof(matchReporting) + " matchReporting.FinalResultTitleForConfirmation was null!", LogLevel.ERROR);
            return;
        }

        await _interfaceLeague.PostMatchReport(
            matchReporting.FinalMessageForMatchReportingChannel, matchReporting.FinalResultTitleForConfirmation, attachmentDatas);

        LeagueMatch? tempMatch = _interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(matchChannelId);

        // Perhaps search within category for a faster operation
        var channel = guild.Channels.FirstOrDefault(
            c => c.Id == matchChannelId &&
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
            _interfaceLeague.DiscordLeagueReferences.LeagueCategoryId).Value.InterfaceChannels.TryRemove(
                matchChannelId, out InterfaceChannel? _ic);
        Database.Instance.Categories.MatchChannelsIdWithCategoryId.TryRemove(matchChannelId, out ulong _id);

        int matchIdTemp = matchId;

        Database.Instance.ArchivedLeagueMatches.Add(tempMatch);
        Log.WriteLine("Added " + matchIdTemp + " to the archive, count is now: " +
            Database.Instance.ArchivedLeagueMatches.Count, LogLevel.DEBUG);

        foreach (var item in _interfaceLeague.LeagueData.Matches.MatchesConcurrentBag.Where(
            m => m.matchId == tempMatch.MatchId))
        {
            _interfaceLeague.LeagueData.Matches.MatchesConcurrentBag.TryTake(out LeagueMatch? _leagueMatch);
            Log.WriteLine("Removed match " + item.MatchId, LogLevel.DEBUG);
        }

        // When removing the player from the database, no need for this because it's done after he is gone from the league
        if (!_removingPlayerFromDatabase)
        {
            _interfaceLeague.UpdateLeagueLeaderboard();
        }

        await SerializationManager.SerializeDB();
    }
}