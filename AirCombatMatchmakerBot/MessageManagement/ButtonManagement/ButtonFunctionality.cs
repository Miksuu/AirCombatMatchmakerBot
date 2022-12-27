using Discord;
using Discord.WebSocket;
using System.ComponentModel;
using System.Linq;

public static class ButtonFunctionality
{
    private static ILeague? FindLeagueInterfaceWithSplitStringPart(
        string _splitStringIdPart)
    {
        Log.WriteLine("Starting to find Ileague from db with: " +
            _splitStringIdPart, LogLevel.VERBOSE);

        KeyValuePair<ulong, InterfaceCategory> findLeagueCategoryType =
            Database.Instance.Categories.GetCreatedCategoryWithChannelKvpWithId(
                ulong.Parse(_splitStringIdPart));
        CategoryName leagueCategoryName = findLeagueCategoryType.Value.CategoryName;

        Log.WriteLine("found: " + nameof(leagueCategoryName) + ": " +
            leagueCategoryName.ToString(), LogLevel.VERBOSE);

        var leagueInterface =
            LeagueManager.GetLeagueInstanceWithLeagueCategoryName(leagueCategoryName);

        Log.WriteLine(
            "Found interface " + nameof(leagueInterface) + ": " +
            leagueInterface.LeagueCategoryName, LogLevel.VERBOSE);

        ILeague? dbLeagueInstance =
            Database.Instance.Leagues.GetInterfaceLeagueCategoryFromTheDatabase(leagueInterface);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine(nameof(dbLeagueInstance) + " db: " +
            dbLeagueInstance.LeagueCategoryName, LogLevel.VERBOSE);
        return dbLeagueInstance;
    }

    public static async Task<string> MainRegistration(SocketMessageComponent _component)
    {
        string response = "";
        // Checks that the player does not exist in the database already, true if this is not the case
        if (UserManager.AddNewPlayerToTheDatabaseById(_component.User.Id).Result)
        {
            Database.Instance.CachedUsers.AddUserIdToCachedList(_component.User.Id);

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
        return response;
    }

    public static async Task LeagueRegistration(
        SocketMessageComponent _component, string _splitString)
    {
        string responseMsg = "";

        Log.WriteLine("starting leagueRegistration", LogLevel.VERBOSE);

        ILeague? dbLeagueInstance = FindLeagueInterfaceWithSplitStringPart(_splitString);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return;
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
                return;
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
                    return;
                }

                // Add the role for the player for the specific league and set him teamActive
                UserManager.SetPlayerActiveAndGrantHimTheRole(
                    dbLeagueInstance, _component.User.Id);

                // Modify the message to have the new player count
                await MessageManager.ModifyLeagueRegisterationChannelMessage(dbLeagueInstance);


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

                await MessageManager.ModifyLeagueRegisterationChannelMessage(dbLeagueInstance);
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

        await _component.RespondAsync(responseMsg, ephemeral: true);
    }

    public static async Task PostChallenge(
        SocketMessageComponent _component, string _splitString)
    {
        Log.WriteLine("Starting processing a challenge by: " + _component.User.Id +
            " for league: " + _splitString, LogLevel.VERBOSE);

        ILeague? dbLeagueInstance = FindLeagueInterfaceWithSplitStringPart(_splitString);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return;
        }

        /*
        Log.WriteLine("Adding team to the challenge list", LogLevel.VERBOSE);

        BaseLeague? baseLeague = dbLeagueInstance as BaseLeague;

        if (baseLeague == null)
        {
            Log.WriteLine(nameof(baseLeague) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return;
        }*/

        dbLeagueInstance.LeagueData.PostChallengeToThisLeague(
            _component.User.Id, dbLeagueInstance.LeaguePlayerCountPerTeam);
    }
}