using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class LEAGUEREGISTRATIONMESSAGE : BaseMessage
{
    public LEAGUEREGISTRATIONMESSAGE()
    {
        messageName = MessageName.LEAGUEREGISTRATIONMESSAGE;

        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.LEAGUEREGISTRATIONBUTTON, 1),
            });

        messageDescription = "Insert league registration message here";
    }

    [DataMember] public ulong belongsToLeagueCategoryId;

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override string GenerateMessage()
    {
        return messageDescription;
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

        messageEmbedTitle = EnumExtensions.GetEnumMemberAttrValue(interfaceLeague.LeagueCategoryName);

        string returned =
            GetAllowedUnitsAsString(interfaceLeague) + "\n" +
            GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(interfaceLeague);

        Log.WriteLine(returned, LogLevel.VERBOSE);

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