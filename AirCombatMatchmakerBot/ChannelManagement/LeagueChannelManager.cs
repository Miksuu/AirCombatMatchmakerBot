using Discord;
using Discord.WebSocket;

public static class LeagueChannelManager
{
    public static async Task<ulong> CreateALeagueJoinButton(
        ITextChannel _leagueRegistrationChannel, ILeague? _leagueInterface, string _leagueNameString)
    {
        Log.WriteLine("Starting to create a league join button for: " + _leagueNameString, LogLevel.VERBOSE);

        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return 0;
        }

        string leagueButtonRegisterationCustomId =
           "leagueRegisteration_" + _leagueInterface.LeagueName.ToString();

        Log.WriteLine(nameof(leagueButtonRegisterationCustomId) + ": " +
            leagueButtonRegisterationCustomId, LogLevel.VERBOSE);

        _leagueInterface = LeagueManager.GetILeagueFromTheDatabase(_leagueInterface);

        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return 0;
        }

        _leagueInterface.DiscordLeagueReferences.leagueRegisterationChannelMessageId =
            await ButtonComponents.CreateButtonMessage(
                _leagueRegistrationChannel.Id,
                GenerateALeagueJoinButtonMessage(_leagueInterface),
                "Join",
                leagueButtonRegisterationCustomId); // Maybe replace this with some other system

        Log.WriteLine("Done creating a league join button for: " + _leagueNameString, LogLevel.DEBUG);

        return _leagueInterface.DiscordLeagueReferences.leagueRegisterationChannelMessageId;
    }


    /*
    public static void CreateCategoryAndChannelsForALeague(SocketGuild _guild, ITextChannel _leagueRegistrationChannel)
    {
        string? leagueName = EnumExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueName);

        if (leagueName == null)
        {
            Log.WriteLine("LeagueName from " + _leagueInterface.LeagueName.ToString() +
                " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Starting to create a category channel for league: " +
            leagueName, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        // Get the role and create it if it already doesn't exist
        var role = RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
            guild, leagueName).Result;

        Log.WriteLine("Role is named: " + role.Name + " with ID: " + role.Id, LogLevel.VERBOSE);

        // Set the permissions, by default everyone is denied access except the league role owners
        List<Overwrite> permissions = new List<Overwrite>
        {
            new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Deny)),

            new Overwrite(role.Id, PermissionTarget.Role,
                new OverwritePermissions(viewChannel: PermValue.Allow))
        };

        SocketCategoryChannel? socketCategoryChannel =
            CategoryManager.CreateANewSocketCategoryChannelAndReturnIt(guild,
                leagueName, permissions).Result;

        if (socketCategoryChannel == null)
        {
            Log.WriteLine(nameof(socketCategoryChannel) + " was null!", LogLevel.CRITICAL);
            return;
        }

        _leagueInterface.DiscordLeagueReferences.leagueRoleId = role.Id;
        //_leagueInterface.DiscordLeagueReferences.leagueCategoryId = socketCategoryChannel.Id;
        //CreateChannelsForTheCategory(_leagueInterface, guild);

        Log.WriteLine("End of creating a category channel for league: " +
            _leagueInterface.LeagueName.ToString(), LogLevel.DEBUG);
    } */


    public static string GenerateALeagueJoinButtonMessage(ILeague _leagueInterface)
    {
        string? leagueEnumAttrValue =
            EnumExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueName);

        Log.WriteLine(nameof(leagueEnumAttrValue) + ": " +
            leagueEnumAttrValue, LogLevel.VERBOSE);

        return "." + "\n" + leagueEnumAttrValue + "\n" +
            GetAllowedUnitsAsString(_leagueInterface) + "\n" +
            GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(_leagueInterface);
    }

    private static string GetAllowedUnitsAsString(ILeague _leagueInterface)
    {
        string allowedUnits = string.Empty;

        for (int u = 0; u < _leagueInterface.LeagueUnits.Count; ++u)
        {
            allowedUnits += EnumExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueUnits[u]);

            // Is not the last index
            if (u != _leagueInterface.LeagueUnits.Count - 1)
            {
                allowedUnits += ", ";
            }
        }

        return allowedUnits;
    }

    private static string GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(ILeague _leagueInterface)
    {
        int count = 0;

        foreach (Team team in _leagueInterface.LeagueData.Teams)
        {
            if (team.active)
            {
                count++;
                Log.WriteLine("team: " + team.teamName +
                    " is active, increased count to: " + count, LogLevel.VERBOSE);
            }
            else
            {
                Log.WriteLine("team: " + team.teamName + " is not active", LogLevel.VERBOSE);
            }
        }

        Log.WriteLine("Total count: " + count, LogLevel.VERBOSE);

        if (_leagueInterface.LeaguePlayerCountPerTeam > 1)
        {
            return "Teams: " + count;
        }
        else
        {
            return "Players: " + count;
        }
    }

    /*
    public static async void CreateChannelsForTheCategory(
        ILeague _leagueInterface, SocketGuild _guild)
    {
        Log.WriteLine("Starting to create categories for: " +
            _leagueInterface.LeagueName.ToString(), LogLevel.VERBOSE);

        foreach (LeagueCategoryChannelType channelType in
            Enum.GetValues(typeof(LeagueCategoryChannelType)))
        {
            Log.WriteLine("Looping through: " + channelType.ToString(), LogLevel.VERBOSE);

            if (!_leagueInterface.DiscordLeagueReferences.leagueChannels.ContainsKey(channelType))
            {
                string? channelTypeString = EnumExtensions.GetEnumMemberAttrValue(channelType);
                if (channelTypeString == null)
                {
                    Log.WriteLine(nameof(channelTypeString) + " is null!", LogLevel.CRITICAL);
                    return;
                }

                Log.WriteLine("Does not contain: " + channelType.ToString() +
                    " adding it", LogLevel.DEBUG);

                /*
                ulong channelId = await CreateAChannelForTheCategory(_guild, channelTypeString,
                        _leagueInterface.DiscordLeagueReferences.leagueCategoryId);

                _leagueInterface.DiscordLeagueReferences.leagueChannels.Add(
                    channelType, channelId);

                LeagueChannelFeatures.ActivateFeatureOfTheChannel(channelId, channelType); 
            }
            else
            {
                Log.WriteLine("Contains " + channelType.ToString() +
                    " disregarding it", LogLevel.DEBUG);
            }

            Log.WriteLine("Done looping through: " + channelType.ToString(), LogLevel.VERBOSE);
        }
    }
    */

}