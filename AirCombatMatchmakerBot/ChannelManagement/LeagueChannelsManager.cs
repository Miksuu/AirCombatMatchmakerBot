﻿using Discord;
using Discord.WebSocket;

public static class LeagueChannelsManager
{
    public static void CreateCategoryAndChannelsForALeague(ILeague _leagueInterface)
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

        _leagueInterface.DiscordLeagueReferences.leagueCategoryId = socketCategoryChannel.Id;
        CreateChannelsForTheCategory(_leagueInterface, guild);

        Log.WriteLine("End of creating a category channel for league: " +
            _leagueInterface.LeagueName.ToString(), LogLevel.DEBUG);
    }

    public static void CreateChannelsForTheCategory(
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
                Log.WriteLine("Does not contain: " + channelType.ToString() +
                    " adding it", LogLevel.DEBUG);

                _leagueInterface.DiscordLeagueReferences.leagueChannels.Add(
                    channelType, CreateAChannelForTheCategory(
                        _guild,
                        EnumExtensions.GetEnumMemberAttrValue(channelType),
                        _leagueInterface.DiscordLeagueReferences.leagueCategoryId).Result);
            }
            else
            {
                Log.WriteLine("Contains " + channelType.ToString() +
                    " disregarding it", LogLevel.DEBUG);
            }

            Log.WriteLine("Done looping through: " + channelType.ToString(), LogLevel.VERBOSE);
        }
    }

    private static async Task<ulong> CreateAChannelForTheCategory(
        SocketGuild _guild, string _name, ulong _forCategory)
    {
        Log.WriteLine("Create a channel named: " + _name +
            " for category: " + _forCategory, LogLevel.VERBOSE);

        var channel = await _guild.CreateTextChannelAsync(_name, tcp => tcp.CategoryId = _forCategory);

        Log.WriteLine("Done creating a channel named: " + _name + " with ID: " + channel.Id +
            " for category: " + _forCategory, LogLevel.DEBUG);

        return channel.Id;
    }
}