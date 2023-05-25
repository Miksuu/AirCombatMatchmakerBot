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

        mcc = new MatchChannelComponents(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            return;
        }
        Log.WriteLine("units count: " + mcc.interfaceLeagueCached.LeagueUnits.Count, LogLevel.VERBOSE);

        foreach (UnitName unitName in mcc.interfaceLeagueCached.LeagueUnits)
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

        mcc = new MatchChannelComponents(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
            return nameof(mcc) + " was null!";
        }

        string finalMessage = "Selected plane:\n";

        var matchReportData = mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData;

        int playersThatAreReady = 0;
        foreach (var teamKvp in matchReportData)
        {
            foreach (var item in teamKvp.Value.TeamMemberIdsWithSelectedPlanesByTheTeam)
            {
                string checkmark = EnumExtensions.GetEnumMemberAttrValue(EmojiName.REDSQUARE);

                if (item.Value.CurrentStatus == EmojiName.WHITECHECKMARK)
                {
                    checkmark = EnumExtensions.GetEnumMemberAttrValue(EmojiName.WHITECHECKMARK);
                    playersThatAreReady++;
                }

                finalMessage += checkmark + " " + teamKvp.Value.TeamName + "\n";
            }
        }

        Log.WriteLine(playersThatAreReady + " | " +
            mcc.interfaceLeagueCached.LeaguePlayerCountPerTeam * 2, LogLevel.DEBUG);

        if (playersThatAreReady >= mcc.interfaceLeagueCached.LeaguePlayerCountPerTeam * 2 &&
            mcc.leagueMatchCached.MatchReporting.MatchState == MatchState.PLAYERREADYCONFIRMATIONPHASE)
        {
            mcc.leagueMatchCached.MatchReporting.MatchState = MatchState.REPORTINGPHASE;

            InterfaceChannel interfaceChannel;
            try
            {
                interfaceChannel = Database.Instance.Categories.FindInterfaceCategoryWithId(
                    thisInterfaceMessage.MessageCategoryId).FindInterfaceChannelWithIdInTheCategory(
                        thisInterfaceMessage.MessageChannelId);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.CRITICAL);
                return ex.Message;
            }

            new Thread(() => mcc.leagueMatchCached.StartTheMatchOnSecondThread(interfaceChannel)).Start();
        }

        Log.WriteLine("Generated: " + finalMessage, LogLevel.DEBUG);

        return finalMessage;
    }
}