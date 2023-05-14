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
    MatchChannelComponents mcc = new MatchChannelComponents();
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

        mcc.FindMatchAndItsLeagueAndInsertItToTheCache(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            return nameof(mcc) + " was null!";
        }

        foreach (var teamKvp in mcc.leagueMatchCached.TeamsInTheMatch)
        {
            Team team = mcc.interfaceLeagueCached.LeagueData.Teams.FindTeamById(
                mcc.interfaceLeagueCached.LeaguePlayerCountPerTeam, teamKvp.Key);

            string teamMembers =
                team.GetTeamInAString(
                    true, mcc.interfaceLeagueCached.LeaguePlayerCountPerTeam);

            generatedMessage += teamMembers;

            if (firstTeam) { generatedMessage += " vs "; firstTeam = false; }
        }

        return generatedMessage;
    }
}