using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;

public static class CategoryManager
{
    public static async Task<SocketCategoryChannel?> CreateANewSocketCategoryChannelAndReturnIt(
        SocketGuild _guild, string _categoryName, List<Overwrite> _permissions)
    {
        Log.WriteLine("Starting to create a new category with name: " + _categoryName, LogLevel.VERBOSE);

        RestCategoryChannel newCategory = await _guild.CreateCategoryChannelAsync(
            _categoryName, x => x.PermissionOverwrites = _permissions);
        if (newCategory == null)
        {
            Log.WriteLine(nameof(newCategory) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Created a new RestCategoryChannel with ID: " + newCategory.Id, LogLevel.VERBOSE);

        SocketCategoryChannel socketCategoryChannel =
            _guild.GetCategoryChannel(newCategory.Id);

        Log.WriteLine("socketCategoryId: " + socketCategoryChannel.Id.ToString(),LogLevel.VERBOSE);

        if (socketCategoryChannel == null)
        {
            Log.WriteLine(nameof(socketCategoryChannel) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Created a new socketCategoryChannel :" +socketCategoryChannel.Id.ToString() +
            " named: " + socketCategoryChannel.Name, LogLevel.DEBUG);

        return socketCategoryChannel;
    }
}