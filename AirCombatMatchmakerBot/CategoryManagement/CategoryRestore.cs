using Discord;
using Discord.WebSocket;
using System;


public static class CategoryRestore
{
    public static bool CheckIfCategoryHasBeenDeletedAndRestoreForCategory(
        System.Collections.Generic.KeyValuePair<
        ulong, InterfaceCategory> _categoryKvp,
        SocketGuild _guild)
    {
        Log.WriteLine("Checking if categoryId: " + _categoryKvp.Key +
            " has been deleted.", LogLevel.VERBOSE);

        if (_guild.CategoryChannels.Any(x => x.Id == _categoryKvp.Key))
        {
            Log.WriteLine("Category found, returning. ", LogLevel.VERBOSE);
            return true;
        }

        Log.WriteLine("Category " + _categoryKvp.Value.CategoryType +
            " not found, regenerating it...", LogLevel.ERROR);

        // Delete the old entry from the database
        Database.Instance.Categories.RemoveFromCreatedCategoryWithChannelWithKey(
            _categoryKvp.Key);

        return false;
    }
}