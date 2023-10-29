﻿using System.Data;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class LEAGUESTATUSMESSAGE : BaseMessage
{
    public LEAGUESTATUSMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.LEAGUESTATUSMESSAGE;
        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>
        {
        };
        thisInterfaceMessage.MessageEmbedTitle = "Leaderboard:\n"; ;
        thisInterfaceMessage.MessageDescription = "";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _channelCategoryId)
    {
        base.GenerateRegularButtons(_component, _channelCategoryId);
    }

    public override Task<MessageComponents> GenerateMessage(ulong _channelCategoryId = 0)
    {
        string finalMessage = string.Empty;
        List<Team> sortedTeamListByElo = new List<Team>();

        InterfaceLeague interfaceLeague =
            Database.GetInstance<ApplicationDatabase>().Leagues.GetILeagueByCategoryId(thisInterfaceMessage.MessageCategoryId);

        sortedTeamListByElo =
            new List<Team>(interfaceLeague.LeagueData.Teams.TeamsConcurrentBag.OrderByDescending(
                x => x.SkillRating));

        foreach (Team team in sortedTeamListByElo)
        {
            finalMessage += "[" + team.SkillRating + "] " + team.TeamName + " | " + team.GetTeamStats() + "\n";
        }
        if (sortedTeamListByElo.Count > 0)
        {
            Log.WriteLine("Generated the leaderboard (" + sortedTeamListByElo.Count + "): " + finalMessage);
        }
        else
        {
            Log.WriteLine("Generated the leaderboard: " + finalMessage);
        }

        return Task.FromResult(finalMessage);
    }

    public override string GenerateMessageFooter()
    {
        return "";
    }
}