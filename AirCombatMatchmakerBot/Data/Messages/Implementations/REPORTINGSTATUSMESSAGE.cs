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
            var tacviewLink = teamReportData.TacviewLink;

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

                // Add any other non tuple here, kinda temp fix
                if (field.FieldType == typeof(string)) continue;

                Log.WriteLine("This is tuple field: " + field.FieldType, LogLevel.VERBOSE);
                
                if (field.FieldType.IsAssignableFrom((1, false).GetType()))
                {
                    Log.WriteLine("Type is tuple<int, bool>", LogLevel.VERBOSE);
                    GenerateTuple(field, teamReportData, typeof(int));
                }

                if (field.FieldType.IsAssignableFrom(("asd", false).GetType()))
                {
                    Log.WriteLine("Type is tuple<string, bool>", LogLevel.VERBOSE);
                    //Log.WriteLine(field.FieldType.IsAssignableFrom(("asd", false).GetType()).ToString(), LogLevel.DEBUG);
                    GenerateTuple(field, teamReportData, typeof(string));
                }
            }

            Log.WriteLine("Done looping through team: " + teamKvp.Key + " (" + teamKvp.Value +
                ")" + "with message: " + reportingStatusPerTeam, LogLevel.VERBOSE);
            reportingStatusMessage += reportingStatusPerTeam + "\n";
        }

        Log.WriteLine("Returning: " + reportingStatusMessage, LogLevel.DEBUG);

        return reportingStatusMessage;
    }

    public override bool GenerateTuple(FieldInfo _field, ReportData _reportData, Type _type)
    {
        var newField = _field;
        Tuple<typeof(_type), bool> newTuple = (Tuple<_type, bool>)newField.GetValue(_reportData);

        Log.WriteLine("Final tuple: " + newTuple.Item1 + " | " + newTuple.Item2, LogLevel.DEBUG);
        /*
        var dataMember = _field.GetCustomAttribute<DataMemberAttribute>();
        if (dataMember == null)
        {
            Log.WriteLine(dataMember + " was null!", LogLevel.CRITICAL);
            return false;
        }
        
        
        var fieldValue = _field.GetValue(_field);
        if (fieldValue == null)
        {
            Log.WriteLine(nameof(fieldValue) + " was null!", LogLevel.CRITICAL);
            return false;
        }

        string? fieldValueString = fieldValue.ToString();
        if (fieldValueString == null)
        {
            Log.WriteLine(nameof(fieldValue) + "string was null!", LogLevel.CRITICAL);
            return false;
        }

        //Log.WriteLine("tupleType: " + _tupleType, LogLevel.DEBUG);
        Tuple<T, bool> instanceValue = (Tuple<T, bool>)EnumExtensions.GetInstance(_tupleType.ToString());

        Log.WriteLine(nameof(instanceValue) + " type: " + instanceValue.GetType(), LogLevel.DEBUG);

        var itemOneValueInString = instanceValue.Item1.ToString();
        var itemtwo = instanceValue.Item2.ToString();
        var itemtwobool = bool.Parse(itemtwo);

        var tupleInstanceValue = new Tuple<string, bool>(itemOneValueInString, itemtwobool);

        Log.WriteLine("Final tuple: " + tupleInstanceValue.Item1 + " | " + tupleInstanceValue.Item2, LogLevel.DEBUG);
        */
        return true;
    }
}