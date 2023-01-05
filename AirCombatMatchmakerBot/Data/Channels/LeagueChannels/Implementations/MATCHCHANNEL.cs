using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Reflection.Metadata.Ecma335;

[DataContract]
public class MATCHCHANNEL : BaseChannel
{
    public MATCHCHANNEL()
    {
        channelType = ChannelType.MATCHCHANNEL;
        channelMessages = new List<MessageName>
        {
            //MessageName.MATCHSTARTMESSAGE,
            MessageName.REPORTINGMESSAGE,
        };
    }

    public override List<Overwrite> GetGuildPermissions(
        SocketGuild _guild, params ulong[] _allowedUsersIdsArray)
    {
        List<Overwrite> listOfOverwrites = new List<Overwrite>();

        Log.WriteLine("Overwriting permissions for: " + channelName +
            " users that will be allowed access count: " +
            _allowedUsersIdsArray.Length, LogLevel.VERBOSE);

        listOfOverwrites.Add(new Overwrite(_guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)));

        foreach (ulong userId in _allowedUsersIdsArray)
        {
            Log.WriteLine("Adding " + userId + " to the permission allowed list on: " +
                channelName, LogLevel.VERBOSE);

            listOfOverwrites.Add(
                new Overwrite(userId, PermissionTarget.User,
                    new OverwritePermissions(viewChannel: PermValue.Allow)));
        }

        return listOfOverwrites;
    }

    public override async Task PrepareChannelMessages()
    {
        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        InterfaceChannel databaseInterfaceChannel =
            await base.PrepareCustomChannelMessages(GenerateMatchInitiationMessage());

        await base.PostChannelMessages(guild, databaseInterfaceChannel);
    }

    private string? GenerateMatchInitiationMessage()
    {
        string generatedMessage = "";
        bool firstTeam = true;

        Log.WriteLine("Starting to generate match initiation message on channel: " +
            channelName + " with id: " + channelId + " under category ID: " +
            channelsCategoryId, LogLevel.VERBOSE);

        InterfaceLeague interfaceLeague =
            Database.Instance.Leagues.GetILeagueByCategoryId(channelsCategoryId);

        LeagueMatch? foundMatch = 
            interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(channelId);

        if (foundMatch == null)
        {
            Log.WriteLine(nameof(foundMatch) + " was null!", LogLevel.ERROR);
            return null;
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

    /*
        public Task PostChannelMessages()
    {
        return Task.CompletedTask;
    }*/
}