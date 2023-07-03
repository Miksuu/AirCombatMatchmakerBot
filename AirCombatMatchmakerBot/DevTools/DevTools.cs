using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class DevTools
{
    private static SocketGuild socketGuild;

    // !!!
    // ONLY FOR TESTING, DELETES ALL CHANNELS AND CATEGORIES
    // !!!

    public async static Task DeleteAllCategoriesChannelsAndRoles()
    {
        socketGuild = BotReference.GetGuildRef();

        await DeleteCategories(new List<string> { "main-category" });
        await DeleteChannels(new List<string> { "info", "test", "main-category" });

        FileManager.DeleteDirectoryIfItExists(TacviewManager.tacviewsPath);
        FileManager.DeleteDirectoryIfItExists(Log.logsPath);

        await DeleteDatabase();

        await DeleteRoles(new List<string> { "Developer", "Server Booster", "AirCombatMatchmakerBotDev", "Discord Me", "@everyone", "@here" });
    }

    private async static Task DeleteCategories(List<string> _categoriesNotToDelete)
    {
        Log.WriteLine("Deleting all categories with count: " + _categoriesNotToDelete.Count, LogLevel.DEBUG);
        foreach (SocketCategoryChannel category in socketGuild.CategoryChannels)
        {
            Log.WriteLine("Looping on category : " + category.Name);

            if (_categoriesNotToDelete.Contains(category.Name))
            {
                Log.WriteLine("Wont delete: " + category.Name);
                continue;
            }

            Log.WriteLine("deleting category: " + category.Name);
            await category.DeleteAsync();
            Log.WriteLine("done deleting category: " + category.Name, LogLevel.DEBUG);
        }

        Log.WriteLine("Done deleting all categories");
    }

    private async static Task DeleteChannels(List<string> _channelsNotToDelete)
    {
        Log.WriteLine("Deleting all channels with count: " + _channelsNotToDelete.Count, LogLevel.DEBUG);
        foreach (SocketGuildChannel channel in socketGuild.Channels)
        {
            Log.WriteLine("Looping on channel : " + channel.Name);

            if (_channelsNotToDelete.Contains(channel.Name))
            {
                Log.WriteLine("Wont delete: " + channel.Name);
                continue;
            }

            Log.WriteLine("deleting channel: " + channel.Name);
            await channel.DeleteAsync();
            Log.WriteLine("done deleting channel: " + channel.Name, LogLevel.DEBUG);
        }

        Log.WriteLine("Done deleting all channels", LogLevel.DEBUG);
    }

    private async static Task DeleteRoles(List<string> _rolesNotToDelete)
    {
        Log.WriteLine("Deleting all roles with count: " + _rolesNotToDelete.Count, LogLevel.DEBUG);
        // Delete roles here
        foreach (SocketRole role in socketGuild.Roles)
        {
            Log.WriteLine("on role: " + role.Name);

            if (_rolesNotToDelete.Contains(role.Name))
            {
                Log.WriteLine("Wont delete: " + role.Name);
                continue;
            }

            Log.WriteLine("Deleting role: " + role.Name);
            await role.DeleteAsync();
            Log.WriteLine("done deleting role: " + role.Name, LogLevel.DEBUG);
        }

        Log.WriteLine("Done deleting all roles", LogLevel.DEBUG);
    }

    private async static Task DeleteDatabase()
    {
        Log.WriteLine("Deleting database", LogLevel.DEBUG);
        FileManager.DeleteFileIfItExists(Database.dbPathWithFileName);
        await SerializationManager.HandleDatabaseCreationOrLoading("0");
        Log.WriteLine("Done deleting database", LogLevel.DEBUG);
    }

    // !!!
    // ONLY FOR TESTING, DELETES ALL CHANNELS AND CATEGORIES
    // !!!
}

