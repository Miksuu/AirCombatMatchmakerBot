using Discord;

public static class MessageManager
{
    public static async Task ModifyLeagueRegisterationChannelMessage(ILeague _dbLeagueInstance)
    {
        Log.WriteLine("Modifying league registration channel message with: " +
            _dbLeagueInstance.LeagueCategoryName, LogLevel.VERBOSE);

        await ModifyMessage(LeagueManager.leagueRegistrationChannelId,
            _dbLeagueInstance.DiscordLeagueReferences.leagueRegistrationChannelMessageId,
         GenerateALeagueJoinButtonMessage(_dbLeagueInstance));
    }

    private static async Task ModifyMessage(
        ulong _channelId, ulong _messageId, string _content)
    {
        Log.WriteLine("Modifying a message on channel id: " + _channelId + " that has msg id: " +
            _messageId + " with content: " + _content, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        var channel = guild.GetTextChannel(_channelId) as ITextChannel;

        await channel.ModifyMessageAsync(_messageId, m => m.Content = _content);

        Log.WriteLine("Modifying the message: " + _messageId + " done.", LogLevel.VERBOSE);
    }

    public static string GenerateALeagueJoinButtonMessage(ILeague _leagueInterface)
    {
        string? leagueEnumAttrValue =
            EnumExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueCategoryName);

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
}