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

    public override string GenerateMessage()
    {
        if (thisInterfaceMessage.MessageDescription == null)
        {
            Log.WriteLine("MessageDescription was null!", LogLevel.CRITICAL);
            return "MessageDescription was null!";
        }

        return thisInterfaceMessage.MessageDescription;
    }

    public string GenerateMessageForSpecificCategoryLeague()
    {
        Log.WriteLine("Starting to generate the league registration message with: " +
            belongsToLeagueCategoryId, LogLevel.DEBUG);

        InterfaceLeague interfaceLeague =
            Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(belongsToLeagueCategoryId);

        thisInterfaceMessage.MessageEmbedTitle = EnumExtensions.GetEnumMemberAttrValue(interfaceLeague.LeagueCategoryName);

        string returned =
            GetAllowedUnitsAsString(interfaceLeague) + "\n" +
            GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(interfaceLeague);

        Log.WriteLine(returned, LogLevel.VERBOSE);

        return returned;
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
                    " is active, increased count to: " + count, LogLevel.VERBOSE);
            }
            else
            {
                Log.WriteLine("team: " + TeamName + " is not active", LogLevel.VERBOSE);
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