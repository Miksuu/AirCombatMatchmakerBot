using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;

[DataContract]
public class ReportData
{
    public string TeamName
    {
        get => teamName.GetValue();
        set => teamName.SetValue(value);
    }

    
    public ConcurrentBag<BaseReportingObject> ReportingObjects
    {
        get => reportingObjects.GetValue();
        set => reportingObjects.SetValue(value);
    }

    public float FinalEloDelta
    {
        get => finalEloDelta.GetValue();
        set => finalEloDelta.SetValue(value);
    }

    public bool ConfirmedMatch
    {
        get => confirmedMatch.GetValue();
        set => confirmedMatch.SetValue(value);
    }

    [DataMember] private logString teamName = new logString();
    /*
    [DataMember] private logConcurrentDictionary<ulong, ReportObject> teamMemberIdsWithSelectedPlanesByTheTeam = 
        new logConcurrentDictionary<ulong, ReportObject>();*/
    [DataMember] private logConcurrentBag<BaseReportingObject> reportingObjects = new logConcurrentBag<BaseReportingObject>();
    [DataMember] private logClass<float> finalEloDelta = new logClass<float>();
    [DataMember] private logClass<bool> confirmedMatch = new logClass<bool>();

    public ReportData() { }

    public ReportData(string _reportingTeamName, ConcurrentBag<Player> _players)
    {
        TeamName = _reportingTeamName;

        foreach (TypeOfTheReportingObject typeOfTheReportingObject in Enum.GetValues(typeof(TypeOfTheReportingObject)))
        {
            /*
            string typeOfTheReportingObjectString = EnumExtensions.GetEnumMemberAttrValue(typeOfTheReportingObject);
            Log.WriteLine("typeOfTheReportingObjectString after enumValueCheck: " + typeOfTheReportingObjectString, LogLevel.VERBOSE);
            if (typeOfTheReportingObjectString == null)
            {
                Log.WriteLine(nameof(typeOfTheReportingObjectString) + " was null!", LogLevel.CRITICAL);
                continue;
            }*/

            var reportingObjectInstance = TypeOfTheReportingObject)EnumExtensions.GetInstance(typeOfTheReportingObject.ToString());

            Log.WriteLine(reportingObjectInstance.ToString(), LogLevel.DEBUG);

            /*
            Log.WriteLine("Starting to create a league join button for: " + leagueNameString, LogLevel.VERBOSE);

            Log.WriteLine(nameof(leagueInterfaceFromDatabase) + " before creating leagueButtonRegisterationCustomId: "
                + leagueInterfaceFromDatabase.ToString(), LogLevel.VERBOSE);

            if (leagueInterfaceFromDatabase.LeagueRegistrationMessageId != 0) continue;

            InterfaceMessage interfaceMessage =
                (InterfaceMessage)EnumExtensions.GetInstance(MessageName.LEAGUEREGISTRATIONMESSAGE.ToString());

            var newInterfaceMessage = await interfaceMessage.CreateTheMessageAndItsButtonsOnTheBaseClass(
                    _client, this, true, true, leagueInterfaceFromDatabase.LeagueCategoryId);

            leagueInterfaceFromDatabase.LeagueRegistrationMessageId = interfaceMessage.MessageId;

            thisInterfaceChannel.InterfaceMessagesWithIds.TryAdd(
                leagueInterfaceFromDatabase.LeagueCategoryId,
                    (InterfaceMessage)EnumExtensions.GetInstance(MessageName.LEAGUEREGISTRATIONMESSAGE.ToString()));

            Log.WriteLine("Added to the ConcurrentDictionary, count is now: " +
                thisInterfaceChannel.InterfaceMessagesWithIds.Count, LogLevel.VERBOSE);

            Log.WriteLine("Done looping on: " + leagueNameString, LogLevel.VERBOSE);*/
        }

        
        foreach (var player in _players)
        {
            Log.WriteLine("Adding player: " + player.PlayerDiscordId, LogLevel.VERBOSE);
            var reportingObject = reportingObjects.FirstOrDefault(x => x.GetTypeOfTheReportingObject() == TypeOfTheReportingObject.PLAYERPLANE) as PLAYERPLANE;
            //var newReportObject = new ReportObject("Plane", EmojiName.REDSQUARE, true);
            reportingObject.TeamMemberIdsWithSelectedPlanesByTheTeam.TryAdd(player.PlayerDiscordId, UnitName.NOTSELECTED);
        }

        /*
        Log.WriteLine("done adding players: " + TeamMemberIdsWithSelectedPlanesByTheTeam.Count +
            " on team: " + _reportingTeamName, LogLevel.VERBOSE);-*/
    }


}