using Discord;
using Discord.WebSocket;
using System;

public static class ChannelRestore
{
    public static async Task<bool> CheckIfChannelHasBeenDeletedAndRestore(
        ulong _categoryId, SocketGuild _guild, InterfaceChannel _interfaceChannel)
    {
        Log.WriteLine("Checking if channel in " + _categoryId + " has been deleted.", LogLevel.VERBOSE);

        if (_guild.GetCategoryChannel(_categoryId).Channels.Any(
            x => x.Name == EnumExtensions.GetEnumMemberAttrValue(
                _interfaceChannel.ChannelName)))
        {
            Log.WriteLine("Channel found, returning. ", LogLevel.VERBOSE);
            return true;
        }

        // Handles deleting the old value
        var dbKeyValue = Database.Instance.CreatedCategoriesWithChannels.First(
                                         x => x.Key == _categoryId).Value;

        var dbFinal = dbKeyValue.InterfaceChannels.First(
                                             ic => ic.ChannelId == _interfaceChannel.ChannelId);

        dbFinal.ChannelFeaturesWithMessageIds.Clear();

        dbKeyValue.InterfaceChannels.Remove(dbFinal);

        Log.WriteLine("Channel " + _interfaceChannel.ChannelName +
            " not found, regenerating it...", LogLevel.ERROR);

        return false;
    }
}