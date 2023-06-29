using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class LEAGUEREGISTRATIONMESSAGE : BaseMessage
{
    public LEAGUEREGISTRATIONMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.LEAGUEREGISTRATIONMESSAGE;

        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.LEAGUEREGISTRATIONBUTTON, 1),
            });

        thisInterfaceMessage.MessageDescription = "Insert league registration message here";
    }

    [DataMember] public ulong belongsToLeagueCategoryId;

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override Task<string> GenerateMessage()
    {
        if (thisInterfaceMessage.MessageDescription == null)
        {
            Log.WriteLine("MessageDescription was null!", LogLevel.CRITICAL);
            return Task.FromResult("MessageDescription was null!");
        }

        return Task.FromResult(thisInterfaceMessage.MessageDescription);
    }

    public async Task<string> GenerateMessageForSpecificCategoryLeague()
    {
        Log.WriteLine("Starting to generate the league registration message with: " +
            belongsToLeagueCategoryId, LogLevel.DEBUG);

        InterfaceLeague interfaceLeague =
            Database.Instance.Leagues.GetILeagueByCategoryId(belongsToLeagueCategoryId);

        thisInterfaceMessage.MessageEmbedTitle = EnumExtensions.GetEnumMemberAttrValue(interfaceLeague.LeagueCategoryName);

        string channelJumpLinks = string.Empty;

        var category = Database.Instance.Categories.FindInterfaceCategoryWithId(belongsToLeagueCategoryId);

        string returned =
            GetAllowedUnitsAsString(interfaceLeague) + "\n" +
            GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(interfaceLeague) + "\n\n" +
            await GetCategoryJumpUrl(category, ChannelType.MATCHSCHEDULERCHANNEL, MessageName.MATCHSCHEDULERSTATUSMESSAGE) + "\n" +
            await GetCategoryJumpUrl(category, ChannelType.CHALLENGE, MessageName.CHALLENGEMESSAGE); ;

        Log.WriteLine(returned);

        return returned;
    }

    private async Task<string> GetCategoryJumpUrl(InterfaceCategory _interfaceCategory, ChannelType _channelType, MessageName _messageName)
    {
        var channelId = _interfaceCategory.FindInterfaceChannelWithNameInTheCategory(_channelType).ChannelId;

        return await Database.Instance.Categories.GetMessageJumpUrl(belongsToLeagueCategoryId, channelId, _messageName);
    }

    private string GetAllowedUnitsAsString(InterfaceLeague _interfaceLeague)
    {
        string allowedUnits = string.Empty;

        for (int u = 0; u < _interfaceLeague.LeagueUnits.Count; ++u)
        {
            allowedUnits +=
                EnumExtensions.GetEnumMemberAttrValue(_interfaceLeague.LeagueUnits.ElementAt(u));

            // Is not the last index
            if (u != _interfaceLeague.LeagueUnits.Count - 1)
            {
                allowedUnits += ", ";
            }
        }

        Log.WriteLine("Allowed units: " + allowedUnits, LogLevel.DEBUG);

        return allowedUnits;
    }

    private string GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(InterfaceLeague _interfaceLeague)
    {
        int count = 0;

        foreach (Team team in _interfaceLeague.LeagueData.Teams.TeamsConcurrentBag)
        {
            string TeamName = team.GetTeamName(_interfaceLeague.LeaguePlayerCountPerTeam);

            if (team.TeamActive)
            {
                count++;
                Log.WriteLine("team: " + TeamName +
                    " is active, increased count to: " + count);
            }
            else
            {
                Log.WriteLine("team: " + TeamName + " is not active");
            }
        }

        Log.WriteLine("Total count: " + count);

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