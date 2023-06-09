using Discord;
using Discord.WebSocket;
using System;

public static class RoleManager
{
    public static Task<string> FindRoleNameById(ulong _roleId)
    {
        Log.WriteLine("Looking for role: " + _roleId, LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();
        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return Task.FromResult("");
        }

        var roles = guild.Roles.FirstOrDefault(r => r.Id == _roleId);
        if (roles == null)
        {
            Log.WriteLine(nameof(roles) + " was null!", LogLevel.CRITICAL);
            return Task.FromResult("");
        }

        return Task.FromResult(roles.Name);
    }

    public static async Task GrantUserAccessWithId(ulong _userId, ulong _roleId)
    {
        Log.WriteLine("Granting player + " + _userId + " access with id: " + _roleId,LogLevel.VERBOSE);
        await GrantUserAccess(_userId, FindRoleNameById(_roleId).Result);
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

        var role = guild.Roles.FirstOrDefault(x => x.Name == _roleName);
        if (role == null)
        {
            Log.WriteLine("Role " + _roleName + "was null!", LogLevel.CRITICAL);
            return;
        }

        if (user.RoleIds.Contains(role.Id))
        {
            Log.WriteLine("User already had that role!", LogLevel.DEBUG);
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

        var role = guild.Roles.FirstOrDefault(x => x.Name == _roleName);
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
        Log.WriteLine("Created a new role: " + newRole.Name + " with id: " + newRole.Id, LogLevel.DEBUG);

        SocketRole socketRole = _guild.GetRole(newRole.Id);
        Log.WriteLine("Found socketrole: " + socketRole.Name + " with id: " +
            socketRole.Id + " returning it.", LogLevel.VERBOSE);

        return socketRole;
    }
}