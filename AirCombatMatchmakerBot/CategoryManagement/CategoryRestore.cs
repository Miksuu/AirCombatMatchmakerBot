using Discord;
using Discord.WebSocket;
using System;

public static class CategoryRestore
{
    public static async Task<bool> CheckIfCategoryHasBeenDeletedAndRestore(
        ulong _categoryId, SocketGuild _guild, InterfaceCategory _interfaceCategory)
    {
        Log.WriteLine("Checking if categoryId: " + _categoryId + " has been deleted.", LogLevel.VERBOSE);

        if (_guild.CategoryChannels.Any(x => x.Id == _categoryId))
        {
            Log.WriteLine("Category found, returning. ", LogLevel.VERBOSE);
            return true;
        }

        Log.WriteLine("Category " + _interfaceCategory.CategoryName +
            " not found, regenerating it...", LogLevel.ERROR);

        return false;
    }
}