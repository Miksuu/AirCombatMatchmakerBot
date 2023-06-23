using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;

[DataContract]
public abstract class BaseLeague : InterfaceLeague
{
    LeagueName InterfaceLeague.LeagueCategoryName
    {
        get => leagueCategoryName.GetValue();
        set => leagueCategoryName.SetValue(value);
    }

    Era InterfaceLeague.LeagueEra
    {
        get => leagueEra.GetValue();
        set => leagueEra.SetValue(value);
    }

    int InterfaceLeague.LeaguePlayerCountPerTeam
    {
        get => leaguePlayerCountPerTeam.GetValue();
        set => leaguePlayerCountPerTeam.SetValue(value);
    }

    ConcurrentBag<UnitName> InterfaceLeague.LeagueUnits
    {
        get => leagueUnits.GetValue();
        set => leagueUnits.SetValue(value);
    }

    public LeagueData LeagueData
    {
        get => leagueData.GetValue();
        set => leagueData.SetValue(value);
    }

    public ulong LeagueCategoryId
    {
        get => leagueCategoryId.GetValue();
        set => leagueCategoryId.SetValue(value);
    }

    [IgnoreDataMember]
    public ConcurrentDictionary<ChannelType, ulong> LeagueChannels
    {
        get => leagueChannels.GetValue();
        set => leagueChannels.SetValue(value);
    }

    public ulong LeagueRoleId
    {
        get => leagueRoleId.GetValue();
        set => leagueRoleId.SetValue(value);
    }

    public ulong LeagueRegistrationMessageId
    {
        get => leagueRegistrationMessageId.GetValue();
        set => leagueRegistrationMessageId.SetValue(value);
    }

    public ConcurrentBag<ScheduledEvent> LeagueScheduledEvents
    {
        get => leagueScheduledEvents.GetValue();
        set => leagueScheduledEvents.SetValue(value);
    }

    // Generated based on the implementation
    [DataMember] protected logClass<LeagueName> leagueCategoryName = new logClass<LeagueName>(new LeagueName());
    [DataMember] protected logClass<Era> leagueEra = new logClass<Era>(new Era());
    [DataMember] protected logClass<int> leaguePlayerCountPerTeam = new logClass<int>();
    [DataMember] protected logConcurrentBag<UnitName> leagueUnits = new logConcurrentBag<UnitName>();
    [DataMember] protected logClass<LeagueData> leagueData = new logClass<LeagueData>(new LeagueData());

    // The reference to the category created by the system
    [DataMember] private logClass<ulong> leagueCategoryId = new logClass<ulong>();

    // The references for the channelTypes inside the category
    [DataMember] private logConcurrentDictionary<ChannelType, ulong> leagueChannels = new logConcurrentDictionary<ChannelType, ulong>();

    // Id of the role which gives access to the league channelTypes
    [DataMember] private logClass<ulong> leagueRoleId = new logClass<ulong>();

    // Reference to the MessageDescription related to this league on the #league-registration channel
    [DataMember] private logClass<ulong> leagueRegistrationMessageId = new logClass<ulong>();

    // Events to loop through in the specific league
    [DataMember] private logConcurrentBag<ScheduledEvent> leagueScheduledEvents = new logConcurrentBag<ScheduledEvent>();

    protected InterfaceLeague thisInterfaceLeague;

    public BaseLeague()
    {
        thisInterfaceLeague = this;
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);

    public InterfaceCategory FindLeaguesInterfaceCategory()
    {
        Log.WriteLine("Finding interfaceCategory in: " + thisInterfaceLeague.LeagueCategoryName +
            " with id: " + LeagueCategoryId, LogLevel.VERBOSE);

        InterfaceCategory interfaceCategory = 
            Database.Instance.Categories.FindInterfaceCategoryWithId(LeagueCategoryId);
        if (interfaceCategory == null)
        {
            Log.WriteLine(nameof(interfaceCategory) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(interfaceCategory) + " was null!");
        }

        Log.WriteLine("Found: " + interfaceCategory.CategoryType, LogLevel.VERBOSE);

        return interfaceCategory;
    }

    public async Task PostMatchReport(string _finalResultMessage, string _finalResultTitle,
        AttachmentData[] _attachmentDatas)
    {
        try
        {
            InterfaceCategory leagueCategory =
                Database.Instance.Categories.FindInterfaceCategoryWithId(LeagueCategoryId);

            InterfaceChannel matchReportsChannelInterface =
                leagueCategory.FindInterfaceChannelWithNameInTheCategory(ChannelType.MATCHREPORTSCHANNEL);

            var client = BotReference.GetClientRef();
            if (client == null)
            {
                Exceptions.BotClientRefNull();
                return;
            }

            var textChannel = await client.GetChannelAsync(matchReportsChannelInterface.ChannelId) as ITextChannel;
            if (textChannel == null)
            {
                Log.WriteLine(nameof(textChannel) + " was null!", LogLevel.ERROR);
                return;
            }

            await matchReportsChannelInterface.CreateARawMessageForTheChannelFromMessageNameWithAttachmentData(
                _finalResultMessage, _attachmentDatas, _finalResultTitle);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }
    }

