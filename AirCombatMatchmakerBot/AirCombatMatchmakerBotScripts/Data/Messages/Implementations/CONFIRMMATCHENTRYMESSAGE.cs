using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class CONFIRMMATCHENTRYMESSAGE : BaseMessage
{
    MatchChannelComponents mcc;

    public CONFIRMMATCHENTRYMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.CONFIRMMATCHENTRYMESSAGE;
        thisInterfaceMessage.MessageEmbedTitle = "Select your plane. If you do not select your plane before the timer below expires," +
            " the match will be timed out.";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _channelCategoryId)
    {
        Log.WriteLine("Generating buttons with: " + _channelCategoryId);

        Dictionary<string, string> buttonsToGenerate = new Dictionary<string, string>();

        mcc = new MatchChannelComponents(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.ERROR);
            return;
        }
        Log.WriteLine("units count: " + mcc.interfaceLeagueCached.LeagueUnits.Count);

        foreach (UnitName unitName in mcc.interfaceLeagueCached.LeagueUnits)
        {
            string unitNameKey = unitName.ToString();
            string unitNameEnumMemberValue = EnumExtensions.GetEnumMemberAttrValue(unitName);

            Log.WriteLine(unitNameKey + " | " + unitNameEnumMemberValue, LogLevel.DEBUG);

            buttonsToGenerate.Add(unitNameKey, unitNameEnumMemberValue);
        }

        base.GenerateButtonsWithCustomPropertiesAndIds(
            buttonsToGenerate, ButtonName.PLANESELECTIONBUTTON, _component, _channelCategoryId);
    }

    public override Task<string> GenerateMessage(ulong _channelCategoryId = 0)
    {
        try
        {
            Log.WriteLine("Starting to generate a message for the confirmation", LogLevel.DEBUG);

            InitializeMatchComponents();

            string finalMessage = GenerateFinalMessage();

            Log.WriteLine("Generated: " + finalMessage, LogLevel.DEBUG);

            return Task.FromResult(finalMessage);
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return Task.FromResult(ex.Message);
        }
    }

    private void InitializeMatchComponents()
    {
        mcc = new MatchChannelComponents(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.ERROR);
            throw new Exception(nameof(mcc) + " was null!");
        }
    }

    private string GenerateFinalMessage()
    {
        string finalMessage = string.Empty;

        finalMessage += GenerateScheduledEventsMessage();
        finalMessage += "\n**Selected plane**:\n";

        var generatedTuple = GenerateMatchReportDataMessage();
        finalMessage += generatedTuple.Item1;

        //CheckPlayersReadyAndStartMatch(generatedTuple.Item1, generatedTuple.Item2);

        finalMessage += CheckMatchQueueEvent();

        return finalMessage;
    }

    private string GenerateScheduledEventsMessage()
    {
        string scheduledEventsMessage = string.Empty;

        foreach (ScheduledEvent scheduledEvent in mcc.leagueMatchCached.MatchEventManager.ClassScheduledEvents)
        {
            if (scheduledEvent.GetType() != typeof(MatchQueueAcceptEvent))
            {
                continue;
            }

            if (scheduledEvent.LeagueCategoryIdCached != mcc.interfaceLeagueCached.LeagueCategoryId ||
                scheduledEvent.MatchChannelIdCached != mcc.leagueMatchCached.MatchChannelId)
            {
                continue;
            }

            if (mcc.leagueMatchCached.IsAScheduledMatch)
            {
                scheduledEventsMessage += "**" +
                    TimeService.ConvertToZuluTimeFromUnixTime(scheduledEvent.TimeToExecuteTheEventOn).ToString() + "**\n";
            }

            scheduledEventsMessage += "Time left: " +
                TimeService.ReturnTimeLeftAsStringFromTheTimeTheActionWillTakePlace(scheduledEvent.TimeToExecuteTheEventOn) + "\n";
        }

        return scheduledEventsMessage;
    }

    private (string, int) GenerateMatchReportDataMessage()
    {
        string matchReportDataMessage = string.Empty;
        var matchReportData = mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData;
        int playersThatAreReady = 0;

        foreach (var teamKvp in matchReportData)
        {
            PLAYERPLANE? teamPlane = GetTeamPlane(teamKvp);
            foreach (var kvp in teamPlane.TeamMemberIdsWithSelectedPlanesByTheTeam)
            {
                playersThatAreReady = UpdateMatchReportDataMessage(ref matchReportDataMessage, teamKvp, kvp, playersThatAreReady);
            }
        }

        Log.WriteLine(playersThatAreReady + " | " +
            mcc.interfaceLeagueCached.LeaguePlayerCountPerTeam * 2, LogLevel.DEBUG);

        return (matchReportDataMessage, playersThatAreReady);
    }

    private PLAYERPLANE GetTeamPlane(KeyValuePair<int, ReportData> teamKvp)
    {
        PLAYERPLANE? teamPlane = teamKvp.Value.FindBaseReportingObjectOfType(TypeOfTheReportingObject.PLAYERPLANE) as PLAYERPLANE;
        if (teamPlane == null)
        {
            Log.WriteLine(nameof(teamPlane) + " was null!", LogLevel.ERROR);
            throw new Exception(nameof(teamPlane) + " was null!");
        }
        return teamPlane;
    }

    private int UpdateMatchReportDataMessage(ref string _matchReportDataMessage, KeyValuePair<int, ReportData> _teamKvp, KeyValuePair<ulong, UnitName> _kvp, int _playersThatAreReady)
    {
        string checkmark = EnumExtensions.GetEnumMemberAttrValue(EmojiName.REDSQUARE);

        if (_kvp.Value != UnitName.NOTSELECTED)
        {
            checkmark = EnumExtensions.GetEnumMemberAttrValue(EmojiName.WHITECHECKMARK);
            _playersThatAreReady++;
        }

        _matchReportDataMessage += checkmark + " " + _teamKvp.Value.TeamName;

        if (mcc.leagueMatchCached.MatchEventManager.ClassScheduledEvents.Any(e => e.GetType() == typeof(TempQueueEvent)))
        {
            AppendTempQueueEvents(ref _matchReportDataMessage, _kvp);
        }

        _matchReportDataMessage += "\n";
        return _playersThatAreReady;
    }

    private void AppendTempQueueEvents(ref string matchReportDataMessage, KeyValuePair<ulong, UnitName> kvp)
    {
        var listOfTempQueueEvents = mcc.leagueMatchCached.MatchEventManager.GetListOfEventsByType(typeof(TempQueueEvent));

        foreach (TempQueueEvent scheduledEvent in listOfTempQueueEvents)
        {
            if (scheduledEvent.PlayerIdCached != kvp.Key)
            {
                continue;
            }

            Log.WriteLine(scheduledEvent.TimeToExecuteTheEventOn.ToString());

            matchReportDataMessage += " (valid for: " + TimeService.ReturnTimeLeftAsStringFromTheTimeTheActionWillTakePlace(scheduledEvent.TimeToExecuteTheEventOn) + ")";
        }
    }

    // private void CheckPlayersReadyAndStartMatch(string _finalMessage, int _playersThatAreReady)
    // {
    //     if (_playersThatAreReady < mcc.interfaceLeagueCached.LeaguePlayerCountPerTeam * 2 ||
    //         mcc.leagueMatchCached.MatchState != MatchState.PLAYERREADYCONFIRMATIONPHASE)
    //     {
    //         return;
    //     }

    //     mcc.leagueMatchCached.MatchEventManager.ClearCertainTypeOfEventsFromTheList(typeof(MatchQueueAcceptEvent));
    //     mcc.leagueMatchCached.MatchEventManager.ClearCertainTypeOfEventsFromTheList(typeof(TempQueueEvent));

    //     mcc.leagueMatchCached.MatchState = MatchState.REPORTINGPHASE;

    //     InterfaceChannel interfaceChannel = Database.GetInstance<DiscordBotDatabase>().Categories.FindInterfaceCategoryWithCategoryId(
    //             thisInterfaceMessage.MessageCategoryId).FindInterfaceChannelWithIdInTheCategory(
    //                 thisInterfaceMessage.MessageChannelId);

    //     new Thread(() => mcc.leagueMatchCached.StartTheMatchOnSecondThread(interfaceChannel)).Start();
    // }

    private string CheckMatchQueueEvent()
    {
        string matchQueueEventMessage = string.Empty;

        if (mcc.leagueMatchCached.MatchEventManager.ClassScheduledEvents.Any(x => x.GetType() != typeof(MatchQueueAcceptEvent)))
        {
            return matchQueueEventMessage;
        }

        var matchQueueEvent =
        mcc.leagueMatchCached.MatchEventManager.ClassScheduledEvents.FirstOrDefault(
            x => x.GetType() == typeof(MatchQueueAcceptEvent));

        var timeLeft = TimeService.CalculateTimeUntilWithUnixTime(matchQueueEvent.TimeToExecuteTheEventOn);
        if (timeLeft > 1200)
        {
            matchQueueEventMessage +=
                "\n*Note that accepting the match 20 minutes before it's beginning makes your plane selection valid only for 5 minutes!*";
        }

        return matchQueueEventMessage;
    }

    public override string GenerateMessageFooter()
    {
        return "";
        //return "Last updated at: " + DateTime.UtcNow.ToLongTimeString() + " " + DateTime.UtcNow.ToLongDateString() + " (GMT+0)";
    }
}