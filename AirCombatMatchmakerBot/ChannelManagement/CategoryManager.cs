using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;

public static class CategoryManager
{
    public static async Task<SocketCategoryChannel?> CreateANewSocketCategoryChannelAndReturnIt(
        SocketGuild _guild, string _categoryName)
    {
        Log.WriteLine("Starting to create a new category with name: " + _categoryName, LogLevel.VERBOSE);

        RestCategoryChannel newCategory = await _guild.CreateCategoryChannelAsync(_categoryName);
        if (newCategory == null)
        {
            Log.WriteLine(nameof(newCategory) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Created a new RestCategoryChannel with ID: " + newCategory.Id, LogLevel.VERBOSE);

        SocketCategoryChannel socketCategoryChannel = 
            _guild.GetCategoryChannel(newCategory.Id);
        if (socketCategoryChannel == null)
        {
            Log.WriteLine(nameof(socketCategoryChannel) + " was null!", LogLevel.CRITICAL);
            return null;
        }

        Log.WriteLine("Created a new socketCategoryChannel :" +socketCategoryChannel.Id.ToString() +
            " named: " + socketCategoryChannel.Name, LogLevel.DEBUG);

        return socketCategoryChannel;
    }

    public static async Task<bool> CheckIfLeagueCategoryExists(ulong _categoryId)
    {
        Log.WriteLine("Checking if category with id: " + _categoryId + " exists", LogLevel.VERBOSE);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return false;
        }

        foreach (SocketCategoryChannel socketCategoryChannel in guild.CategoryChannels)
        {
            Log.WriteLine("Looping on category: " + socketCategoryChannel.Id + " named:" +
                socketCategoryChannel.Name, LogLevel.VERBOSE);

            if (socketCategoryChannel.Id == _categoryId)
            {
                Log.WriteLine("Category with id: " + _categoryId +
                    " was found, named: " + socketCategoryChannel.Name, LogLevel.DEBUG);
                return true;
            }
        }

        // Someone probably deleted the category, the program will regenerate those
        Log.WriteLine("Category with id: " + _categoryId + " was null, not found. " +
            "Perhaps someone deleted it manually?", LogLevel.ERROR);
        return false;
    }
}