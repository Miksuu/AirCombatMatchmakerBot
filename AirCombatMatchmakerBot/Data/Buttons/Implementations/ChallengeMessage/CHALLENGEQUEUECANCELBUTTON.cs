using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class CHALLENGEQUEUECANCELBUTTON : BaseButton
{
    public CHALLENGEQUEUECANCELBUTTON()
    {
        buttonName = ButtonName.CHALLENGEQUEUECANCELBUTTON;
        buttonLabel = "Leave Queue";
        buttonStyle = ButtonStyle.Danger;
        ephemeralResponse = true;
    }

    public override async Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("Starting processing a challenge canel request by: " +
            _component.User.Id , LogLevel.VERBOSE);

        // Find the category of the given button, temp, could optimise it here
        CategoryType? categoryName = null;
        foreach (var interfaceCategoryKvp in 
            Database.Instance.Categories.CreatedCategoriesWithChannels)
        {
            Log.WriteLine("Loop on: " + interfaceCategoryKvp.Key + " | " +
                interfaceCategoryKvp.Value.CategoryType, LogLevel.VERBOSE);
            if (interfaceCategoryKvp.Value.InterfaceChannels.Any(
                x => x.Value.ChannelId == _component.Channel.Id))
            {
                Log.WriteLine("Found category: " +
                    interfaceCategoryKvp.Value.CategoryType, LogLevel.DEBUG);
                categoryName = interfaceCategoryKvp.Value.CategoryType;
                break;
            }
        }

        if (categoryName == null)
        {
            string errorMsg = nameof(categoryName) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
        }

        string? categoryNameString = categoryName.ToString();
        if (categoryNameString == null)
        {
            string errorMsg = nameof(categoryName) + "String was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL); ;
            return (errorMsg, false);
        }

        Log.WriteLine("categoryNameString: " + categoryNameString, LogLevel.VERBOSE);

        InterfaceLeague? dbLeagueInstance = 
            Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(buttonCategoryId);

        if (dbLeagueInstance == null)
        {
            string errorMsg = "Error adding to the queue! Could not find the league" + nameof(dbLeagueInstance) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return (errorMsg, false);
        }

        string response = dbLeagueInstance.LeagueData.ChallengeStatus.RemoveChallengeFromThisLeague(
            _component.User.Id, dbLeagueInstance.LeaguePlayerCountPerTeam, dbLeagueInstance);

        if (response == "notInTheQueue")
        {
            return ("You are not in the queue!!", false);
        }

        await _interfaceMessage.GenerateAndModifyTheMessage();

        Log.WriteLine("After modifying message", LogLevel.VERBOSE);

        return ("", true);
    }
}