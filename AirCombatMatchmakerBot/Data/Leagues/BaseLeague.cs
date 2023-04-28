using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System.Collections.Concurrent;

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
        get
        {
            Log.WriteLine("Getting " + nameof(leaguePlayerCountPerTeam) + ": " + leaguePlayerCountPerTeam, LogLevel.VERBOSE);
            return leaguePlayerCountPerTeam;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leaguePlayerCountPerTeam) + leaguePlayerCountPerTeam
                + " to: " + value, LogLevel.VERBOSE);
            leaguePlayerCountPerTeam = value;
        }
    }

    ConcurrentBag<UnitName> InterfaceLeague.LeagueUnits
    {
        get
        {
            Log.WriteLine("Getting " + nameof(leagueUnits) + ": " + leagueUnits, LogLevel.VERBOSE);
            return leagueUnits;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(leagueUnits) + leagueUnits
                + " to: " + value, LogLevel.VERBOSE);
            leagueUnits = value;
        }
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

    DiscordLeagueReferences InterfaceLeague.DiscordLeagueReferences
    {
        get
        {
            Log.WriteLine("Getting " + nameof(discordLeagueReferences) + ": " + discordLeagueReferences, LogLevel.VERBOSE);
            return discordLeagueReferences;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(discordLeagueReferences) + discordLeagueReferences
                + " to: " + value, LogLevel.VERBOSE);
            discordLeagueReferences = value;
        }
    }

    // Generated based on the implementation
    [DataMember] protected CategoryType leagueCategoryName;
    [DataMember] protected Era leagueEra;
    [DataMember] protected int leaguePlayerCountPerTeam;
    [DataMember] protected ConcurrentBag<UnitName> leagueUnits = new ConcurrentBag<UnitName>();
    [DataMember] protected LeagueData leagueData = new LeagueData();
    [DataMember] protected DiscordLeagueReferences discordLeagueReferences = new DiscordLeagueReferences();

    public BaseLeague()
    {
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);

    public InterfaceCategory? FindLeaguesInterfaceCategory()
    {
        Log.WriteLine("Finding interfaceCategory in: " + leagueCategoryName +
            " with id: " + discordLeagueReferences.LeagueCategoryId, LogLevel.VERBOSE);

        InterfaceCategory interfaceCategory = 
            Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
                discordLeagueReferences.LeagueCategoryId).Value;
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
                discordLeagueReferences.LeagueCategoryId).Value;

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
            discordLeagueReferences.LeagueCategoryId).Value.FindInterfaceChannelWithNameInTheCategory(
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

    public async Task<(string, bool)> RegisterUserToALeague(ulong _userId)
    {
        string responseMsg = string.Empty;

        Log.WriteLine("Registering user to league: " +
            leagueCategoryName, LogLevel.VERBOSE);

        ulong challengeChannelId = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            discordLeagueReferences.LeagueCategoryId).Value.FindInterfaceChannelWithNameInTheCategory(
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
                return (errorMsg, false);
            }

            Log.WriteLine("Found player: " + player.PlayerNickName +
                " (" + player.PlayerDiscordId + ")", LogLevel.VERBOSE);

            bool playerIsInATeamAlready = leagueData.Teams.CheckIfPlayerIsAlreadyInATeamById(
                leaguePlayerCountPerTeam, _userId);

            bool playerIsInActiveTeamAlready = leagueData.Teams.CheckIfPlayersTeamIsActiveByIdAndReturnThatTeam(
                leaguePlayerCountPerTeam, _userId).TeamActive;

            if (!playerIsInATeamAlready)
            {
                Log.WriteLine("The player was not found in any team in the league", LogLevel.VERBOSE);

                // Create a team with unique ID and increment that ID
                // after the data has been serialized
                Team newTeam = new Team(
                    new ConcurrentBag<Player> { player },
                    player.PlayerNickName,
                    leagueData.Teams.CurrentTeamInt);

                if (leaguePlayerCountPerTeam < 2)
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
                    Log.WriteLine("This league is team based with number of players per team: " +
                        leaguePlayerCountPerTeam, LogLevel.ERROR);
                    return ("", false);
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
                return (responseMsg, false);
            }
        }
        else
        {
            responseMsg = "Error joining the league! Press the register button first!" +
                " (only admins should be able to see this)";
            Log.WriteLine("Player: " + _userId +
                " tried to join a league before registering", LogLevel.WARNING);
            return (responseMsg, false);
        }

        UpdateLeagueLeaderboard();

        return (responseMsg, true);
    }
}