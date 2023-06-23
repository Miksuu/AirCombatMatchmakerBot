using Discord;
using System.Runtime.Serialization;

[DataContract]
public class MATCHSTARTMESSAGE : BaseMessage
{
    MatchChannelComponents mcc;
    public MATCHSTARTMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.MATCHSTARTMESSAGE;
        thisInterfaceMessage.MessageDescription = "Insert the match start message here";
        mentionMatchPlayers = true;
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override Task<string> GenerateMessage()
    {
        string generatedMessage = "";
        bool firstTeam = true;

        Log.WriteLine("Starting to generate match initiation message on channel: with id: " +
            thisInterfaceMessage.MessageChannelId + " under category ID: " +
            thisInterfaceMessage.MessageCategoryId, LogLevel.VERBOSE);

        mcc = new MatchChannelComponents(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            return Task.FromResult(nameof(mcc) + " was null!");
        }

        foreach (var teamKvp in mcc.leagueMatchCached.TeamsInTheMatch)
        {
            try
            {
                Team team = mcc.interfaceLeagueCached.LeagueData.Teams.FindTeamById(teamKvp.Key);

                string teamMembers =
                    team.GetTeamInAString(
                        true, mcc.interfaceLeagueCached.LeaguePlayerCountPerTeam);

                generatedMessage += teamMembers;

                if (firstTeam) { generatedMessage += " vs "; firstTeam = false; }
            }
            catch(Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                continue;
            }
        }

        return Task.FromResult(generatedMessage);
    }
}