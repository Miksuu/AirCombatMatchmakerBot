using Discord;
using Discord.WebSocket;
using System;


public static class CategoryRestore
{
    public static bool CheckIfCategoryHasBeenDeletedAndRestoreForCategory(
        ulong _categoryKey, SocketGuild _guild)
    {
        Log.WriteLine("Checking if categoryId: " + _categoryKey +
            " has been deleted.", LogLevel.VERBOSE);

        if (_guild.CategoryChannels.Any(x => x.Id == _categoryKey))
        {
            Log.WriteLine("Category found, returning. ", LogLevel.VERBOSE);
            return true;
        }

        Log.WriteLine("Category " + _categoryKey +
            " not found, regenerating it...", LogLevel.DEBUG);

        // Delete the old entry from the database
        //Database.Instance.Categories.RemoveFromCreatedCategoryWithChannelWithKey(
        //    _categoryKey);

        return false;
    }
}