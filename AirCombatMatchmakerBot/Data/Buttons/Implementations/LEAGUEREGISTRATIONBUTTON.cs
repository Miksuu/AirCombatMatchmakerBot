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

    public override async Task<string> ActivateButtonFunction(SocketMessageComponent _component, string _splitString)
    {
        string responseMsg = "";

        Log.WriteLine("starting leagueRegistration", LogLevel.VERBOSE);

        InterfaceLeague? dbLeagueInstance = ButtonFunctionality.FindLeagueInterfaceWithSplitStringPart(_splitString);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return "";
        }

        Log.WriteLine("found: " + nameof(dbLeagueInstance) +
            dbLeagueInstance.LeagueCategoryName, LogLevel.VERBOSE);

        // Check that the player is in the PlayerData
        // (should be, he doesn't see this button before, except if hes admin)
        if (Database.Instance.PlayerData.CheckIfPlayerDataPlayerIDsContainsKey(_component.User.Id))
        {
            Player player = Database.Instance.PlayerData.GetAPlayerProfileById(_component.User.Id);

            if (player.GetPlayerDiscordId() == 0)
            {
                Log.WriteLine("Player's: " + player.GetPlayerNickname() +
                    " id was 0!", LogLevel.CRITICAL);
                return "";
            }

            Log.WriteLine("Found player: " + player.GetPlayerNickname() +
                " (" + player.GetPlayerDiscordId() + ")", LogLevel.VERBOSE);

            if (!dbLeagueInstance.LeagueData.Teams.CheckIfPlayerIsAlreadyInATeamById(
                _component.User.Id))
            {
                Log.WriteLine(
                    "The player was not found in any team in the league", LogLevel.VERBOSE);

                // Create a team with unique ID and increment that ID
                // after the data has been serialized
                Team newTeam = new Team(
                    new List<Player> { player },
                    player.GetPlayerNickname(),
                    dbLeagueInstance.LeagueData.Teams.CurrentTeamInt);

                if (dbLeagueInstance.LeaguePlayerCountPerTeam < 2)
                {
                    Log.WriteLine("This league is solo", LogLevel.VERBOSE);

                    dbLeagueInstance.LeagueData.Teams.AddToListOfTeams(newTeam);
                }
                else
                {
                    // Not implemented yet
                    Log.WriteLine("This league is team based with number of players per team: " +
                        dbLeagueInstance.LeaguePlayerCountPerTeam, LogLevel.ERROR);
                    return "";
                }

                // Add the role for the player for the specific league and set him teamActive
                UserManager.SetPlayerActiveAndGrantHimTheRole(
                    dbLeagueInstance, _component.User.Id);

                // Modify the message to have the new player count
                await dbLeagueInstance.ModifyLeagueRegisterationChannelMessage();


                Log.WriteLine("Done creating team: " + newTeam + " team count is now: " +
                    dbLeagueInstance.LeagueData.Teams.TeamsList.Count, LogLevel.DEBUG);

                dbLeagueInstance.LeagueData.Teams.IncrementCurrentTeamInt();
                await SerializationManager.SerializeDB();
            }
            else
            {
                // Need to handle team related behaviour better later

                Log.WriteLine("The player was already in a team in that league!" +
                    " Setting him active", LogLevel.DEBUG);

                UserManager.SetPlayerActiveAndGrantHimTheRole(
                    dbLeagueInstance, _component.User.Id);

                await dbLeagueInstance.ModifyLeagueRegisterationChannelMessage();
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