using Discord;
using Discord.WebSocket;

public static class LeagueSpecificChannelsManager
{
    public static void CreateCategoryAndChannelsForALeague(ILeague _leagueInterface)
    {
        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        var newCategory = 
            guild.CreateCategoryChannelAsync(EnumExtensions.GetEnumMemberAttrValue(_leagueInterface.LeagueName));
    }
}