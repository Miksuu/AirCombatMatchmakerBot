using Discord;
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections.Concurrent;

[DataContract]
public class REPORTINGSTATUSMESSAGE : BaseMessage
{
    MatchChannelComponents mcc;

    public REPORTINGSTATUSMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.REPORTINGSTATUSMESSAGE;
        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>();
        thisInterfaceMessage.MessageEmbedTitle = "Current reporting status";
        thisInterfaceMessage.MessageDescription = "Insert the reporting status message here";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override Task<string> GenerateMessage(ulong _leagueCategoryId = 0)
    {
        string reportingStatusMessage = string.Empty;

        mcc = new MatchChannelComponents(this);
        if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
        {
            string errorMsg = nameof(mcc.interfaceLeagueCached) + " or " +
                nameof(mcc.leagueMatchCached) + " was null!";
            Log.WriteLine(errorMsg, LogLevel.CRITICAL);
            return Task.FromResult(errorMsg);
        }

        foreach (var teamKvp in mcc.leagueMatchCached.TeamsInTheMatch)
        {
            string reportingStatusPerTeam = teamKvp.Value + ":\n";

            if (!mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData.ContainsKey(teamKvp.Key))
            {
                Log.WriteLine("Does not contain reporting data on: " + teamKvp.Key + " named: " +
                    teamKvp.Value, LogLevel.CRITICAL);
                continue;
            }

            var teamReportData = mcc.leagueMatchCached.MatchReporting.TeamIdsWithReportData[teamKvp.Key];
            if (teamReportData == null)
            {
                Log.WriteLine(nameof(teamReportData) + " was null!", LogLevel.CRITICAL);
                continue;
            }

            foreach (var item in teamReportData.ReportingObjects)
            {
                string finalCheckMark = string.Empty;

                var interfaceItem = (InterfaceReportingObject)item;

                if (interfaceItem.HiddenBeforeConfirmation)
                {
                    Log.WriteLine(interfaceItem.TypeOfTheReportingObject + " was hidden before confirmation");
                    continue;
                }

                if (interfaceItem.CurrentStatus == EmojiName.WHITECHECKMARK)
                {
                    finalCheckMark = EnumExtensions.GetEnumMemberAttrValue(EmojiName.WHITECHECKMARK);
                }
                else if (interfaceItem.CurrentStatus == EmojiName.YELLOWSQUARE)
                {
                    finalCheckMark = EnumExtensions.GetEnumMemberAttrValue(EmojiName.YELLOWSQUARE);
                }
                else
                {
                    finalCheckMark = EnumExtensions.GetEnumMemberAttrValue(interfaceItem.CachedDefaultStatus);
                }

                Log.WriteLine("Found: " + nameof(interfaceItem) + " with values: " +
                    interfaceItem.TypeOfTheReportingObject + ", " + interfaceItem.ObjectValue + ", " +
                    interfaceItem.CurrentStatus + ", " + interfaceItem.CachedDefaultStatus.ToString() + ", with" +
                    finalCheckMark, LogLevel.DEBUG);

                reportingStatusPerTeam += finalCheckMark + " " +
                    EnumExtensions.GetEnumMemberAttrValue(interfaceItem.TypeOfTheReportingObject) + ": " +
                    interfaceItem.ObjectValue + "\n";
            }

            Log.WriteLine("Done looping through team: " + teamKvp.Key + " (" + teamKvp.Value +
                ")" + "with message: " + reportingStatusPerTeam);
            reportingStatusMessage += reportingStatusPerTeam + "\n";
        }

        Log.WriteLine("Returning: " + reportingStatusMessage, LogLevel.DEBUG);

        return Task.FromResult(reportingStatusMessage);
    }
}