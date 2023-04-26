using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;

[DataContract]
public class REPORTINGSTATUSMESSAGE : BaseMessage
{
    public REPORTINGSTATUSMESSAGE()
    {
        messageName = MessageName.REPORTINGSTATUSMESSAGE;
        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>();
        messageEmbedTitle = "Current reporting status";
        messageDescription = "Insert the reporting status message here";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override string GenerateMessage()
    {
        string reportingStatusMessage = string.Empty;

        InterfaceLeague? interfaceLeague =
            Database.Instance.Leagues.GetILeagueByCategoryId(messageCategoryId);
        if (interfaceLeague == null)
        {
            Log.WriteLine(nameof(interfaceLeague) + " was null!", LogLevel.ERROR);
            return "";
        }

        LeagueMatch? foundMatch =
            interfaceLeague.LeagueData.Matches.FindLeagueMatchByTheChannelId(messageChannelId);
        if (foundMatch == null)
        {
            Log.WriteLine(nameof(foundMatch) + " was null!", LogLevel.ERROR);
            return "";
        }

        foreach (var teamKvp in foundMatch.TeamsInTheMatch)
        {
            string reportingStatusPerTeam = teamKvp.Value + ":\n";

            if (!foundMatch.MatchReporting.TeamIdsWithReportData.ContainsKey(teamKvp.Key))
            {
                Log.WriteLine("Does not contain reporting data on: " + teamKvp.Key + " named: " +
                    teamKvp.Value, LogLevel.CRITICAL);
                continue;
            }

            var teamReportData = foundMatch.MatchReporting.TeamIdsWithReportData[teamKvp.Key];
            if (teamReportData == null)
            {
                Log.WriteLine(nameof(teamReportData) + " was null!", LogLevel.CRITICAL);
                continue;
            }

            FieldInfo[] fields = typeof(TeamReportData).GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

            Log.WriteLine("fields count: " + fields.Length, LogLevel.DEBUG);

            foreach (FieldInfo field in fields)
            {
                string finalCheckMark = string.Empty;

                Log.WriteLine("field type: " + field.FieldType, LogLevel.DEBUG);

                // Only process the PlayerReportData fields (ignore teamName)
                if (field.FieldType != typeof(PlayerReportData)) continue;

                Log.WriteLine("This is " + nameof(PlayerReportData) + " field: " +
                    field.FieldType, LogLevel.VERBOSE);

                PlayerReportData? reportObject = (PlayerReportData?)field.GetValue(teamReportData);
                if (reportObject == null)
                {
                    Log.WriteLine(nameof(reportObject) + " was null!", LogLevel.CRITICAL);
                    continue;
                }

                if (reportObject.CurrentStatus == EmojiName.WHITECHECKMARK)
                {
                    finalCheckMark = EnumExtensions.GetEnumMemberAttrValue(EmojiName.WHITECHECKMARK);
                }
                else if (reportObject.CurrentStatus == EmojiName.YELLOWSQUARE)
                {
                    finalCheckMark = EnumExtensions.GetEnumMemberAttrValue(EmojiName.YELLOWSQUARE);
                }
                else
                {
                    finalCheckMark = EnumExtensions.GetEnumMemberAttrValue(reportObject.CachedDefaultStatus);
                }

                Log.WriteLine("Found: " + nameof(reportObject) + " with values: " +
                    reportObject.FieldNameDisplay + ", " + reportObject.ObjectValue + ", " +
                    reportObject.CurrentStatus + ", " + reportObject.CachedDefaultStatus.ToString() + ", with" +
                    finalCheckMark, LogLevel.DEBUG);

                reportingStatusPerTeam += finalCheckMark + " " + reportObject.FieldNameDisplay + ": " +
                    reportObject.ObjectValue + "\n";
            }

            Log.WriteLine("Done looping through team: " + teamKvp.Key + " (" + teamKvp.Value +
                ")" + "with message: " + reportingStatusPerTeam, LogLevel.VERBOSE);
            reportingStatusMessage += reportingStatusPerTeam + "\n";
        }

        Log.WriteLine("Returning: " + reportingStatusMessage, LogLevel.DEBUG);

        return reportingStatusMessage;
    }
}