using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Runtime.ConstrainedExecution;

[DataContract]
public abstract class BaseLeague : InterfaceLeague
{
    CategoryType InterfaceLeague.LeagueCategoryName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueCategoryName) + ": " + leagueCategoryName, LogLevel.VERBOSE);
            return leagueCategoryName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueCategoryName) + leagueCategoryName
                + " to: " + value, LogLevel.VERBOSE);
            leagueCategoryName = value;
        }
    }

    Era InterfaceLeague.LeagueEra
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueEra) + ": " + leagueEra, LogLevel.VERBOSE);
            return leagueEra;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueEra) + leagueEra
                + " to: " + value, LogLevel.VERBOSE);
            leagueEra = value;
        }
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

    LeagueData InterfaceLeague.LeagueData
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueData) + ": " + leagueData, LogLevel.VERBOSE);
            return leagueData;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueData) + leagueData
                + " to: " + value, LogLevel.VERBOSE);
            leagueData = value;
        }
    }

    public ulong LeagueCategoryId
    {
        get => leagueCategoryId.GetValue();
        set => leagueCategoryId.SetValue(value);
    }

    public ConcurrentDictionary<ChannelType, ulong> LeagueChannels
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueChannels) + " with count of: " +
                leagueChannels.Count, LogLevel.VERBOSE);
            return leagueChannels;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueChannels)
                + " to: " + value, LogLevel.VERBOSE);
            leagueChannels = value;
        }
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

    // Generated based on the implementation
    [DataMember] protected CategoryType leagueCategoryName;
    [DataMember] protected Era leagueEra;
    [DataMember] protected logInt leaguePlayerCountPerTeam = new logInt();
    [DataMember] protected logConcurrentBag<UnitName> leagueUnits = new logConcurrentBag<UnitName>();
    [DataMember] protected LeagueData leagueData = new LeagueData();

    // The reference to the category created by the system
    [DataMember] private logUlong leagueCategoryId = new logUlong();

    // The references for the channelTypes inside the category
    [DataMember] private ConcurrentDictionary<ChannelType, ulong> leagueChannels = new ConcurrentDictionary<ChannelType, ulong>();

    // Id of the role which gives access to the league channelTypes
    [DataMember] private logUlong leagueRoleId = new logUlong();

    // Reference to the MessageDescription related to this league on the #league-registration channel
    [DataMember] private logUlong leagueRegistrationMessageId = new logUlong();

    protected InterfaceLeague thisInterfaceLeague;

    public BaseLeague()
    {
        thisInterfaceLeague = this;
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);

    public InterfaceCategory? FindLeaguesInterfaceCategory()
    {
        Log.WriteLine("Finding interfaceCategory in: " + leagueCategoryName +
            " with id: " + LeagueCategoryId, LogLevel.VERBOSE);

        InterfaceCategory interfaceCategory = 
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                LeagueCategoryId).Value;
        if (interfaceCategory == null)
        {
            Log.WriteLine(nameof(interfaceCategory) + " was null!", LogLevel.CRITICAL);
            return interfaceCategory;
        }

        Log.WriteLine("Found: " + interfaceCategory.CategoryType, LogLevel.VERBOSE);

        return interfaceCategory;
    }

    public async Task PostMatchReport(string _finalResultMessage, string _finalResultTitle,
        AttachmentData[] _attachmentDatas)
    {
        InterfaceCategory leagueCategory =
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                LeagueCategoryId).Value;

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

    public void UpdateLeagueLeaderboard()
    {
        Log.WriteLine("Updating leaderboard on: " + leagueCategoryName, LogLevel.VERBOSE);

        var leagueStatusMessage = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            LeagueCategoryId).Value.FindInterfaceChannelWithNameInTheCategory(
                ChannelType.LEAGUESTATUS).FindInterfaceMessageWithNameInTheChannel(
                    MessageName.LEAGUESTATUSMESSAGE);

        if (leagueStatusMessage == null)
        {
            Log.WriteLine(nameof(leagueStatusMessage) + " was null!", LogLevel.ERROR);
            return;
        }

        leagueStatusMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("Done updating leaderboard on: " + leagueCategoryName, LogLevel.VERBOSE);
    }

    public Task<Response> RegisterUserToALeague(ulong _userId)
    {
        string responseMsg = string.Empty;

        Log.WriteLine("Registering user to league: " +
            leagueCategoryName, LogLevel.VERBOSE);

        ulong challengeChannelId = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            LeagueCategoryId).Value.FindInterfaceChannelWithNameInTheCategory(
            ChannelType.CHALLENGE).ChannelId;

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

            bool playerIsInATeamAlready = leagueData.Teams.CheckIfPlayerIsAlreadyInATeamById(
                thisInterfaceLeague.LeaguePlayerCountPerTeam, _userId);

            bool playerIsInActiveTeamAlready = leagueData.Teams.CheckIfPlayersTeamIsActiveByIdAndReturnThatTeam(
                thisInterfaceLeague.LeaguePlayerCountPerTeam, _userId).TeamActive;

            if (!playerIsInATeamAlready)
            {
                Log.WriteLine("The player was not found in any team in the league", LogLevel.VERBOSE);

                // Create a team with unique ID and increment that ID
                // after the data has been serialized
                Team newTeam = new Team(
                    new ConcurrentBag<Player> { player },
                    player.PlayerNickName,
                    leagueData.Teams.CurrentTeamInt);

                if (thisInterfaceLeague.LeaguePlayerCountPerTeam < 2)
                {
                    Log.WriteLine("This league is solo", LogLevel.VERBOSE);
                        
                    leagueData.Teams.AddToConcurrentBagOfTeams(newTeam);

                    responseMsg = "Registration complete on: " +
                        EnumExtensions.GetEnumMemberAttrValue(leagueCategoryName) + "\n" +
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
                    leagueData.Teams.TeamsConcurrentBag.Count, LogLevel.DEBUG);

                leagueData.Teams.IncrementCurrentTeamInt();
            }
            else if (playerIsInATeamAlready && !playerIsInActiveTeamAlready)
            {
                // Need to handle team related behaviour better later

                Log.WriteLine("The player was already in a team in that league!" +
                    " Setting him active", LogLevel.DEBUG);

                UserManager.SetTeamActiveAndGrantThePlayerRole(this, _userId);

                responseMsg = "You have rejoined: " +
                    EnumExtensions.GetEnumMemberAttrValue(leagueCategoryName) + "\n" +
                    " You can look for a match in: <#" + challengeChannelId + ">";
            }
            else if (playerIsInATeamAlready && playerIsInActiveTeamAlready)
            {
                Log.WriteLine("Player " + player.PlayerDiscordId + " tried to join: " + leagueCategoryName +
                    ", had a team already active", LogLevel.VERBOSE);
                responseMsg = "You are already part of " + EnumExtensions.GetEnumMemberAttrValue(leagueCategoryName) +
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