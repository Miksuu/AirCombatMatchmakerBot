using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

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

    List<UnitName> InterfaceLeague.LeagueUnits
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
            Log.WriteLine("Getting " + nameof(discordleagueReferences) + ": " + discordleagueReferences, LogLevel.VERBOSE);
            return discordleagueReferences;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(discordleagueReferences) + discordleagueReferences
                + " to: " + value, LogLevel.VERBOSE);
            discordleagueReferences = value;
        }
    }

    // Generated based on the implementation
    [DataMember] protected CategoryType leagueCategoryName;
    [DataMember] protected Era leagueEra;
    [DataMember] protected int leaguePlayerCountPerTeam;
    [DataMember] protected List<UnitName> leagueUnits = new List<UnitName>();
    [DataMember] protected LeagueData leagueData = new LeagueData();
    [DataMember] protected DiscordLeagueReferences discordleagueReferences = new DiscordLeagueReferences();

    public BaseLeague()
    {
    }

    public abstract List<Overwrite> GetGuildPermissions(SocketGuild _guild, SocketRole _role);

    public string GenerateALeagueJoinButtonMessage()
    {
        string? leagueEnumAttrValue =
            EnumExtensions.GetEnumMemberAttrValue(leagueCategoryName);

        Log.WriteLine(nameof(leagueEnumAttrValue) + ": " +
            leagueEnumAttrValue, LogLevel.VERBOSE);

        return "." + "\n" + leagueEnumAttrValue + "\n" +
            GetAllowedUnitsAsString() + "\n" +
            GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface();
    }

    public string GetAllowedUnitsAsString()
    {
        string allowedUnits = string.Empty;

        for (int u = 0; u < leagueUnits.Count; ++u)
        {
            allowedUnits +=
                EnumExtensions.GetEnumMemberAttrValue(leagueUnits[u]);

            // Is not the last index
            if (u != leagueUnits.Count - 1)
            {
                allowedUnits += ", ";
            }
        }

        return allowedUnits;
    }

    public string GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface()
    {
        int count = 0;

        foreach (Team team in leagueData.Teams.TeamsList)
        {
            string teamName = team.GetTeamName(leaguePlayerCountPerTeam);

            if (team.TeamActive)
            {
                count++;
                Log.WriteLine("team: " + teamName +
                    " is active, increased count to: " + count, LogLevel.VERBOSE);
            }
            else
            {
                Log.WriteLine("team: " + teamName + " is not active", LogLevel.VERBOSE);
            }
        }

        Log.WriteLine("Total count: " + count, LogLevel.VERBOSE);

        if (leaguePlayerCountPerTeam > 1)
        {
            return "Teams: " + count;
        }
        else
        {
            return "Players: " + count;
        }
    }

    public async Task ModifyLeagueRegisterationChannelMessage()
    {
        Log.WriteLine("Modifying league registration channel message with: " +
            leagueCategoryName, LogLevel.VERBOSE);

        // Find the category fo the message ID
        var category = Database.Instance.Categories.CreatedCategoriesWithChannels.FirstOrDefault(
            x => x.Value.CategoryType == CategoryType.REGISTRATIONCATEGORY);

        if (category.Value == null)
        {
            Log.WriteLine(nameof(category) + " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Found category: " + category.Value.CategoryType, LogLevel.DEBUG);

        var channel = category.Value.InterfaceChannels.FirstOrDefault(
                x => x.Value.ChannelType == ChannelType.LEAGUEREGISTRATION);

        if (channel.Value == null)
        {
            Log.WriteLine(nameof(channel) + " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Found channel: " + channel.Value.ChannelName, LogLevel.DEBUG);

        var messageId = channel.Value.InterfaceMessagesWithIds.FirstOrDefault(
            x => x.Key.ToString() == leagueCategoryName.ToString()).Value.MessageId;

        Log.WriteLine("Found messageId: " + messageId, LogLevel.DEBUG);

        await MessageManager.ModifyMessage(
            channel.Value.ChannelId, messageId, GenerateALeagueJoinButtonMessage());
    }

    public string GenerateALeagueChallengeButtonMessage()
    {
        string? leagueEnumAttrValue =
            EnumExtensions.GetEnumMemberAttrValue(leagueCategoryName);

        Log.WriteLine(nameof(leagueEnumAttrValue) + ": " +
            leagueEnumAttrValue, LogLevel.VERBOSE);

        return "." + "\n" + leagueEnumAttrValue + "\n" +
            GetAllowedUnitsAsString() + "\n" +
            GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface();
    }
}