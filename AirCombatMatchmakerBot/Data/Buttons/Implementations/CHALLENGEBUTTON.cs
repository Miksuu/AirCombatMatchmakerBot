﻿using Discord;
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
        SocketMessageComponent _component, string _splitString, ulong _channelId, ulong _messageId, string _message)
    {
        Log.WriteLine("Starting processing a challenge by: " + _component.User.Id +
             " for league: " + _splitString, LogLevel.VERBOSE);

        // Find the category of the given button, temp, could optimise it here
        CategoryName? categoryName = null;
        foreach (var interfaceCategoryKvp in Database.Instance.Categories.CreatedCategoriesWithChannels)
        {
            Log.WriteLine("Loop on: " + interfaceCategoryKvp.Key + " | " +
                interfaceCategoryKvp.Value.CategoryName, LogLevel.VERBOSE);
            if (interfaceCategoryKvp.Value.InterfaceChannels.Any(x => x.ChannelId == _component.Channel.Id))
            {
                Log.WriteLine("Found category: " + interfaceCategoryKvp.Value.CategoryName, LogLevel.DEBUG);
                categoryName = interfaceCategoryKvp.Value.CategoryName;
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
            Database.Instance.Leagues.FindLeagueInterfaceWithSplitStringPart(categoryNameString);

        if (dbLeagueInstance == null)
        {
            Log.WriteLine(nameof(dbLeagueInstance) +
                " was null! Could not find the league.", LogLevel.CRITICAL);
            return "Error adding to the queue! could not find the league.";
        }

        // Merge the message and the current challenge status in to one.
        string postedChallengeMessage = dbLeagueInstance.LeagueData.PostChallengeToThisLeague(
            _component.User.Id, dbLeagueInstance.LeaguePlayerCountPerTeam);

        if (postedChallengeMessage == "alreadyInQueue")
        {
            return "You are already in the queue!";
        }

        string newMessage = _message + postedChallengeMessage;

        await MessageManager.ModifyMessage(_channelId, _messageId, newMessage);

        return "";
    }
}