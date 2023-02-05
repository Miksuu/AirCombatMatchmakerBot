using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;
using System.Globalization;

[DataContract]
public class LEAGUEREGISTRATIONMESSAGE : BaseMessage
{
    public LEAGUEREGISTRATIONMESSAGE()
    {
        messageName = MessageName.LEAGUEREGISTRATIONMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>
        {
            { ButtonName.LEAGUEREGISTRATIONBUTTON, 1 }
        };
        message = "Insert league registration message here";
    }

    [DataMember] public ulong belongsToLeagueCategoryId;

    public override string GenerateMessage()
    {
        return message;
    }

    public string GenerateMessageForSpecificCategoryLeague()
    {
        Log.WriteLine("Starting to generate the league registration message with: " +
            belongsToLeagueCategoryId, LogLevel.DEBUG);

        InterfaceLeague? interfaceLeague =
            Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(belongsToLeagueCategoryId);
        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.CRITICAL);
            return nameof(interfaceLeague) + " was null!";
        }

        /*
        // Find the category fo the message ID
        var category = Database.Instance.Categories.CreatedCategoriesWithChannels.FirstOrDefault(
            x => x.Value.CategoryType == CategoryType.REGISTRATIONCATEGORY);

        if (category.Value == null)
        {
            Log.WriteLine(nameof(category) + " was null!", LogLevel.CRITICAL);
            return nameof(category) + " was null!";
        }

        Log.WriteLine("Found category: " + category.Value.CategoryType, LogLevel.DEBUG);

        var channel = category.Value.InterfaceChannels.FirstOrDefault(
                x => x.Value.ChannelType == ChannelType.LEAGUEREGISTRATION);

        if (channel.Value == null)
        {
            Log.WriteLine(nameof(channel) + " was null!", LogLevel.CRITICAL);
            return nameof(channel) + " was null!";
        }

        Log.WriteLine("Found channel: " + channel.Value.ChannelName, LogLevel.DEBUG);*/

        /*
        InterfaceMessage interfaceMessage = channel.Value.InterfaceMessagesWithIds.FirstOrDefault(
            x => x.Key == discordLeagueReferences.LeagueCategoryId.ToString()).Value;
       

        Log.WriteLine("Found messageId: " + interfaceMessage.MessageId, LogLevel.VERBOSE); */

        //await interfaceMessage.ModifyMessage(GenerateALeagueJoinButtonMessage());

        /*
        string? leagueEnumAttrValue =
            EnumExtensions.GetEnumMemberAttrValue(leagueCategoryName);

        Log.WriteLine(nameof(leagueEnumAttrValue) + ": " +
            leagueEnumAttrValue, LogLevel.VERBOSE); */

        string returned = "\n" + EnumExtensions.GetEnumMemberAttrValue(interfaceLeague.LeagueCategoryName) + "\n" +
            GetAllowedUnitsAsString(interfaceLeague) + "\n" +
            GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(interfaceLeague);

        Log.WriteLine(returned, LogLevel.DEBUG);

        return returned;
    }

    /*
    public async Task GenerateAndModifyTheMessageForLeagueRegistration(InterfaceLeague? _interfaceLeague)
    {
        if (_interfaceLeague == null)
        {
            Log.WriteLine(nameof(_interfaceLeague) + " was null!", LogLevel.CRITICAL);
            return;
        }

        await ModifyMessage("\n" + EnumExtensions.GetEnumMemberAttrValue(_interfaceLeague.LeagueCategoryName) + "\n" +
            GetAllowedUnitsAsString(_interfaceLeague) + "\n" +
            GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(_interfaceLeague));
    }*/

    private string GetAllowedUnitsAsString(InterfaceLeague _interfaceLeague)
    {
        string allowedUnits = string.Empty;

        for (int u = 0; u < _interfaceLeague.LeagueUnits.Count; ++u)
        {
            allowedUnits +=
                EnumExtensions.GetEnumMemberAttrValue(_interfaceLeague.LeagueUnits[u]);

            // Is not the last index
            if (u != _interfaceLeague.LeagueUnits.Count - 1)
            {
                allowedUnits += ", ";
            }
        }

        return allowedUnits;
    }

    private string GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(InterfaceLeague _interfaceLeague)
    {
        int count = 0;

        foreach (Team team in _interfaceLeague.LeagueData.Teams.TeamsList)
        {
            string teamName = team.GetTeamName(_interfaceLeague.LeaguePlayerCountPerTeam);

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

        if (_interfaceLeague.LeaguePlayerCountPerTeam > 1)
        {
            return "Teams: " + count;
        }
        else
        {
            return "Players: " + count;
        }
    }
}