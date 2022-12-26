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
                    await UserManager.HandleUserRegisterationToCache(_component.User.Id);

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
                string responseMsg = "";

                Log.WriteLine("starting leagueRegistration", LogLevel.VERBOSE);

                var findLeagueCategoryType
                    = Database.Instance.StoredLeagues.First(x => x.LeagueCategoryName.ToString() == splitString[1]);
                CategoryName leagueCategoryName = findLeagueCategoryType.LeagueCategoryName;

                var leagueInterface = LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueCategoryName);

                Log.WriteLine("leagueInterface: " + leagueInterface,LogLevel.VERBOSE);

                Log.WriteLine("Found " + nameof(leagueInterface) + ": " + leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

                ILeague? dbLeagueInstance = LeagueManager.FindLeagueAndReturnInterfaceFromDatabase(leagueInterface);

                if (dbLeagueInstance == null)
                {
                    Log.WriteLine(nameof(dbLeagueInstance) + " was null! Could not find the league.", LogLevel.CRITICAL);
                    return;
                }

                Log.WriteLine("found: " + nameof(dbLeagueInstance) + dbLeagueInstance.LeagueCategoryName, LogLevel.VERBOSE);

                // Check that the player is in the PlayerData (should be, he doesn't see this button before, except if hes admin)
                if (Database.Instance.PlayerData.PlayerIDs.ContainsKey(_component.User.Id))
                {
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
                }
                else
                {
                    responseMsg = "Error joining the league! Press the register button first!" +
                        " (only admins should be able to see this)";
                    Log.WriteLine("Player: " + _component.User.Id + "(" + _component.User.Username + ")" +
                        " tried to join a league before registering", LogLevel.WARNING);
                }

                await _component.RespondAsync(responseMsg, ephemeral: true);

                break;
            default:
                response = "Something went wrong with the button press!";
                logLevel = LogLevel.ERROR;
                break;
        }

        Log.WriteLine("Before serialization on ButtonHandler", LogLevel.VERBOSE);

        await SerializationManager.SerializeDB();


        if (splitString[0] != "leagueRegistration")
        {
            Log.WriteLine(response, logLevel);
        }
        if (response != "EMPTY") await _component.RespondAsync(response, ephemeral: true);
        else { Log.WriteLine("the response was: " + response, LogLevel.CRITICAL); }
    }
}