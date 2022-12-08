using Discord;

public static class RoleManagement
{
    public static async Task GrantUserAccess(ulong _playerId, string _roleName)
    {
        Log.WriteLine("Granting role " + _roleName + " from: " + _playerId, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild != null)
        {
            var user = guild.GetUser(_playerId) as IGuildUser;

            if (user != null)
            {
                var role = guild.Roles.First(x => x.Name == _roleName);
                if (role != null)
                {
                    // Add the role to the user
                    await user.AddRoleAsync(role);
                }
                else { Log.WriteLine("Role " + _roleName + "was null!", LogLevel.CRITICAL); }
            }
            else { Log.WriteLine("User with id: " + _playerId + " was null!", LogLevel.CRITICAL); }
        }
        else { Exceptions.BotGuildRefNull(); }
    }

    public static async Task RevokeUserAccess(ulong _playerId, string _roleName)
    {
        Log.WriteLine("Revoking role " + _roleName + " from: " + _playerId, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild != null)
        {
            var user = guild.GetUser(_playerId) as IGuildUser;

            if (user != null)
            {
                var role = guild.Roles.First(x => x.Name == _roleName);
                if (role != null)
                {
                    // Add the role to the user
                    await user.RemoveRoleAsync(role);
                }
                else { Log.WriteLine("Role " + _roleName + "was null!", LogLevel.CRITICAL); }
            }
            else { Log.WriteLine("User with id: " + _playerId + " was null!", LogLevel.CRITICAL); }
        }
        else { Exceptions.BotGuildRefNull(); }
    }
}