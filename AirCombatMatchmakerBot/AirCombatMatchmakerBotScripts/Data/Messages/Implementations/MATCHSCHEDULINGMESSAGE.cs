using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class MATCHSCHEDULINGMESSAGE : BaseMessage
{
    MatchChannelComponents mcc;

    public MATCHSCHEDULINGMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.MATCHSCHEDULINGMESSAGE;

        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
            });

        thisInterfaceMessage.MessageEmbedTitle = "Schedule your match here using the /schedule command";
        thisInterfaceMessage.MessageDescription = "Match scheduling message";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _channelCategoryId)
    {
        base.GenerateRegularButtons(_component, _channelCategoryId);
    }

    public override Task<MessageComponents> GenerateMessage(ulong _channelCategoryId = 0)
    {
        mcc = new MatchChannelComponents(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.ERROR);
            return Task.FromResult(nameof(mcc) + " was null!");
        }

        string finalMessage = "Enter the time you would be able to play the match on in format: \n " +
            "``/schedule 27.11.2022 1030z``\n" + "Instead of using the date 27.11.2022 you can use: [today, tomorrow]," +
            " [any weekday] (will assume the next day that's available), [now] (will schedule the match 20 minutes away," +
            " 5 minutes time to accept), [x hours, minutes] (schedules the match from x hours and minutes away)\n" +
            "You can also use slashes in the date and put the time first, for example: ``/schedule 11z 27/11/2022``\n\n" +
            "Examples:\n" +
            "``/schedule 0659z 01.02.2024``\n" +
            "``/schedule 03/04/2024 07z``\n" +
            "``/schedule 1835z saturday``\n" +
            "``/schedule sunday 19z``\n" +
            "``/schedule today 1920z``\n" +
            "``/schedule tomorrow 2023z``\n" +
            "``/schedule now``\n" +
            "``/schedule 6 hours``\n" +
            "``/schedule 4 hours 30minutes``\n" +
            "``/schedule 42 minutes``\n" +
            "*Copypasting these messages won't work! You have to type them in manually.*";

        var leagueMatch = Database.GetInstance<ApplicationDatabase>().Leagues.GetILeagueByCategoryId(
            thisInterfaceMessage.MessageCategoryId).LeagueData.Matches.FindLeagueMatchByTheChannelId(
                thisInterfaceMessage.MessageChannelId);

        finalMessage += Database.GetInstance<ApplicationDatabase>().Leagues.GetListOfTimesThatWontBeSuitableForScheduling(leagueMatch, mcc.interfaceLeagueCached);

        return Task.FromResult(finalMessage);
    }

    public override string GenerateMessageFooter()
    {
        return "";
        //return "Last updated at: " + DateTime.UtcNow.ToLongTimeString() + " " + DateTime.UtcNow.ToLongDateString() + " (GMT+0)";
    }
}