using Discord;
using Discord.WebSocket;
using System;

public static class CategoryRestore
{
    public static Task<bool>
        CheckIfCategoryHasBeenDeletedAndRestoreForCategory(
        System.Collections.Generic.KeyValuePair<
        ulong, InterfaceCategory> _categoryKvp,
        SocketGuild _guild)
    {
        Log.WriteLine("Checking if categoryId: " + _categoryKvp.Key +
            " has been deleted.", LogLevel.VERBOSE);

        if (_guild.CategoryChannels.Any(x => x.Id == _categoryKvp.Key))
        {
            Log.WriteLine("Category found, returning. ", LogLevel.VERBOSE);
            return new Task<bool>(() => true);
        }

        Log.WriteLine("Category " + _categoryKvp.Value.CategoryName +
            " not found, regenerating it...", LogLevel.ERROR);

        // Delete the old entry from the database
        Database.Instance.CreatedCategoriesWithChannels.Remove(_categoryKvp.Key);

        return new Task<bool>(() => false);
    }
}