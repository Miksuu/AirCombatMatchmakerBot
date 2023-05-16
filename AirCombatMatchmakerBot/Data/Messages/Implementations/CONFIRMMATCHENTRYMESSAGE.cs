using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class CONFIRMMATCHENTRYMESSAGE : BaseMessage
{
    MatchChannelComponents mcc = new MatchChannelComponents();

    public CONFIRMMATCHENTRYMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.CONFIRMMATCHENTRYMESSAGE;

        /*
        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.PLANESELECTIONBUTTON, 0),
            });
        */
        thisInterfaceMessage.MessageEmbedTitle = "This message confirms the match entry [add more detailed message here]";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        //base.GenerateRegularButtons(_component, _leagueCategoryId);

        Log.WriteLine("Generating buttons with: " + _leagueCategoryId, LogLevel.VERBOSE);

        Dictionary<string, string> buttonsToGenerate = new Dictionary<string, string>();

        var league = Database.Instance.Leagues.GetILeagueByCategoryId(_leagueCategoryId);
        if (league == null)
        {
            Log.WriteLine(nameof(league) + " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("units count: " + league.LeagueUnits.Count, LogLevel.VERBOSE);

        foreach (UnitName unitName in league.LeagueUnits)
        {
            string unitNameKey = unitName.ToString();
            string unitNameEnumMemberValue = EnumExtensions.GetEnumMemberAttrValue(unitName);

            Log.WriteLine(unitNameKey + " | " + unitNameEnumMemberValue, LogLevel.DEBUG);

            buttonsToGenerate.Add(unitNameKey, unitNameEnumMemberValue);
        }

        base.GenerateButtonsWithCustomPropertiesAndIds(
            buttonsToGenerate, ButtonName.PLANESELECTIONBUTTON, _component, _leagueCategoryId);
    }

    public override string GenerateMessage()
    {
        Log.WriteLine("Starting to generate a message for the confirmation", LogLevel.DEBUG);

        mcc.FindMatchAndItsLeagueAndInsertItToTheCache(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            return nameof(mcc) + " was null!";
        }

        string finalMessage = "Selected plane:\n";

        var matchReportData = mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData;

        int selectedTeamsCounter = 0;
        foreach (var teamKvp in matchReportData)
        {
            string checkmark = EnumExtensions.GetEnumMemberAttrValue(EmojiName.REDSQUARE);

            if (teamKvp.Value.ConfirmedMatch)
            {
                checkmark = EnumExtensions.GetEnumMemberAttrValue(EmojiName.WHITECHECKMARK);
                selectedTeamsCounter++;
            }

            finalMessage += checkmark + " " + teamKvp.Value.TeamName + "\n";
        }

        if (selectedTeamsCounter > 1)
        {
            mcc.leagueMatchCached.FinishTheMatch(mcc.interfaceLeagueCached);
        }

        finalMessage += "You can either Confirm/Dispute the result below.";

        Log.WriteLine("Generated: " + finalMessage, LogLevel.DEBUG);

        return finalMessage;
    }
}