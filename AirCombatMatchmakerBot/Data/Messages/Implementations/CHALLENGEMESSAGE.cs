using System.Runtime.Serialization;
using System.Collections.Concurrent;

[DataContract]
public class CHALLENGEMESSAGE : BaseMessage
{
    public CHALLENGEMESSAGE()
    {
        messageName = MessageName.CHALLENGEMESSAGE;

        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.CHALLENGEBUTTON, 1),
                new KeyValuePair<ButtonName, int>(ButtonName.CHALLENGEQUEUECANCELBUTTON, 1),
            });

        messageDescription = "Insert the challenge message here";
    }

    protected override void GenerateCustomMessageButtonNamesWithAmount()
    {

    }

    public override string GenerateMessage()
    {
        Log.WriteLine("Generating a challenge queue message with _channelId: " +
            messageChannelId + " on category: " + messageCategoryId, LogLevel.VERBOSE);

        foreach (var createdCategoriesKvp in
            Database.Instance.Categories.CreatedCategoriesWithChannels)
        {
            Log.WriteLine("On league: " +
                createdCategoriesKvp.Value.CategoryType, LogLevel.VERBOSE);

            string leagueName =
                EnumExtensions.GetEnumMemberAttrValue(createdCategoriesKvp.Value.CategoryType);

            Log.WriteLine("Full league name: " + leagueName, LogLevel.VERBOSE);

            if (createdCategoriesKvp.Value.InterfaceChannels.Any(
                    x => x.Value.ChannelId == messageChannelId))
            {
                ulong channelIdToLookFor =
                    createdCategoriesKvp.Value.InterfaceChannels.FirstOrDefault(
                        x => x.Value.ChannelId == messageChannelId).Value.ChannelId;

                Log.WriteLine("Looping on league: " + leagueName +
                    " looking for id: " + channelIdToLookFor, LogLevel.VERBOSE);

                if (messageChannelId == channelIdToLookFor)
                {
                    Log.WriteLine("Found: " + channelIdToLookFor +
                        " is league: " + leagueName, LogLevel.DEBUG);


                    messageEmbedTitle = leagueName + " challenge.";
                    string challengeMessage = "Players In The Queue: \n";

                    var leagueCategory = Database.Instance.Leagues.FindLeagueInterfaceWithLeagueCategoryId(messageCategoryId);
                    if (leagueCategory == null)
                    {
                        Log.WriteLine(nameof(leagueCategory) + " was null!", LogLevel.ERROR);
                        return "";
                    }

                    foreach (int teamInt in leagueCategory.LeagueData.ChallengeStatus.TeamsInTheQueue)
                    {
                        var team = leagueCategory.LeagueData.FindActiveTeamWithTeamId(teamInt);
                        if (team == null)
                        {
                            Log.WriteLine(nameof(team) + " was null!", LogLevel.ERROR);
                            return "";
                        }

                        challengeMessage += "[" + team.SkillRating + "] " + team.TeamName + "\n";
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