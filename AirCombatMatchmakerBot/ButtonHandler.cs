﻿using Discord;
using Discord.WebSocket;
using System.ComponentModel;
using System.Linq;

public static class ButtonHandler
{
    public static async Task HandleButtonPress(SocketMessageComponent _component)
    {
        // Splits the button press action and the user ID
        string[] splitString = _component.Data.CustomId.Split('_');

        Log.WriteLine("Button press detected by: " + _component.User.Id + " | splitStrings: " +
            splitString[0] + " | " + splitString[1], LogLevel.DEBUG);

        string response = "EMPTY";
        LogLevel logLevel = LogLevel.DEBUG;
        // Checks with first element of the split string (action)
        switch (splitString[0])
        {
            // Player registeration, 2nd part of split string is hes ID
            case "registeration":
                // Check that the button is the user's one
                if (_component.User.Id.ToString() == splitString[1])
                {
                    // Checks that the player does not exist in the database already, true if this is not the case
                    if (UserManager.AddNewPlayerToTheDatabaseById(_component.User.Id).Result)
                    {
                        await ChannelManager.DeleteUsersRegisterationChannel(_component.User.Id);
                        /*
                        response = _component.User.Mention + ", " +
                            BotMessaging.GetMessageResponse(
                                _component.Data.CustomId,
                                " registeration complete, welcome! \n" +
                                "This channel will close soon.",
                                _component.Channel.Name); */
                    }
                    // This should not be the case, the registeration channel should not be available for the user
                    // TO DO: Also remember to remove the button!!
                    else
                    {
                        response = _component.User.Mention + ", " +
                            BotMessaging.GetMessageResponse(
                                _component.Data.CustomId,
                                " You are already registered \n" +
                                "one of our admins has been informed about a possible bug in the program.",
                                _component.Channel.Name);

                        // Admin warning
                        logLevel = LogLevel.WARNING;
                    }
                }
                else
                {
                    response = _component.User.Mention + ", " +
                        BotMessaging.GetMessageResponse(
                            _component.Data.CustomId,
                            " that's not your button!",
                            _component.Channel.Name);
                }
                break;
            case "leagueRegisteration":
                var leagueInstance = ClassExtensions.GetInstance(splitString[1]);
                ILeague leagueInterface = (ILeague)leagueInstance;

                Log.WriteLine("Found " + nameof(leagueInterface) + ": " + leagueInterface.LeagueName, LogLevel.VERBOSE);

                ILeague? dbLeagueInstance = LeagueManager.FindLeagueAndReturnInterfaceFromDatabase(leagueInterface);

                if (dbLeagueInstance != null)
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

                        Team newTeam = new Team(new List<Player> { player }, player.playerNickName);
                        newTeam.active = true;

                        if (dbLeagueInstance.LeaguePlayerCountPerTeam < 2)
                        {
                            Log.WriteLine("This league is solo", LogLevel.VERBOSE);

                            dbLeagueInstance.LeagueData.Teams.Add(newTeam);

                            Log.WriteLine("Done adding the team. Count is now: " +
                                dbLeagueInstance.LeagueData.Teams.Count,LogLevel.VERBOSE);


                            // Modify the message to have the new player count
                            await BotMessaging.ModifyMessage(1049555859656671232,
                                dbLeagueInstance.LeagueData.leagueChannelMessageId, 
                                LeagueManager.GenerateALeagueJoinButtonMessage(dbLeagueInstance));
                        }
                        else
                        {
                            // Not implemented yet
                            Log.WriteLine("This league is team based with number of players per team: " +
                                dbLeagueInstance.LeaguePlayerCountPerTeam, LogLevel.ERROR);
                        }

                        Log.WriteLine("Done creating team: " + newTeam + " team count is now: " +
                            dbLeagueInstance.LeagueData.Teams.Count, LogLevel.DEBUG);
                    }
                    else
                    {
                        Log.WriteLine("The player was already in a team in that league!", LogLevel.VERBOSE);

                        // Do some answer to the player
                    }
                }
                else Log.WriteLine(nameof(dbLeagueInstance) + " was null! Could not find the league.", LogLevel.CRITICAL);

                await _component.RespondAsync();

                break;
            default:
                response = "Something went wrong with the button press!";
                logLevel = LogLevel.ERROR;
                break;
        }

        await SerializationManager.SerializeDB();

        if (splitString[0] != "leagueRegisteration")
        {
            Log.WriteLine(response, logLevel);
            if (response != "EMPTY") await _component.RespondAsync(response);
        }
    }
}