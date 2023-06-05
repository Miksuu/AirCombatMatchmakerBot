using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class CHALLENGEMESSAGE : BaseMessage
{
    public CHALLENGEMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.CHALLENGEMESSAGE;

        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.CHALLENGEBUTTON, 1),
                new KeyValuePair<ButtonName, int>(ButtonName.CHALLENGEQUEUECANCELBUTTON, 1),
            });

        thisInterfaceMessage.MessageDescription = "Insert the challenge message here";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override string GenerateMessage()
    {
        Log.WriteLine("Generating a challenge queue message with _channelId: " +
            thisInterfaceMessage.MessageChannelId + " on category: " + thisInterfaceMessage.MessageCategoryId, LogLevel.VERBOSE);

        foreach (var createdCategoriesKvp in
            Database.Instance.Categories.CreatedCategoriesWithChannels)
        {
            Log.WriteLine("On league: " +
                createdCategoriesKvp.Value.CategoryType, LogLevel.VERBOSE);

            string leagueName =
                EnumExtensions.GetEnumMemberAttrValue(createdCategoriesKvp.Value.CategoryType);

            Log.WriteLine("Full league name: " + leagueName, LogLevel.VERBOSE);

            if (createdCategoriesKvp.Value.InterfaceChannels.Any(
                    x => x.Value.ChannelId == thisInterfaceMessage.MessageChannelId))
            {
                ulong channelIdToLookFor =
                    createdCategoriesKvp.Value.InterfaceChannels.FirstOrDefault(
                        x => x.Value.ChannelId == thisInterfaceMessage.MessageChannelId).Value.ChannelId;

                Log.WriteLine("Looping on league: " + leagueName +
                    " looking for id: " + channelIdToLookFor, LogLevel.VERBOSE);

                if (thisInterfaceMessage.MessageChannelId == channelIdToLookFor)
                {
                    Log.WriteLine("Found: " + channelIdToLookFor +
                        " is league: " + leagueName, LogLevel.DEBUG);


                    thisInterfaceMessage.MessageEmbedTitle = leagueName + " challenge.";
                    string challengeMessage = "Players In The Queue: \n";

                    //Log.WriteLine("id: " + thisInterfaceMessage.MessageCategoryId, LogLevel.WARNING);

                    var leagueCategory =
                        Database.Instance.Leagues.GetILeagueByCategoryId(thisInterfaceMessage.MessageCategoryId);
                    if (leagueCategory == null)
                    {
                        Log.WriteLine(nameof(leagueCategory) + " was null!", LogLevel.ERROR);
                        return "";
                    }

                    foreach (int teamInt in leagueCategory.LeagueData.ChallengeStatus.TeamsInTheQueue)
                    {
                        try
                        {
                            var team = leagueCategory.LeagueData.FindActiveTeamWithTeamId(teamInt);
                            challengeMessage += "[" + team.SkillRating + "] " + team.TeamName + "\n";
                        }
                        catch(Exception ex)
                        {
                            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                            continue;
                        }
                    }

                    Log.WriteLine("Challenge message generated: " + challengeMessage, LogLevel.VERBOSE);

                    return challengeMessage;
                }
            }
        }

        Log.WriteLine("Did not find a channel id to generate a challenge" +
            " queue message on!", LogLevel.ERROR);

        return string.Empty;
    }
}