    public void UpdateLeagueLeaderboard()
    {
        Log.WriteLine("Updating leaderboard on: " + thisInterfaceLeague.LeagueCategoryName, LogLevel.VERBOSE);

        try
        {
            InterfaceMessage leagueStatusMessage =
                Database.Instance.Categories.FindInterfaceCategoryWithId(
                    LeagueCategoryId).FindInterfaceChannelWithNameInTheCategory(ChannelType.LEAGUESTATUS).
                        FindInterfaceMessageWithNameInTheChannel(
                            MessageName.LEAGUESTATUSMESSAGE);

            leagueStatusMessage.GenerateAndModifyTheMessage();
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return;
        }



        Log.WriteLine("Done updating leaderboard on: " + thisInterfaceLeague.LeagueCategoryName, LogLevel.VERBOSE);
    }

    public Task<Response> RegisterUserToALeague(ulong _userId)
    {
        string responseMsg = string.Empty;

        Log.WriteLine("Registering user to league: " +
            thisInterfaceLeague.LeagueCategoryName, LogLevel.VERBOSE);

        InterfaceChannel foundChannel;

        try
        {
            foundChannel = Database.Instance.Categories.FindInterfaceCategoryWithId(
                LeagueCategoryId).FindInterfaceChannelWithNameInTheCategory(
                    ChannelType.CHALLENGE);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return Task.FromResult(new Response(ex.Message, false));
        }

        ulong challengeChannelId = foundChannel.ChannelId;

        // Check that the player is in the PlayerData
        // (should be, he doesn't see this button before, except if hes admin)
        if (Database.Instance.PlayerData.CheckIfPlayerDataPlayerIDsContainsKey(
            _userId))
        {
            Player player = Database.Instance.PlayerData.GetAPlayerProfileById(
                _userId);
            if (player.PlayerDiscordId == 0)
            {
                string errorMsg = "Player's: " + player.PlayerNickName + " id was 0!";
                Log.WriteLine(errorMsg, LogLevel.CRITICAL);
                return Task.FromResult(new Response(errorMsg, false));
            }

            Log.WriteLine("Found player: " + player.PlayerNickName +
                " (" + player.PlayerDiscordId + ")", LogLevel.VERBOSE);

            bool playerIsInATeamAlready = LeagueData.Teams.CheckIfPlayerIsAlreadyInATeamById(_userId);

            bool playerIsInActiveTeamAlready = LeagueData.Teams.CheckIfPlayersTeamIsActiveByIdAndReturnThatTeam(
                _userId).TeamActive;

            if (!playerIsInATeamAlready)
            {
                Log.WriteLine("The player was not found in any team in the league", LogLevel.VERBOSE);

                // Create a team with unique ID and increment that ID
                // after the data has been serialized
                Team newTeam = new Team(
                    new ConcurrentBag<Player> { player },
                    player.PlayerNickName,
                    LeagueData.Teams.CurrentTeamInt);

                if (thisInterfaceLeague.LeaguePlayerCountPerTeam < 2)
                {
                    Log.WriteLine("This league is solo", LogLevel.VERBOSE);
                        
                    LeagueData.Teams.AddToConcurrentBagOfTeams(newTeam);

                    responseMsg = "Registration complete on: " +
                        EnumExtensions.GetEnumMemberAttrValue(thisInterfaceLeague.LeagueCategoryName) + "\n" +
                        " You can look for a match in: <#" + challengeChannelId + ">";
                }
                else
                {
                    // Not implemented yet
                    string errorMsg = 
                        "This league is team based with number of players per team: " + leaguePlayerCountPerTeam;
                    Log.WriteLine(errorMsg, LogLevel.CRITICAL);
                    return Task.FromResult(new Response(errorMsg, false));
                }

                // Add the role for the player for the specific league and set him teamActive
                UserManager.SetTeamActiveAndGrantThePlayerRole(this, _userId);

                Log.WriteLine("Done creating team: " + newTeam + " team count is now: " +
                    LeagueData.Teams.TeamsConcurrentBag.Count, LogLevel.DEBUG);

                LeagueData.Teams.IncrementCurrentTeamInt();
            }
            else if (playerIsInATeamAlready && !playerIsInActiveTeamAlready)
            {
                // Need to handle team related behaviour better later

                Log.WriteLine("The player was already in a team in that league!" +
                    " Setting him active", LogLevel.DEBUG);

                UserManager.SetTeamActiveAndGrantThePlayerRole(this, _userId);

                responseMsg = "You have rejoined: " +
                    EnumExtensions.GetEnumMemberAttrValue(thisInterfaceLeague.LeagueCategoryName) + "\n" +
                    " You can look for a match in: <#" + challengeChannelId + ">";
            }
            else if (playerIsInATeamAlready && playerIsInActiveTeamAlready)
            {
                // Temp fix for fast clickers of two different leagues they can just press it again to receive the role
                UserManager.SetTeamActiveAndGrantThePlayerRole(this, _userId);

                Log.WriteLine("Player " + player.PlayerDiscordId + " tried to join: " + thisInterfaceLeague.LeagueCategoryName +
                    ", had a team already active", LogLevel.VERBOSE);
                responseMsg = "You are already part of " + EnumExtensions.GetEnumMemberAttrValue(thisInterfaceLeague.LeagueCategoryName) +
                    "\n" + " You can look for a match in: <#" + challengeChannelId + ">";
                return Task.FromResult(new Response(responseMsg, false));
            }
        }
        else
        {
            responseMsg = "Error joining the league! Press the register button first!" +
                " (only admins should be able to see this)";
            Log.WriteLine("Player: " + _userId +
                " tried to join a league before registering", LogLevel.WARNING);
            return Task.FromResult(new Response(responseMsg, false));
        }

        UpdateLeagueLeaderboard();

        return Task.FromResult(new Response(responseMsg, true));
    }
}