using Discord;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

[DataContract]
public class Database
{
    public PlayerData PlayerData
    {
        get => playerData.GetValue();
        set => playerData.SetValue(value);
    }
    public Admins Admins
    {
        get => admins.GetValue();
        set => admins.SetValue(value);
    }
    public CachedUsers CachedUsers
    {
        get => cachedUsers.GetValue();
        set => cachedUsers.SetValue(value);
    }
    public Categories Categories
    {
        get => categories.GetValue();
        set => categories.SetValue(value);
    }
    public Leagues Leagues
    {
        get => leagues.GetValue();
        set => leagues.SetValue(value);
    }
    public EventScheduler EventScheduler
    {
        get => eventScheduler.GetValue();
        set => eventScheduler.SetValue(value);
    }

    public ConcurrentBag<LeagueMatch> ArchivedLeagueMatches
    {
        get => archivedLeagueMatches.GetValue();
        set => archivedLeagueMatches.SetValue(value);
    }

    public static Database Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new Database();
                }
                return instance;
            }
        }
        set
        {
            instance = value;
        }
    }

    // Singleton stuff
    private static Database? instance;
    private static readonly object padlock = new object();

    // The Database components
    [DataMember] private logVar<PlayerData> playerData = new logVar<PlayerData>(new PlayerData());
    [DataMember] private logVar<Admins> admins = new logVar<Admins>(new Admins());
    [DataMember] private logVar<CachedUsers> cachedUsers = new logVar<CachedUsers>(new CachedUsers());
    [DataMember] private logVar<Categories> categories = new logVar<Categories>(new Categories());
    [DataMember] private logVar<Leagues> leagues = new logVar<Leagues>(new Leagues());
    [DataMember] private logVar<EventScheduler> eventScheduler = new logVar<EventScheduler>(new EventScheduler());
    [DataMember] private logConcurrentBag<LeagueMatch> archivedLeagueMatches = new logConcurrentBag<LeagueMatch>();

    public async Task RemovePlayerFromTheDatabase(ulong _playerDiscordId)
    {
        Log.WriteLine("Removing player: " + _playerDiscordId + " from the database.", LogLevel.DEBUG);

        await PlayerData.DeletePlayerProfile(_playerDiscordId);
        CachedUsers.RemoveUserFromTheCachedConcurrentBag(_playerDiscordId);

        InterfaceChannel interfaceChannel;
        try
        {
            interfaceChannel = Categories.FindInterfaceCategoryByCategoryName(
                CategoryType.REGISTRATIONCATEGORY).FindInterfaceChannelWithNameInTheCategory(
                    ChannelType.LEAGUEREGISTRATION);
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("leagues count: " + Leagues.StoredLeagues.Count, LogLevel.VERBOSE);

        foreach (InterfaceLeague interfaceLeague in Leagues.StoredLeagues)
        {
            Log.WriteLine("Starting to process league: " + interfaceLeague.LeagueCategoryName, LogLevel.DEBUG);

            // Place the teams to remove 
            List<int> teamsToRemove = new List<int>();
            foreach (Team team in interfaceLeague.LeagueData.Teams.TeamsConcurrentBag)
            {
                Log.WriteLine("Looping through team: " + team.TeamName + "(" + team.TeamId + ")", LogLevel.VERBOSE);
                if (team.Players.Any(p => p.PlayerDiscordId == _playerDiscordId))
                {
                    Log.WriteLine("Player " + +_playerDiscordId + " is in team: " +
                        team.TeamName + "(" + team.TeamId + ")", LogLevel.VERBOSE);

                    teamsToRemove.Add(team.TeamId);

                }
                Log.WriteLine("done looping through teams: " + interfaceLeague.LeagueData.Teams.TeamsConcurrentBag.Count, LogLevel.VERBOSE);
            }

            foreach (int teamId in teamsToRemove)
            {
                while (interfaceLeague.LeagueData.ChallengeStatus.TeamsInTheQueue.TryTake(out int element) && !element.Equals(teamId))
                {
                    interfaceLeague.LeagueData.ChallengeStatus.TeamsInTheQueue.Add(element);
                }

                foreach (LeagueMatch match in interfaceLeague.LeagueData.Matches.MatchesConcurrentBag)
                {
                    if (match.TeamsInTheMatch.ContainsKey(teamId))
                    {
                        Log.WriteLine("Match: " + match.MatchId + " contains: " + teamId +
                            " which has player: " + _playerDiscordId, LogLevel.DEBUG);

                        EloSystem.CalculateAndSaveFinalEloDeltaForMatchForfeit(
                            match.MatchReporting.FindTeamsInTheMatch(interfaceLeague),
                            match.MatchReporting.TeamIdsWithReportData.ToDictionary(x => x.Key, x => x.Value), teamId);

                        Log.WriteLine("Possibly deprecated, rework this", LogLevel.WARNING);
                        match.FinishTheMatch(interfaceLeague);
                    }
                }

                foreach (var item in interfaceLeague.LeagueData.Teams.TeamsConcurrentBag.Where(
                    t => t.TeamId == teamId))
                {
                    interfaceLeague.LeagueData.Teams.TeamsConcurrentBag.TryTake(out Team? _removedTeam);
                    Log.WriteLine("Removed match " + item.TeamId, LogLevel.DEBUG);
                }

                Log.WriteLine("Found and removed" + _playerDiscordId + " in team with id: " + teamId, LogLevel.DEBUG);
            }

            try
            {
                var challengeMessage = Categories.FindInterfaceCategoryWithId(
                    interfaceLeague.LeagueCategoryId).
                        FindInterfaceChannelWithNameInTheCategory(
                            ChannelType.CHALLENGE).FindInterfaceMessageWithNameInTheChannel(
                                MessageName.CHALLENGEMESSAGE);

                await challengeMessage.GenerateAndModifyTheMessage();
            }
            catch (Exception ex) 
            {
                Log.WriteLine(ex.Message, LogLevel.ERROR);
                continue;
            }

            // Re-implement the player removal from here

            /*
            Dictionary<string, InterfaceMessage> leagueRegistrationMessages = new ConcurrentDictionary<string, InterfaceMessage>();

            
            // Replaced league name with the channel/catergoryname
            foreach (var kvp in interfaceChannel.InterfaceMessagesWithIds)
            {
                if (kvp.Value.MessageName == MessageName.LEAGUEREGISTRATIONMESSAGE)
                {
                    leagueRegistrationMessages.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (var messageKvp in leagueRegistrationMessages)
            {
                Log.WriteLine("ON: " + messageKvp.Key, LogLevel.DEBUG);

                LEAGUEREGISTRATIONMESSAGE? leagueRegistrationMessage = messageKvp.Value as LEAGUEREGISTRATIONMESSAGE;
                if (leagueRegistrationMessage == null)
                {
                    Log.WriteLine(nameof(leagueRegistrationMessage) + " was null!", LogLevel.CRITICAL);
                    return;
                }

                Log.WriteLine("ids: " + leagueRegistrationMessage.belongsToLeagueCategoryId + " | " +
                    interfaceLeague.DiscordLeagueReferences.LeagueCategoryId, LogLevel.DEBUG);

                if (leagueRegistrationMessage.belongsToLeagueCategoryId ==
                    interfaceLeague.DiscordLeagueReferences.LeagueCategoryId)
                {
                    Log.WriteLine("true, modifying", LogLevel.VERBOSE);

                    await messageKvp.Value.ModifyMessage(
                        leagueRegistrationMessage.GenerateMessageForSpecificCategoryLeague());

                    //await messageKvp.Value.GenerateAndModifyTheMessage();
                }

                Log.WriteLine("after if", LogLevel.VERBOSE);

                /*
                var MessageDescription = messageKvp.Value;

                var leagueRegistrationMessage = MessageDescription as LEAGUEREGISTRATIONMESSAGE;
                if (leagueRegistrationMessage == null)
                {
                    Log.WriteLine(nameof(leagueRegistrationMessage) + " was null!", LogLevel.ERROR);
                    continue;
                }

                if (leagueRegistrationMessage.IfLeagueRegistrationMessageIsCorrectFromCategoryId(
                    interfaceChannel, interfaceLeague.DiscordLeagueReferences.LeagueCategoryId))
                {
                    await leagueRegistrationMessage.GenerateAndModifyTheMessage();
                }
            }

            Log.WriteLine("Done looping through " + leagueRegistrationMessages.Count + " messages", LogLevel.VERBOSE);


            Log.WriteLine("before updating leaderboard", LogLevel.VERBOSE);

            // Updates the leaderboard after the player has been removed from the league
            interfaceLeague.UpdateLeagueLeaderboard();

            Log.WriteLine("Done processing league: " + interfaceLeague.LeagueCategoryName, LogLevel.VERBOSE);*/
        }

        Log.WriteLine("Done processing all leagues", LogLevel.VERBOSE);

        /*
        // Remove user's access (back to the registration...)
        await RoleManager.RevokeUserAccess(_playerDiscordId, "Member");

        foreach (InterfaceLeague interfaceLeague in Database.Instance.Leagues.StoredLeagues)
        {
            if (interfaceLeague.LeagueData.Teams.CheckIfPlayerIsAlreadyInATeamById(
                interfaceLeague.LeaguePlayerCountPerTeam, _playerDiscordId))
            {
                await RoleManager.RevokeUserAccess(_playerDiscordId, EnumExtensions.GetEnumMemberAttrValue(
                    interfaceLeague.LeagueCategoryName));
            } 
        }*/

        await SerializationManager.SerializeDB();
    }
}