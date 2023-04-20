using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class CONFIRMMATCHENTRYMESSAGE : BaseMessage
{
    public CONFIRMMATCHENTRYMESSAGE()
    {
        messageName = MessageName.CONFIRMMATCHENTRYMESSAGE;

        /*
        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.PLANESELECTIONBUTTON, 0),
            });
        */
        messageEmbedTitle = "This message confirms the match entry [add more detailed message here]";
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
        return messageDescription;
    }
}