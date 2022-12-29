using Discord;
using Discord.WebSocket;
using System;

public static class ChannelRestore
{
    public static async Task<bool> CheckIfChannelHasBeenDeletedAndRestoreForCategory(
        ulong _categoryId,
        InterfaceChannel _interfaceChannel,
        SocketGuild _guild)
    {
        Log.WriteLine("Checking if channel in " + _categoryId +
            " has been deleted.", LogLevel.VERBOSE);

        if (_guild.GetCategoryChannel(_categoryId).Channels.Any(
            x => x.Id == _interfaceChannel.ChannelId))
        {
            Log.WriteLine("Channel found, returning. ", LogLevel.VERBOSE);
            return true;
        }

        // Handles deleting the old value
        var dbKeyValue =
            Database.Instance.Categories.GetCreatedCategoryWithChannelKvpWithId(
                _categoryId).Value;

        var dbFinal = dbKeyValue.InterfaceChannels.First(
                                             ic => ic.ChannelId == _interfaceChannel.ChannelId);

        dbFinal.ChannelMessagesWithIds.Clear();

        dbKeyValue.InterfaceChannels.Remove(dbFinal);

        Log.WriteLine("Channel " + _interfaceChannel.ChannelName +
            " not found, regenerating it...", LogLevel.ERROR);

        return false;
    }
}   