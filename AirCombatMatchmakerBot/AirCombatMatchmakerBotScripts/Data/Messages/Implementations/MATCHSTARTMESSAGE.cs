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

    protected override void GenerateButtons(ComponentBuilder _component, ulong _channelCategoryId)
    {
        base.GenerateRegularButtons(_component, _channelCategoryId);
    }

    public override Task<string> GenerateMessage(ulong _channelCategoryId = 0)
    {
        string generatedMessage = "";
        bool firstTeam = true;

        Log.WriteLine("Starting to generate match initiation message on channel: with id: " +
            thisInterfaceMessage.MessageChannelId + " under category ID: " +
            thisInterfaceMessage.MessageCategoryId);

        mcc = new MatchChannelComponents(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.ERROR);
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
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.ERROR);
                continue;
            }
        }

        return Task.FromResult(generatedMessage);
    }

    public override string GenerateMessageFooter()
    {
        return "";
    }
}