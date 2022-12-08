using Discord;

public static class RoleManagement
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
}