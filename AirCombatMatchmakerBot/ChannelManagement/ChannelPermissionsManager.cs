using Discord;
using Discord.WebSocket;

public static class ChannelPermissionsManager
{
    public static async Task SetRegisterationChannelPermissions(
        ulong _userId, ITextChannel _channel)
    {
        // Sets permission overrides
        var permissionOverridesUser = new OverwritePermissions(viewChannel: PermValue.Allow);

        if (_channel != null)
        {
            Log.WriteLine("FOUND CHANNEL TO SET PERMISSIONS ON: " + _channel.Id, LogLevel.DEBUG);

            var guild = BotReference.GetGuildRef();

            if (guild != null)
            {
                // Allow the channell access to the new user
                await _channel.AddPermissionOverwriteAsync(guild.GetUser(_userId), permissionOverridesUser);
            }
            else Exceptions.BotGuildRefNull();
        }
        else
        {
            Log.WriteLine("_Channel was null!", LogLevel.CRITICAL);
        }
    }
}