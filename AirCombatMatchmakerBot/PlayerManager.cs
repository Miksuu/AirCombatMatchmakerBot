using Discord;
using Discord.WebSocket;

public static class PlayerManager
{
    public static async Task HandlePlayerLeave(SocketGuild _guild, SocketUser _user)
    {
        Log.WriteLine(_user.Username + " (" + _user.Id +
            ") bailed out! Handling deleting registeration channels etc.", LogLevel.DEBUG);

        await ChannelManager.DeleteUsersChannelsOnLeave(_guild, _user);
    }
}