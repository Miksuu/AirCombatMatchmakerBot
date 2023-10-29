using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class CHALLENGEMESSAGE : BaseMessage
{
    LeagueCategoryComponents lcc;

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

    protected override void GenerateButtons(ComponentBuilder _component, ulong _channelCategoryId)
    {
        base.GenerateRegularButtons(_component, _channelCategoryId);
    }

    private logConcurrentDictionary<int, string> teamsThatHaveMatchesClose = new logConcurrentDictionary<int, string>();

    public override Task<MessageComponents> GenerateMessage(ulong _channelCategoryId = 0)
    {
        try
        {
            string finalMessage = string.Empty;

            lcc = new LeagueCategoryComponents(thisInterfaceMessage.MessageCategoryId);
            if (lcc.interfaceLeagueCached == null)
            {
                Log.WriteLine(nameof(lcc) + " was null!", LogLevel.ERROR);
                throw new InvalidOperationException(nameof(lcc) + " was null!");
            }

            thisInterfaceMessage.MessageEmbedTitle =
                EnumExtensions.GetEnumMemberAttrValue(lcc.interfaceLeagueCached.LeagueCategoryName);

            var category = Database.GetInstance<DiscordBotDatabase>().Categories.CreatedCategoriesWithChannels[thisInterfaceMessage.MessageCategoryId];

            if (category == null)
            {
                Log.WriteLine(nameof(category) + " was null!", LogLevel.ERROR);
                throw new InvalidOperationException(nameof(category) + " was null!");
            }

            string challengeMessage = "Players In The Queue: \n";

            var leagueCategory =
                Database.GetInstance<ApplicationDatabase>().Leagues.GetILeagueByCategoryId(thisInterfaceMessage.MessageCategoryId);

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

            finalMessage += challengeMessage;
            finalMessage += "*Click the buttons below to join/leave the scheduler*";

            thisInterfaceMessage.MessageDescription = finalMessage;

            return Task.FromResult(new MessageComponents(thisInterfaceMessage.MessageDescription));

        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            throw;
        }
    }


    public async Task<bool> UpdateTeamsThatHaveMatchesClose(InterfaceLeague _interfaceLeague)
    {
        try
        {
            bool updateTheMessage = false;

            var leagueCategory =
                Database.GetInstance<ApplicationDatabase>().Leagues.GetILeagueByCategoryId(thisInterfaceMessage.MessageCategoryId);

            TeamsThatHaveMatchesClose.Clear();

            foreach (int teamInt in leagueCategory.LeagueData.ChallengeStatus.TeamsInTheQueue)
            {
                var team = leagueCategory.LeagueData.FindActiveTeamWithTeamId(teamInt);
                var times = await team.GetMatchesThatAreCloseToTeamsMembers(_interfaceLeague, 600);

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