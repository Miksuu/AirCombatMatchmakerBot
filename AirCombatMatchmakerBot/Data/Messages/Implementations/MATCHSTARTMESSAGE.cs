using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;

[DataContract]
public class MATCHSTARTMESSAGE : BaseMessage
{
    public MATCHSTARTMESSAGE()
    {
        messageName = MessageName.MATCHSTARTMESSAGE;
        thisInterfaceMessage.MessageDescription = "Insert the match start message here";
        mentionMatchPlayers = true;
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override string GenerateMessage()
    {
        string generatedMessage = "";
        bool firstTeam = true;

        Log.WriteLine("Starting to generate match initiation message on channel: with id: " +
            thisInterfaceMessage.MessageChannelId + " under category ID: " + thisInterfaceMessage.MessageCategoryId, LogLevel.VERBOSE);

        InterfaceLeague? interfaceLeague =
            Database.Instance.Leagues.GetILeagueByCategoryId(thisInterfaceMessage.MessageCategoryId);
        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.ERROR);
            return "";
        }

        LeagueMatch? foundMatch =
            interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(thisInterfaceMessage.MessageChannelId);
        if (foundMatch == null)
        {
            Log.WriteLine(nameof(foundMatch) + " was null!", LogLevel.ERROR);
            return "";
        }

        foreach (var teamKvp in foundMatch.TeamsInTheMatch)
        {
            Team team = interfaceLeague.LeagueData.Teams.FindTeamById(
                interfaceLeague.LeaguePlayerCountPerTeam, teamKvp.Key);

            string teamMembers =
                team.GetTeamInAString(
                    true, interfaceLeague.LeaguePlayerCountPerTeam);

            generatedMessage += teamMembers;

            if (firstTeam) { generatedMessage += " vs "; firstTeam = false; }
        }

        return generatedMessage;
    }
}