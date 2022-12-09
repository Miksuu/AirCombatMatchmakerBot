﻿using Discord;
using Discord.WebSocket;
using System;

public static class RoleManager
{
    public static async Task<string> FindRoleNameById(ulong _roleId)
    {
        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return "null";
        }

        return guild.Roles.First(r => r.Id == _roleId).Name;
    }

    public static async Task GrantUserAccessWithId(ulong _playerId, ulong _roleId)
    {
        await GrantUserAccess(_playerId, FindRoleNameById(_roleId).Result);
    }

    public static async Task GrantUserAccess(ulong _playerId, string _roleName)
    {
        Log.WriteLine("Granting role " + _roleName + " from: " + _playerId, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        var user = guild.GetUser(_playerId) as IGuildUser;
        if (user == null)
        {
            Log.WriteLine("User with id: " + _playerId + " was null!", LogLevel.CRITICAL);
            return;
        }

        var role = guild.Roles.First(x => x.Name == _roleName);
        if (role == null)
        {
            Log.WriteLine("Role " + _roleName + "was null!", LogLevel.CRITICAL);
            return;
        }

        // Add the role to the user
        await user.AddRoleAsync(role);

        Log.WriteLine("Done granting role " + _roleName + " from: " + _playerId, LogLevel.VERBOSE);
    }

    public static async Task RevokeUserAccess(ulong _playerId, string _roleName)
    {
        Log.WriteLine("Revoking role " + _roleName + " from: " + _playerId, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        var user = guild.GetUser(_playerId) as IGuildUser;
        if (user == null)
        {
            Log.WriteLine("User with id: " + _playerId + " was null!", LogLevel.CRITICAL);
            return;
        }

        var role = guild.Roles.First(x => x.Name == _roleName);
        if (role == null)
        {
            Log.WriteLine("Role " + _roleName + "was null!", LogLevel.CRITICAL);
            return;
        }

        // Add the role to the user
        await user.RemoveRoleAsync(role);

        Log.WriteLine("Done revoking role " + _roleName + " from: " + _playerId, LogLevel.VERBOSE);
    }

    public static async Task<SocketRole> CheckIfRoleExistsByNameAndCreateItIfItDoesntElseReturnIt
        (SocketGuild _guild, string _roleName)
    {
        Log.WriteLine("Checking if role exists by name: " + _roleName, LogLevel.VERBOSE);

        foreach (SocketRole role in _guild.Roles)
        {
            if (role.Name == _roleName)
            {
                Log.WriteLine("Found role: " + role.Name + " with id:" + role.Id +
                    " returning it", LogLevel.DEBUG);
                return role;
            }
        }

        Log.WriteLine("Role" + _roleName + " was not found, creating it", LogLevel.DEBUG);

        var newRole = await _guild.CreateRoleAsync(_roleName);
        Log.WriteLine("Created a new role: " + newRole.Name, LogLevel.VERBOSE);

        SocketRole socketRole = _guild.GetRole(newRole.Id);
        Log.WriteLine("Found socketrole: " + socketRole.Name + " with id: " +
            socketRole.Id + " returning it.", LogLevel.VERBOSE);

        return socketRole;
    }
}