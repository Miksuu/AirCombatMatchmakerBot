using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;

[DataContract]
public class CHALLENGEBUTTON : BaseButton
{
    public CHALLENGEBUTTON()
    {
        buttonName = ButtonName.CHALLENGEBUTTON;
        buttonLabel = "CHALLENGE";
        buttonStyle = ButtonStyle.Primary;
    }

    public void CreateTheButton(){}

    public override async Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, ulong _channelId, InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("Starting processing a challenge by: " +
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
            Log.WriteLine("CategoryName was null!", LogLevel.ERROR);
            return "";
        }

        string? categoryNameString = categoryName.ToString();
        if (categoryNameString == null)
        {
            Log.WriteLine("CategoryNameString was null!", LogLevel.ERROR);
            return "";
        }

        Log.WriteLine("categoryNameString: " + categoryNameString, LogLevel.VERBOSE);

        InterfaceLeague? dbLeagueInstance = 
            Database.Instance.Leagues.FindLeagueInterfaceWithCategoryNameString(buttonCategoryId);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return "Error adding to the queue! could not find the league.";
        }

        // Merge the message and the current challenge status in to one.
        string postedChallengeMessage =
            dbLeagueInstance.LeagueData.ChallengeStatus.PostChallengeToThisLeague(
                _component.User.Id, dbLeagueInstance.LeaguePlayerCountPerTeam,
                dbLeagueInstance).Result;

        if (postedChallengeMessage == "alreadyInQueue")
        {
            return "You are already in the queue!";
        }

        string newMessage = _interfaceMessage.Message + postedChallengeMessage;

        await _interfaceMessage.ModifyMessage(newMessage);

        Log.WriteLine("After modifying message", LogLevel.VERBOSE);

        return "";
    }
}