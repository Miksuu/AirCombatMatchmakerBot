using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class LEAGUEREGISTRATIONBUTTON : BaseButton
{
    public LEAGUEREGISTRATIONBUTTON()
    {
        buttonName = ButtonName.LEAGUEREGISTRATIONBUTTON;
        buttonLabel = "Join";
        buttonStyle = ButtonStyle.Primary;
    }

    public void CreateTheButton(){}

    public override async Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        string responseMsg = "";

        Log.WriteLine("starting leagueRegistration", LogLevel.VERBOSE);

        string[] splitStrings = _component.Data.CustomId.Split('_');

        InterfaceLeague? interfaceLeague =
            Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(ulong.Parse(splitStrings[0]));
        if (interfaceLeague == null)
        {
            string errorMsg = nameof(interfaceLeague) + " was null! Could not find the league.";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
        }

        Log.WriteLine("found: " + nameof(interfaceLeague) +
            interfaceLeague.LeagueCategoryName, LogLevel.VERBOSE);

        ulong challengeChannelId = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            interfaceLeague.DiscordLeagueReferences.LeagueCategoryId).Value.FindInterfaceChannelWithNameInTheCategory(
            ChannelType.CHALLENGE).ChannelId;

        // Check that the player is in the PlayerData
        // (should be, he doesn't see this button before, except if hes admin)
        if (Database.Instance.PlayerData.CheckIfPlayerDataPlayerIDsContainsKey(
            _component.User.Id))
        {
            Player player = Database.Instance.PlayerData.GetAPlayerProfileById(
                _component.User.Id);
            if (player.PlayerDiscordId == 0)
            {
                string errorMsg = "Player's: " + player.PlayerNickName +" id was 0!";
                Log.WriteLine(errorMsg, LogLevel.CRITICAL);
                return (errorMsg, false);
            }

            Log.WriteLine("Found player: " + player.PlayerNickName +
                " (" + player.PlayerDiscordId + ")", LogLevel.VERBOSE);

            bool playerIsInATeamAlready = interfaceLeague.LeagueData.Teams.CheckIfPlayerIsAlreadyInATeamById(
                interfaceLeague.LeaguePlayerCountPerTeam, _component.User.Id);

            bool playerIsInActiveTeamAlready = interfaceLeague.LeagueData.Teams.CheckIfPlayersTeamIsActiveById(
                interfaceLeague.LeaguePlayerCountPerTeam, _component.User.Id);

            if (!playerIsInATeamAlready)
            {
                Log.WriteLine("The player was not found in any team in the league", LogLevel.VERBOSE);

                // Create a team with unique ID and increment that ID
                // after the data has been serialized
                Team newTeam = new Team(
                    new List<Player> { player },
                    player.PlayerNickName,
                    interfaceLeague.LeagueData.Teams.CurrentTeamInt);

                if (interfaceLeague.LeaguePlayerCountPerTeam < 2)
                {
                    Log.WriteLine("This league is solo", LogLevel.VERBOSE);

                    interfaceLeague.LeagueData.Teams.AddToListOfTeams(newTeam);

                    responseMsg = "Registration complete on: " + 
                        EnumExtensions.GetEnumMemberAttrValue(interfaceLeague.LeagueCategoryName) + "\n" +
                        " You can look for a match in: <#" + challengeChannelId + ">";
                }
                else
                {
                    // Not implemented yet
                    Log.WriteLine("This league is team based with number of players per team: " +
                        interfaceLeague.LeaguePlayerCountPerTeam, LogLevel.ERROR);
                    return ("", false);
                }

                // Add the role for the player for the specific league and set him teamActive
                UserManager.SetTeamActiveAndGrantThePlayerRole(
                    interfaceLeague, _component.User.Id);

                
                // Modify the message to have the new player count
                LEAGUEREGISTRATIONMESSAGE? leagueRegistrationMessage = _interfaceMessage as LEAGUEREGISTRATIONMESSAGE;
                if (leagueRegistrationMessage == null)
                {
                    string errorMsg = nameof(leagueRegistrationMessage) + " was null!";
                    Log.WriteLine(errorMsg, LogLevel.CRITICAL);
                    return (errorMsg, false);
                }

                await _interfaceMessage.ModifyMessage(leagueRegistrationMessage.GenerateMessageForSpecificCategoryLeague());

                Log.WriteLine("Done creating team: " + newTeam + " team count is now: " +
                    interfaceLeague.LeagueData.Teams.TeamsList.Count, LogLevel.DEBUG);

                interfaceLeague.LeagueData.Teams.IncrementCurrentTeamInt();
            }
            else if (playerIsInATeamAlready && !playerIsInActiveTeamAlready)
            {
                // Need to handle team related behaviour better later

                Log.WriteLine("The player was already in a team in that league!" +
                    " Setting him active", LogLevel.DEBUG);

                UserManager.SetTeamActiveAndGrantThePlayerRole(
                    interfaceLeague, _component.User.Id);

                responseMsg = "You have rejoined: " +
                    EnumExtensions.GetEnumMemberAttrValue(interfaceLeague.LeagueCategoryName) + "\n" +
                    " You can look for a match in: <#" + challengeChannelId + ">";

                LEAGUEREGISTRATIONMESSAGE? leagueRegistrationMessage = _interfaceMessage as LEAGUEREGISTRATIONMESSAGE;
                if (leagueRegistrationMessage == null)
                {
                    string errorMsg = nameof(leagueRegistrationMessage) + " was null!";
                    Log.WriteLine(errorMsg, LogLevel.CRITICAL);
                    return (errorMsg, false);
                }

                await _interfaceMessage.ModifyMessage(leagueRegistrationMessage.GenerateMessageForSpecificCategoryLeague());
            }
            else if (playerIsInATeamAlready && playerIsInActiveTeamAlready)
            {
                Log.WriteLine("Player " + player.PlayerDiscordId + " tried to join: " + interfaceLeague.LeagueCategoryName +
                    ", had a team already active", LogLevel.VERBOSE);
                responseMsg = "You are already part of " + EnumExtensions.GetEnumMemberAttrValue(interfaceLeague.LeagueCategoryName) +
                    "\n" + " You can look for a match in: <#" + challengeChannelId + ">";
                return (responseMsg, false);
            }
        }
        else
        {
            responseMsg = "Error joining the league! Press the register button first!" +
                " (only admins should be able to see this)";
            Log.WriteLine("Player: " + _component.User.Id +
                " (" + _component.User.Username + ")" +
                " tried to join a league before registering", LogLevel.WARNING);
            return (responseMsg, false);
        }

        interfaceLeague.UpdateLeagueLeaderboard();

        return (responseMsg, true);
    }
}