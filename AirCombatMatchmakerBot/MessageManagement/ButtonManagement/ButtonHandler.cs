using Discord;
using Discord.WebSocket;
using System.ComponentModel;
using System.Linq;

public static class ButtonHandler
{
    public static async Task HandleButtonPress(SocketMessageComponent _component)
    {
        Log.WriteLine(_component.Data.CustomId, LogLevel.DEBUG);

        
        // Splits the button press action and the user ID
        string[] splitString = _component.Data.CustomId.Split('_');

        Log.WriteLine("Button press detected by: " + _component.User.Id + " | splitStrings: " +
            splitString[0] + " | " + splitString[1], LogLevel.DEBUG);
        
        string response = "EMPTY";
        LogLevel logLevel = LogLevel.DEBUG;
        // Checks with first element of the split string (action)
        switch (splitString[0])
        {
            case "mainRegistration":
                // Checks that the player does not exist in the database already, true if this is not the case
                if (UserManager.AddNewPlayerToTheDatabaseById(_component.User.Id).Result)
                {                    
                    response = _component.User.Mention + ", " +
                        BotMessaging.GetMessageResponse(
                            _component.Data.CustomId,
                            " registration complete, welcome!",
                            _component.Channel.Name);
                }
                else
                {
                    response = _component.User.Mention + ", " +
                        BotMessaging.GetMessageResponse(
                            _component.Data.CustomId,
                            " You are already registered!",
                            _component.Channel.Name);
                }
                break;
            case "leagueRegistration":
                /*
                ILeague leagueInterface = LeagueManager.GetLeagueInstance(splitString[1]);

                Log.WriteLine("Found " + nameof(leagueInterface) + ": " + leagueInterface.LeagueName, LogLevel.VERBOSE);

                ILeague? dbLeagueInstance = LeagueManager.FindLeagueAndReturnInterfaceFromDatabase(leagueInterface);

                if (dbLeagueInstance == null)
                {
                    Log.WriteLine(nameof(dbLeagueInstance) + " was null! Could not find the league.", LogLevel.CRITICAL);
                    return;
                }

                Player player = Database.Instance.PlayerData.PlayerIDs[_component.User.Id];

                if (player.playerDiscordId == 0)
                {
                    Log.WriteLine("Player's: " + player.playerNickName + " id was 0!", LogLevel.CRITICAL);
                    return;
                }

                Log.WriteLine("Found player: " + player.playerNickName + " (" + player.playerDiscordId + ")", LogLevel.VERBOSE);

                if (!LeagueManager.CheckIfPlayerIsAlreadyInATeamById(dbLeagueInstance.LeagueData.Teams, _component.User.Id))
                {
                    Log.WriteLine("The player was not found in any team in the league", LogLevel.VERBOSE);

                    // Create a team with unique ID and increment that ID after the data has been serialized
                    Team newTeam = new Team(
                        new List<Player> { player },
                        player.playerNickName,
                        dbLeagueInstance.LeagueData.currentTeamInt); 

                    if (dbLeagueInstance.LeaguePlayerCountPerTeam < 2)
                    {
                        Log.WriteLine("This league is solo", LogLevel.VERBOSE);

                        dbLeagueInstance.LeagueData.Teams.Add(newTeam);

                        Log.WriteLine("Done adding the team. Count is now: " +
                            dbLeagueInstance.LeagueData.Teams.Count, LogLevel.VERBOSE);
                    }
                    else
                    {
                        // Not implemented yet
                        Log.WriteLine("This league is team based with number of players per team: " +
                            dbLeagueInstance.LeaguePlayerCountPerTeam, LogLevel.ERROR);
                        return;
                    }

                    // Add the role for the player for the specific league and set him active
                    UserManager.SetPlayerActiveAndGrantHimTheRole(dbLeagueInstance, _component.User.Id);

                    // Modify the message to have the new player count
                    await MessageManager.ModifyLeagueRegisterationChannelMessage(dbLeagueInstance);

                    Log.WriteLine("Done creating team: " + newTeam + " team count is now: " +
                        dbLeagueInstance.LeagueData.Teams.Count, LogLevel.DEBUG);

                    dbLeagueInstance.LeagueData.currentTeamInt++;
                    await SerializationManager.SerializeDB();
                }
                else
                {
                    // Need to handle team related behaviour better later

                    Log.WriteLine("The player was already in a team in that league! Setting him active", LogLevel.DEBUG);

                    UserManager.SetPlayerActiveAndGrantHimTheRole(dbLeagueInstance, _component.User.Id);

                    await MessageManager.ModifyLeagueRegisterationChannelMessage(dbLeagueInstance);
                }

                await _component.RespondAsync(ephemeral: true);

                break; */
            default:
                response = "Something went wrong with the button press!";
                logLevel = LogLevel.ERROR;
                break;
        }

        Log.WriteLine("Before serialization on ButtonHandler", LogLevel.VERBOSE);

        await SerializationManager.SerializeDB();

        /*
        if (splitString[0] != "leagueRegisteration")
        {*/
            Log.WriteLine(response, logLevel);
        if (response != "EMPTY") await _component.RespondAsync(response, ephemeral: true);
        //}
    }
}