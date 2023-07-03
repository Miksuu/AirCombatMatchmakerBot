using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class DevTools
{
    // !!!
    // ONLY FOR TESTING, DELETES ALL CHANNELS AND CATEGORIES
    // !!!

    public async static void DeleteAllCategoriesChannelsAndRoles()
    {
        var guild = BotReference.GetGuildRef();

        DeleteCategories(guild, new List<string> { "main-category" });
        DeleteChannels(guild, new List<string> { "info", "test" });

        // Delete the old tacviews so it doesn't throw error from old files
        string tacviewPathToDelete = @"C:\AirCombatMatchmakerBot\Data\Tacviews";
        if (Directory.Exists(tacviewPathToDelete)) Directory.Delete(tacviewPathToDelete, true);

        string logsPathToDelete = @"C:\AirCombatMatchmakerBot\Logs";
        if (Directory.Exists(logsPathToDelete)) Directory.Delete(logsPathToDelete, true);

        // Delete db here
        File.Delete(@"C:\AirCombatMatchmakerBot\Data\database.json");
        await SerializationManager.HandleDatabaseCreationOrLoading("0");

        // Delete roles here
        foreach (var item in guild.Roles)
        {
            Log.WriteLine("on role: " + item.Name);

            if (item.Name == "Developer" || item.Name == "Server Booster" ||
                item.Name == "AirCombatMatchmakerBotDev" || item.Name == "Discord Me" ||
                item.Name == "@everyone" || item.Name == "@here")
            {
                continue;
            }

            Log.WriteLine("Deleting role: " + item.Name, LogLevel.DEBUG);

            await item.DeleteAsync();
        }
    }

    private async static void DeleteCategories(SocketGuild _guild, List<string> _categoriesNotToDelete)
    {
        Log.WriteLine("Deleting all categories with count: " + _categoriesNotToDelete.Count);
        foreach (var category in _guild.CategoryChannels)
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
    }

    private async static void DeleteChannels(SocketGuild _guild, List<string> _channelsNotToDelete)
    {
        Log.WriteLine("Deleting all categories with count: " + _channelsNotToDelete.Count);
        foreach (SocketGuildChannel channel in _guild.Channels)
        {
            Log.WriteLine("Looping on channel : " + channel.Name);
            Log.WriteLine(channel.Name, LogLevel.DEBUG);

            if (_channelsNotToDelete.Contains(channel.Name))
            {
                continue;
            }

            Log.WriteLine("deleting channel: " + channel.Name);
            await channel.DeleteAsync();
            Log.WriteLine("done deleting channel: " + channel.Name, LogLevel.DEBUG);
        }
    }

    // !!!
    // ONLY FOR TESTING, DELETES ALL CHANNELS AND CATEGORIES
    // !!!
}
