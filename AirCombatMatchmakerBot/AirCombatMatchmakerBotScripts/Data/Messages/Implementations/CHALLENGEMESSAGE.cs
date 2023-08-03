using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class CHALLENGEMESSAGE : BaseMessage
{
    [IgnoreDataMember]
    ConcurrentDictionary<int, string> TeamsThatHaveMatchesClose
    {
        get => teamsThatHaveMatchesClose.GetValue();
        set => teamsThatHaveMatchesClose.SetValue(value);
    }

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

    private logConcurrentDictionary<int, string> teamsThatHaveMatchesClose = new logConcurrentDictionary<int, string>();

    public override Task<string> GenerateMessage(ulong _leagueCategoryId = 0)
    {
        try
        {
            Log.WriteLine("Generating a challenge queue message with _channelId: " +
                thisInterfaceMessage.MessageChannelId + " on category: " + thisInterfaceMessage.MessageCategoryId);

            foreach (var createdCategoriesKvp in
                DiscordBotDatabase.Instance.Categories.CreatedCategoriesWithChannels)
            {
                Log.WriteLine("On league: " +
                    createdCategoriesKvp.Value.CategoryType);

                string leagueName =
                    EnumExtensions.GetEnumMemberAttrValue(createdCategoriesKvp.Value.CategoryType);

                Log.WriteLine("Full league name: " + leagueName);

                if (!createdCategoriesKvp.Value.InterfaceChannels.Any(
                        x => x.Value.ChannelId == thisInterfaceMessage.MessageChannelId))
                {
                    continue;
                }

                ulong channelIdToLookFor =
                    createdCategoriesKvp.Value.InterfaceChannels.FirstOrDefault(
                        x => x.Value.ChannelId == thisInterfaceMessage.MessageChannelId).Value.ChannelId;

                Log.WriteLine("Looping on league: " + leagueName +
                    " looking for id: " + channelIdToLookFor);

                if (thisInterfaceMessage.MessageChannelId != channelIdToLookFor)
                {
                    continue;
                }

                Log.WriteLine("Found: " + channelIdToLookFor +
                    " is league: " + leagueName, LogLevel.DEBUG);


                thisInterfaceMessage.MessageEmbedTitle = leagueName + " challenge.";
                string challengeMessage = "Players In The Queue: \n";

                var leagueCategory =
                    Database.Instance.Leagues.GetILeagueByCategoryId(thisInterfaceMessage.MessageCategoryId);

                foreach (int teamInt in leagueCategory.LeagueData.ChallengeStatus.TeamsInTheQueue)
                {
                    try
                    {
                        var team = leagueCategory.LeagueData.FindActiveTeamWithTeamId(teamInt);
                        challengeMessage += "[" + team.SkillRating + "] " + team.TeamName;

                        if (TeamsThatHaveMatchesClose.ContainsKey(teamInt))
                        {
                            challengeMessage += TeamsThatHaveMatchesClose[teamInt];
                        }

                        challengeMessage += "\n";
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.Message, LogLevel.ERROR);
                        continue;
                    }
                }

                if (teamsThatHaveMatchesClose.Count() > 0)
                {
                    challengeMessage += "*Players will be removed from the queue 30 minutes before their scheduled match.*";
                }

                Log.WriteLine("Challenge message generated: " + challengeMessage);

                return Task.FromResult(challengeMessage);
            }

            Log.WriteLine("Did not find a channel id to generate a challenge" +
                " queue message on!", LogLevel.ERROR);

            return Task.FromResult(string.Empty);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            throw;
        }
    }

    public async Task<bool> UpdateTeamsThatHaveMatchesClose()
    {
        try
        {
            bool updateTheMessage = false;

            var leagueCategory =
                Database.Instance.Leagues.GetILeagueByCategoryId(thisInterfaceMessage.MessageCategoryId);

            TeamsThatHaveMatchesClose.Clear();

            foreach (int teamInt in leagueCategory.LeagueData.ChallengeStatus.TeamsInTheQueue)
            {
                var team = leagueCategory.LeagueData.FindActiveTeamWithTeamId(teamInt);
                var times = await team.GetMatchesThatAreCloseToTeamsMembers(600);

                if (times == "")
                {
                    continue;
                }

                updateTheMessage = true;

                TeamsThatHaveMatchesClose.TryAdd(teamInt, times);
            }

            return updateTheMessage;
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            throw;
        }
    }

    public override string GenerateMessageFooter()
    {
        return "";
        //return "Last updated at: " + DateTime.UtcNow.ToLongTimeString() + " " + DateTime.UtcNow.ToLongDateString() + " (GMT+0)";
    }
}