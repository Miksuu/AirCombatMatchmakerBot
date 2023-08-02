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

    public async override Task<string> GenerateMessage(ulong _messageCategoryId = 0)
    {
        Log.WriteLine("Starting to generate the league registration message with: " +
            belongsToLeagueCategoryId, LogLevel.DEBUG);

        belongsToLeagueCategoryId = _messageCategoryId;

        InterfaceLeague interfaceLeague =
            Database.Instance.Leagues.GetILeagueByCategoryId(belongsToLeagueCategoryId);

        thisInterfaceMessage.MessageEmbedTitle = EnumExtensions.GetEnumMemberAttrValue(interfaceLeague.LeagueCategoryName);

        var category = DiscordBotDatabase.Instance.Categories.FindInterfaceCategoryWithCategoryId(belongsToLeagueCategoryId);

        string returned =
            GetAllowedUnitsAsString(interfaceLeague) + "\n" +
            GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(interfaceLeague) + "\n\n" +
            await GetChannelSpecificJumpUrl(category, ChannelType.MATCHSCHEDULERCHANNEL, MessageName.MATCHSCHEDULERSTATUSMESSAGE) + "\n" +
            await GetChannelSpecificJumpUrl(category, ChannelType.CHALLENGE, MessageName.CHALLENGEMESSAGE); ;

        Log.WriteLine(returned);

        return returned;
    }

    private async Task<string> GetChannelSpecificJumpUrl(InterfaceCategory _interfaceCategory, ChannelType _channelType, MessageName _messageName)
    {
        var channelId = _interfaceCategory.FindInterfaceChannelWithNameInTheCategory(_channelType).ChannelId;

        return await DiscordBotDatabase.Instance.Categories.GetMessageJumpUrl(belongsToLeagueCategoryId, channelId, _messageName);
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

    public override string GenerateMessageFooter()
    {
        return "";
        //return "Last updated at: " + DateTime.UtcNow.ToLongTimeString() + " " + DateTime.UtcNow.ToLongDateString() + " (GMT+0)";
    }
}