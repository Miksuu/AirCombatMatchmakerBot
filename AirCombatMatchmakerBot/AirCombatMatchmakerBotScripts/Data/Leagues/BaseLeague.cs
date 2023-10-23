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
    LeagueData InterfaceLeague.LeagueData { get => leagueData; set => leagueData = value; }
    EventManager InterfaceLeague.LeagueEventManager { get => leagueEventManager; set => leagueEventManager = value; }

    // Generated based on the implementation
    [DataMember] protected logEnum<LeagueName> leagueCategoryName = new logEnum<LeagueName>();
    [DataMember] protected logEnum<Era> leagueEra = new logEnum<Era>();
    [DataMember] protected logVar<int> leaguePlayerCountPerTeam = new logVar<int>();
    [DataMember] protected logConcurrentBag<UnitName> leagueUnits = new logConcurrentBag<UnitName>();
    [DataMember] private LeagueData leagueData = new LeagueData();

    // The reference to the category created by the system
    [DataMember] private logVar<ulong> leagueCategoryId = new logVar<ulong>();

    // The references for the channelTypes inside the category
    [DataMember] private logConcurrentDictionary<ChannelType, ulong> leagueChannels = new logConcurrentDictionary<ChannelType, ulong>();

    // Id of the role which gives access to the league channelTypes
    [DataMember] private logVar<ulong> leagueRoleId = new logVar<ulong>();

    // Reference to the MessageDescription related to this league on the #league-registration channel
    [DataMember] private logVar<ulong> leagueRegistrationMessageId = new logVar<ulong>();

    // Events to loop through in the specific league
    [DataMember] private EventManager leagueEventManager = new EventManager();

    protected InterfaceLeague thisInterfaceLeague;

    public BaseLeague()
    {
        thisInterfaceLeague = this;
        //leagueData.SetReferences(thisInterfaceLeague);
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);

    public InterfaceCategory FindLeaguesInterfaceCategory()
    {
        Log.WriteLine("Finding interfaceCategory in: " + thisInterfaceLeague.LeagueCategoryName +
            " with id: " + LeagueCategoryId);

        InterfaceCategory interfaceCategory =
            Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryWithCategoryId(LeagueCategoryId);
        if (interfaceCategory == null)
        {
            Log.WriteLine(nameof(interfaceCategory) + " was null!", LogLevel.ERROR);
            throw new InvalidOperationException(nameof(interfaceCategory) + " was null!");
        }

        Log.WriteLine("Found: " + interfaceCategory.CategoryType);

        return interfaceCategory;
    }

    public async Task PostMatchReport(string _finalResultMessage, string _finalResultTitle,
        AttachmentData[] _attachmentDatas)
    {
        try
        {
            InterfaceCategory leagueCategory =
                Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryWithCategoryId(LeagueCategoryId);

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
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return;
        }
    }

    public void UpdateLeagueLeaderboard()
    {
        Log.WriteLine("Updating leaderboard on: " + thisInterfaceLeague.LeagueCategoryName);

        try
        {
            InterfaceMessage leagueStatusMessage =
                Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryWithCategoryId(
                    LeagueCategoryId).FindInterfaceChannelWithNameInTheCategory(ChannelType.LEAGUESTATUS).
                        FindInterfaceMessageWithNameInTheChannel(
                            MessageName.LEAGUESTATUSMESSAGE);

            leagueStatusMessage.GenerateAndModifyTheMessage();
        }
        catch (Exception ex) 
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return;
        }

        Log.WriteLine("Done updating leaderboard on: " + thisInterfaceLeague.LeagueCategoryName);
    }

    public async Task<Response> RegisterUserToALeague(ulong _userId)
    {
        string responseMsg = string.Empty;
        Log.WriteLine("Registering user to league: " + thisInterfaceLeague.LeagueCategoryName);

        if (!Database.GetInstance<ApplicationDatabase>().PlayerData.CheckIfPlayerDataPlayerIDsContainsKey(_userId))
        {
            responseMsg = "Error joining the league! Press the register button first!" +
                " (only admins should be able to see this)";
            Log.WriteLine("Player: " + _userId + " tried to join a league before registering", LogLevel.WARNING);
            return new Response(responseMsg, false);
        }

        Player player = Database.GetInstance<ApplicationDatabase>().PlayerData.GetAPlayerProfileById(_userId);
        if (player.PlayerDiscordId == 0)
        {
            string errorMsg = "Player's: " + player.PlayerNickName + " id was 0!";
            Log.WriteLine(errorMsg, LogLevel.ERROR);
            return new Response(errorMsg, false);
        }

        Log.WriteLine("Found player: " + player.PlayerNickName + " (" + player.PlayerDiscordId + ")");

        bool playerIsInATeamAlready = thisInterfaceLeague.LeagueData.Teams.CheckIfPlayerIsAlreadyInATeamById(_userId);
        bool playerIsInActiveTeamAlready = thisInterfaceLeague.LeagueData.Teams.CheckIfPlayersTeamIsActiveByIdAndReturnThatTeam(_userId).TeamActive;

        if (!playerIsInATeamAlready)
        {
            var response = await HandleNewPlayerRegistration(player);
            responseMsg = response.responseString;
        }
        else if (playerIsInATeamAlready && !playerIsInActiveTeamAlready)
        {
            var response = await HandleExistingPlayerRegistration(_userId);
            responseMsg = response.responseString;
        }
        else if (playerIsInATeamAlready && playerIsInActiveTeamAlready)
        {
            return await HandleActivePlayerRegistration(_userId);
        }

        UpdateLeagueLeaderboard();
        
        return new Response(responseMsg, true);
    }

    private async Task<Response> HandleNewPlayerRegistration(Player _player)
    {
        Log.WriteLine("The player was not found in any team in the league");

        Team newTeam = new Team(
            new ConcurrentBag<Player> { _player },
            _player.PlayerNickName,
            thisInterfaceLeague.LeagueData.Teams.CurrentTeamInt);

        if (thisInterfaceLeague.LeaguePlayerCountPerTeam < 2)
        {
            Log.WriteLine("This league is solo");
            thisInterfaceLeague.LeagueData.Teams.AddToConcurrentBagOfTeams(newTeam);
        }
        else
        {
            string errorMsg = "This league is team based with number of players per team: " + leaguePlayerCountPerTeam;
            Log.WriteLine(errorMsg, LogLevel.ERROR);
            return new Response(errorMsg, false);
        }

        await UserManager.SetTeamActiveAndGrantThePlayerRole(this, _player.PlayerDiscordId, newTeam);
        Log.WriteLine("Done creating team: " + newTeam + " team count is now: " + thisInterfaceLeague.LeagueData.Teams.TeamsConcurrentBag.Count, LogLevel.DEBUG);
        thisInterfaceLeague.LeagueData.Teams.IncrementCurrentTeamInt();

        return new Response("", true);
    }

    private async Task<Response> HandleExistingPlayerRegistration(ulong _userId)
    {
        Log.WriteLine("The player was already in a team in that league! Setting him active", LogLevel.DEBUG);
        var foundTeam = thisInterfaceLeague.LeagueData.Teams.ReturnTeamThatThePlayerIsIn(_userId);
        await UserManager.SetTeamActiveAndGrantThePlayerRole(this, _userId, foundTeam);
        return new Response("You have rejoined the league", true);
    }

    private async Task<Response> HandleActivePlayerRegistration(ulong _userId)
    {
        var foundTeam = thisInterfaceLeague.LeagueData.Teams.ReturnTeamThatThePlayerIsIn(_userId);
        await UserManager.SetTeamActiveAndGrantThePlayerRole(this, _userId, foundTeam);
        Log.WriteLine("Player " + _userId + " tried to join: " + thisInterfaceLeague.LeagueCategoryName + ", had a team already active");
        return new Response("You are already part of the league", false);
    }

    public ConcurrentBag<ScheduledEvent> GetLeagueEventsAndReturnThemInConcurrentBag()
    {
        ConcurrentBag<ScheduledEvent> events = thisInterfaceLeague.LeagueEventManager.GetEvents();

        foreach (LeagueMatch leagueMatch in thisInterfaceLeague.LeagueData.Matches.MatchesConcurrentBag)
        {
            ConcurrentBag<ScheduledEvent> matchEvents = leagueMatch.MatchEventManager.GetEvents();
            foreach (var matchEvent in matchEvents)
            {
                events.Add(matchEvent);
            }
        }

        return events;
    }
}