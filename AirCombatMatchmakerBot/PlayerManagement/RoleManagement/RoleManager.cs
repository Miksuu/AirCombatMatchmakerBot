using Discord;
using Discord.WebSocket;
using System;

public static class RoleManager
{
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

        var newRole = await _guild.CreateRoleAsync(_roleName);
        return _guild.GetRole(newRole.Id);
    }
}