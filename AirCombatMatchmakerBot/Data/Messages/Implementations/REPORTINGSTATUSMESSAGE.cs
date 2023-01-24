using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;
using System.Threading.Channels;
using System.Reflection;
using System.Runtime.CompilerServices;

[DataContract]
public class REPORTINGSTATUSMESSAGE : BaseMessage
{
    public REPORTINGSTATUSMESSAGE()
    {
        messageName = MessageName.REPORTINGSTATUSMESSAGE;
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>();
        message = "Insert the reporting status message here";
    }

    public override string GenerateMessage()
    {
        string reportingStatusMessage = "Current reporting status: \n";

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
            string reportingStatusPerTeam = teamKvp.Value + ": ";

            if (!foundMatch.MatchReporting.TeamIdsWithReportData.ContainsKey(teamKvp.Key))
            {
                Log.WriteLine("Does not contain reporting data on: " + teamKvp.Key + " named: " +
                    teamKvp.Value, LogLevel.CRITICAL);
                continue;
            }



            /*
            // Does not contain the reporting result, just add "none"
            else
            {
                reportingStatusPerTeam += "Not reported yet";
            }*/

            var teamReportData = foundMatch.MatchReporting.TeamIdsWithReportData[teamKvp.Key];
            //var reportedResult = teamReportData.ReportedResult;
            //var tacviewLink = teamReportData.TacviewLink;
            if (teamReportData == null)
            {
                Log.WriteLine(nameof(teamReportData) + " was null!", LogLevel.CRITICAL);
                continue;
            }

            //reportingStatusPerTeam += "**" + teamReportData.TeamName.ToString() + "**\n";

            /*
            Log.WriteLine("Found team's: " + teamKvp.Key + " (" + teamKvp.Value + ")" +
                " reported result: " + reportedResult, LogLevel.VERBOSE);

            reportingStatusPerTeam += reportedResult; */
            //if (tacviewLink != "") reportingStatusPerTeam += " | " + tacviewLink;

            FieldInfo[] fields = typeof(ReportData).GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

            Log.WriteLine("fields count: " + fields.Length, LogLevel.DEBUG);

            foreach (FieldInfo field in fields)
            {
                Log.WriteLine("field type: " + field.FieldType, LogLevel.DEBUG);

                // Only process the ReportObject fields (ignore teamName)
                if (field.FieldType != typeof(ReportObject)) continue;

                Log.WriteLine("This is " + nameof(ReportObject) + " field: " +
                    field.FieldType, LogLevel.VERBOSE);

                ReportObject reportObject = (ReportObject)field.GetValue(teamReportData);
                if (reportObject == null)
                {
                    Log.WriteLine(nameof(reportObject) + " was null!", LogLevel.CRITICAL);
                    continue;
                }

                Log.WriteLine("Found: " + nameof(reportObject) + " with values: " +
                    reportObject.FieldNameDisplay + ", " +reportObject.ObjectValue + ", " + 
                    reportObject.FieldFilled, LogLevel.DEBUG);

                reportingStatusPerTeam += reportObject.FieldNameDisplay + ": " +
                    reportObject.ObjectValue + " | " + reportObject.FieldFilled + "\n";


                /*
                if (field.FieldType.IsAssignableFrom((1, false).GetType()))
                {
                    Log.WriteLine("Type is tuple<int, bool>", LogLevel.VERBOSE);
                    GenerateTuple(field, teamReportData, typeof(int));
                }

                if (field.FieldType.IsAssignableFrom(("asd", false).GetType()))
                {
                    Log.WriteLine("Type is tuple<string, bool>", LogLevel.VERBOSE);
                    GenerateTuple(field, teamReportData, typeof(string));
                }*/
            }

            Log.WriteLine("Done looping through team: " + teamKvp.Key + " (" + teamKvp.Value +
                ")" + "with message: " + reportingStatusPerTeam, LogLevel.VERBOSE);
            reportingStatusMessage += reportingStatusPerTeam + "\n";
        }

        Log.WriteLine("Returning: " + reportingStatusMessage, LogLevel.DEBUG);

        return reportingStatusMessage;
    }
}