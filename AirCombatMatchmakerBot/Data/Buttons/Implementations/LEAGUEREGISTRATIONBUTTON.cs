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

    public override async Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, ulong _channelId,
        ulong _messageId, string _message, string[] _splitStrings)
    {
        string responseMsg = "";

        Log.WriteLine("starting leagueRegistration", LogLevel.VERBOSE);

        InterfaceLeague? interfaceLeague =
            Database.Instance.Leagues.FindLeagueInterfaceWithCategoryNameString(_splitStrings[1]);

        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return "";
        }

        Log.WriteLine("found: " + nameof(interfaceLeague) +
            interfaceLeague.LeagueCategoryName, LogLevel.VERBOSE);

        // Check that the player is in the PlayerData
        // (should be, he doesn't see this button before, except if hes admin)
        if (Database.Instance.PlayerData.CheckIfPlayerDataPlayerIDsContainsKey(
            _component.User.Id))
        {
            Player player = Database.Instance.PlayerData.GetAPlayerProfileById(
                _component.User.Id);

            if (player.PlayerDiscordId == 0)
            {
                Log.WriteLine("Player's: " + player.PlayerNickName +
                    " id was 0!", LogLevel.CRITICAL);
                return "";
            }

            Log.WriteLine("Found player: " + player.PlayerNickName +
                " (" + player.PlayerDiscordId + ")", LogLevel.VERBOSE);

            if (!interfaceLeague.LeagueData.Teams.CheckIfPlayerIsAlreadyInATeamById(
                interfaceLeague.LeaguePlayerCountPerTeam, _component.User.Id))
            {
                Log.WriteLine(
                    "The player was not found in any team in the league", LogLevel.VERBOSE);

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
                }
                else
                {
                    // Not implemented yet
                    Log.WriteLine("This league is team based with number of players per team: " +
                        interfaceLeague.LeaguePlayerCountPerTeam, LogLevel.ERROR);
                    return "";
                }

                // Add the role for the player for the specific league and set him teamActive
                UserManager.SetTeamActiveAndGrantThePlayerRole(
                    interfaceLeague, _component.User.Id);

                // Modify the message to have the new player count
                await interfaceLeague.ModifyLeagueRegisterationChannelMessage();


                Log.WriteLine("Done creating team: " + newTeam + " team count is now: " +
                    interfaceLeague.LeagueData.Teams.TeamsList.Count, LogLevel.DEBUG);

                interfaceLeague.LeagueData.Teams.IncrementCurrentTeamInt();
                await SerializationManager.SerializeDB();
            }
            else
            {
                // Need to handle team related behaviour better later

                Log.WriteLine("The player was already in a team in that league!" +
                    " Setting him active", LogLevel.DEBUG);

                UserManager.SetTeamActiveAndGrantThePlayerRole(
                    interfaceLeague, _component.User.Id);

                await interfaceLeague.ModifyLeagueRegisterationChannelMessage();
            }
        }
        else
        {
            responseMsg = "Error joining the league! Press the register button first!" +
                " (only admins should be able to see this)";
            Log.WriteLine("Player: " + _component.User.Id +
                " (" + _component.User.Username + ")" +
                " tried to join a league before registering", LogLevel.WARNING);
        }

        return responseMsg;
    }
}