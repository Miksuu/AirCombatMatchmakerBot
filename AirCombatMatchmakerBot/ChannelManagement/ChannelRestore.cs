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
        Log.WriteLine("Checking if channel in " + _categoryId + " has been deleted.", LogLevel.VERBOSE);

        if (_guild.GetCategoryChannel(_categoryId).Channels.Any(
            x => x.Id == _interfaceChannel.ChannelId))
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

        //dbKeyValue.InterfaceChannels.Remove(dbFinal);

        Log.WriteLine("Channel " + _interfaceChannel.ChannelName +
            " not found, regenerating it...", LogLevel.ERROR);

        return false;
    }

    public static async Task<bool> CheckIfChannelHasBeenDeletedAndRestoreForeagueCategory(
        ulong _categoryId,
        InterfaceLeagueChannel _interfaceLeagueChannel,
        SocketGuild _guild)
    {
        Log.WriteLine("Checking if channel in " + _categoryId + " has been deleted.", LogLevel.VERBOSE);

        if (_guild.GetCategoryChannel(_categoryId).Channels.Any(
            x => x.Id == _interfaceLeagueChannel.LeagueChannelId))
        {
            Log.WriteLine("Channel found, returning. ", LogLevel.VERBOSE);
            return true;
        }

        // Handles deleting the old value
        var dbKeyValue = Database.Instance.CreatedCategoriesWithChannels.First(
                                         x => x.Key == _categoryId).Value;

        var dbFinal = dbKeyValue.InterfaceChannels.First(
                                             ic => ic.ChannelId == _interfaceLeagueChannel.LeagueChannelId);

        dbFinal.ChannelFeaturesWithMessageIds.Clear();

        //dbKeyValue.InterfaceChannels.Remove(dbFinal);

        Log.WriteLine("Channel " + _interfaceLeagueChannel.LeagueChannelName +
            " not found, regenerating it...", LogLevel.ERROR);

        return false;
    }
}