using Discord;
using Discord.WebSocket;

public static class LeagueChannelManager
{
    public static async Task<ulong> CreateALeagueJoinButton(
        ITextChannel _leagueRegistrationChannel, InterfaceLeagueCategory? _leagueInterface, string _leagueNameString)
    {
        Log.WriteLine("Starting to create a league join button for: " + _leagueNameString, LogLevel.VERBOSE);

        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return 0;
        }

        string leagueButtonRegisterationCustomId =
           "leagueRegistration_" + _leagueInterface.LeagueCategoryName.ToString();

        Log.WriteLine(nameof(leagueButtonRegisterationCustomId) + ": " +
            leagueButtonRegisterationCustomId, LogLevel.VERBOSE);

        _leagueInterface = LeagueManager.GetInterfaceLeagueCategoryFromTheDatabase(_leagueInterface);

        if (_leagueInterface == null)
        {
            Log.WriteLine("_leagueInterface was null!", LogLevel.CRITICAL);
            return 0;
        }

        _leagueInterface.DiscordLeagueReferences.leagueRegistrationChannelMessageId =
            await ButtonComponents.CreateButtonMessage(
                _leagueRegistrationChannel.Id,
                GenerateALeagueJoinButtonMessage(_leagueInterface),
                "Join",
                leagueButtonRegisterationCustomId); // Maybe replace this with some other system

        Log.WriteLine("Done creating a league join button for: " + _leagueNameString, LogLevel.DEBUG);

        return _leagueInterface.DiscordLeagueReferences.leagueRegistrationChannelMessageId;
    }
    public static void CreateCategoryAndChannelsForALeague(SocketGuild _guild, InterfaceLeagueCategory _leagueInterface)
    {
        string? leagueNameString = EnumExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueCategoryName);

        if (leagueNameString == null)
        {
            Log.WriteLine("LeagueCategoryName from " + _leagueInterface.LeagueCategoryName.ToString() +
                " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Starting to create a category channel for league: " +
            leagueNameString, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        // Get the role and create it if it already doesn't exist
        var role = RoleManager.CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt(
            guild, leagueNameString).Result;

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
                leagueNameString, permissions).Result;

        if (socketCategoryChannel == null)
        {
            Log.WriteLine(nameof(socketCategoryChannel) + " was null!", LogLevel.CRITICAL);
            return;
        }

        _leagueInterface.DiscordLeagueReferences.leagueRoleId = role.Id;
        //_leagueInterface.DiscordLeagueReferences.leagueCategoryId = socketCategoryChannel.Id;
        //CreateChannelsForTheLeagueCategory(_leagueInterface, guild);

        Log.WriteLine("End of creating a category channel for league: " +
            _leagueInterface.LeagueCategoryName.ToString(), LogLevel.DEBUG);
    }


    public static string GenerateALeagueJoinButtonMessage(InterfaceLeagueCategory _leagueInterface)
    {
        string? leagueEnumAttrValue =
            EnumExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueCategoryName);

        Log.WriteLine(nameof(leagueEnumAttrValue) + ": " +
            leagueEnumAttrValue, LogLevel.VERBOSE);

        return "." + "\n" + leagueEnumAttrValue + "\n" +
            GetAllowedUnitsAsString(_leagueInterface) + "\n" +
            GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(_leagueInterface);
    }

    private static string GetAllowedUnitsAsString(InterfaceLeagueCategory _leagueInterface)
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

    private static string GetIfTheLeagueHasPlayersOrTeamsAndCountFromInterface(InterfaceLeagueCategory _leagueInterface)
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
    public static async void CreateChannelsForTheLeagueCategory(
        InterfaceLeagueCategory _leagueInterface, SocketGuild _guild)
    {
        Log.WriteLine("Starting to create categories for: " +
            _leagueInterface.LeagueCategoryName.ToString(), LogLevel.VERBOSE);

        foreach (LeagueChannelName channelType in
            Enum.GetValues(typeof(LeagueChannelName)))
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
                ulong leagueChannelId = await CreateAChannelForTheCategory(_guild, channelTypeString,
                        _leagueInterface.DiscordLeagueReferences.leagueCategoryId);

                _leagueInterface.DiscordLeagueReferences.leagueChannels.Add(
                    channelType, leagueChannelId);

                LeagueChannelFeatures.ActivateFeatureOfTheChannel(leagueChannelId, channelType); 
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