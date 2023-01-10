using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class MATCHSTARTMESSAGE : BaseMessage
{
    public MATCHSTARTMESSAGE()
    {
        messageName = MessageName.MATCHSTARTMESSAGE;
        message = "Insert the match start message here";
    }

    public override string GenerateMessage(ulong _channelId, ulong _channelCategoryId)
    {
        string generatedMessage = "";
        bool firstTeam = true;

        Log.WriteLine("Starting to generate match initiation message on channel: with id: " +
            _channelId + " under category ID: " + _channelCategoryId, LogLevel.VERBOSE);

        InterfaceLeague? interfaceLeague =
            Database.Instance.Leagues.GetILeagueByCategoryId(_channelCategoryId);
        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.ERROR);
            return "";
        }

        LeagueMatch? foundMatch =
            interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(_channelId);

        if (foundMatch == null)
        {
            Log.WriteLine(nameof(foundMatch) + " was null!", LogLevel.ERROR);
            return "";
        }

        foreach (int teamId in foundMatch.TeamsInTheMatch)
        {
            Team team = interfaceLeague.LeagueData.Teams.FindTeamById(
                interfaceLeague.LeaguePlayerCountPerTeam, teamId);

            string teamMembers =
                team.GetTeamSkillRatingAndNameInAString(
                    interfaceLeague.LeaguePlayerCountPerTeam);

            generatedMessage += teamMembers;

            if (firstTeam) { generatedMessage += " vs "; firstTeam = false; }
        }

        return generatedMessage;
    }
}