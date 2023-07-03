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

    public EventManager LeagueEventManager
    {
        get => leagueEventManager.GetValue();
        set => leagueEventManager.SetValue(value);
    }

    // Generated based on the implementation
    [DataMember] protected logEnum<LeagueName> leagueCategoryName = new logEnum<LeagueName>();
    [DataMember] protected logEnum<Era> leagueEra = new logEnum<Era>();
    [DataMember] protected logVar<int> leaguePlayerCountPerTeam = new logVar<int>();
    [DataMember] protected logConcurrentBag<UnitName> leagueUnits = new logConcurrentBag<UnitName>();
    [DataMember] protected LeagueData leagueData = new LeagueData();

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
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);

    public InterfaceCategory FindLeaguesInterfaceCategory()
    {
        Log.WriteLine("Finding interfaceCategory in: " + thisInterfaceLeague.LeagueCategoryName +
            " with id: " + LeagueCategoryId);

        InterfaceCategory interfaceCategory = 
            Database.Instance.Categories.FindInterfaceCategoryWithId(LeagueCategoryId);
        if (interfaceCategory == null)
        {
            Log.WriteLine(nameof(interfaceCategory) + " was null!", LogLevel.CRITICAL);
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
        Log.WriteLine("Updating leaderboard on: " + thisInterfaceLeague.LeagueCategoryName);

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



        Log.WriteLine("Done updating leaderboard on: " + thisInterfaceLeague.LeagueCategoryName);
    }

    public async Task<Response> RegisterUserToALeague(ulong _userId)
    {
        try
        {
            string responseMsg = string.Empty;

            Log.WriteLine("Registering user to league: " +
                thisInterfaceLeague.LeagueCategoryName);

            InterfaceChannel foundChannel = Database.Instance.Categories.FindInterfaceCategoryWithId(
                LeagueCategoryId).FindInterfaceChannelWithNameInTheCategory(
                    ChannelType.CHALLENGE);

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
                    return new Response(errorMsg, false);
                }

                Log.WriteLine("Found player: " + player.PlayerNickName +
                    " (" + player.PlayerDiscordId + ")");

                bool playerIsInATeamAlready = LeagueData.Teams.CheckIfPlayerIsAlreadyInATeamById(_userId);

                bool playerIsInActiveTeamAlready = LeagueData.Teams.CheckIfPlayersTeamIsActiveByIdAndReturnThatTeam(
                    _userId).TeamActive;

                if (!playerIsInATeamAlready)
                {
                    Log.WriteLine("The player was not found in any team in the league");

                    // Create a team with unique ID and increment that ID
                    // after the data has been serialized
                    Team newTeam = new Team(
                        new ConcurrentBag<Player> { player },
                        player.PlayerNickName,
                        LeagueData.Teams.CurrentTeamInt);

                    if (thisInterfaceLeague.LeaguePlayerCountPerTeam < 2)
                    {
                        Log.WriteLine("This league is solo");

                        LeagueData.Teams.AddToConcurrentBagOfTeams(newTeam);

                        //responseMsg = "Registration complete on: " +
                        //    EnumExtensions.GetEnumMemberAttrValue(thisInterfaceLeague.LeagueCategoryName) + "\n" +
                        //    " You can look for a match in: <#" + challengeChannelId + ">";
                    }
                    else
                    {
                        // Not implemented yet
                        string errorMsg =
                            "This league is team based with number of players per team: " + leaguePlayerCountPerTeam;
                        Log.WriteLine(errorMsg, LogLevel.CRITICAL);
                        return new Response(errorMsg, false);
                    }

                    // Add the role for the player for the specific league and set him teamActive
                    await UserManager.SetTeamActiveAndGrantThePlayerRole(this, _userId);

                    Log.WriteLine("Done creating team: " + newTeam + " team count is now: " +
                        LeagueData.Teams.TeamsConcurrentBag.Count, LogLevel.DEBUG);

                    LeagueData.Teams.IncrementCurrentTeamInt();
                }
                else if (playerIsInATeamAlready && !playerIsInActiveTeamAlready)
                {
                    // Need to handle team related behaviour better later

                    Log.WriteLine("The player was already in a team in that league!" +
                        " Setting him active", LogLevel.DEBUG);

                    await UserManager.SetTeamActiveAndGrantThePlayerRole(this, _userId);

                    //responseMsg = "You have rejoined: " +
                    //    EnumExtensions.GetEnumMemberAttrValue(thisInterfaceLeague.LeagueCategoryName) + "\n" +
                    //    " You can look for a match in: <#" + challengeChannelId + ">";
                }
                else if (playerIsInATeamAlready && playerIsInActiveTeamAlready)
                {
                    // Temp fix for fast clickers of two different leagues they can just press it again to receive the role
                    await UserManager.SetTeamActiveAndGrantThePlayerRole(this, _userId);

                    Log.WriteLine("Player " + player.PlayerDiscordId + " tried to join: " + thisInterfaceLeague.LeagueCategoryName +
                        ", had a team already active");
                    //responseMsg = "You are already part of " + EnumExtensions.GetEnumMemberAttrValue(thisInterfaceLeague.LeagueCategoryName) +
                    //    "\n" + " You can look for a match in: <#" + challengeChannelId + ">";
                    return new Response(responseMsg, false);
                }
            }
            else
            {
                responseMsg = "Error joining the league! Press the register button first!" +
                    " (only admins should be able to see this)";
                Log.WriteLine("Player: " + _userId +
                    " tried to join a league before registering", LogLevel.WARNING);
                return new Response(responseMsg, false);
            }

            UpdateLeagueLeaderboard();

            return new Response(responseMsg, true);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            return new Response(ex.Message, false);
        }
    }

    public void HandleLeaguesAndItsMatchesEvents(ulong _currentUnixTime)
    {
        LeagueEventManager.HandleEvents(_currentUnixTime);

        foreach (LeagueMatch leagueMatch in LeagueData.Matches.MatchesConcurrentBag)
        {
            leagueMatch.MatchEventManager.HandleEvents(_currentUnixTime);
        }
    }
}