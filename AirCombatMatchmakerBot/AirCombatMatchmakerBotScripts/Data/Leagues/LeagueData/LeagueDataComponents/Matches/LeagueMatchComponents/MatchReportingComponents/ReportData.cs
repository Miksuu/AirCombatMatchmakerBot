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
    [DataMember] private logConcurrentBag<BaseReportingObject> reportingObjects = new logConcurrentBag<BaseReportingObject>();
    [DataMember] private logVar<float> finalEloDelta = new logVar<float>();
    [DataMember] private logVar<bool> confirmedMatch = new logVar<bool>();

    public ReportData() { }

    public ReportData(string _reportingTeamName, ConcurrentBag<Player> _players)
    {
        TeamName = _reportingTeamName;

        foreach (TypeOfTheReportingObject typeOfTheReportingObject in Enum.GetValues(typeof(TypeOfTheReportingObject)))
        {
            Log.WriteLine(typeOfTheReportingObject.ToString(), LogLevel.DEBUG);
            var reportingObjectInstance = EnumExtensions.GetInstance(typeOfTheReportingObject.ToString());
            Log.WriteLine(reportingObjectInstance.ToString(), LogLevel.DEBUG);
            var re = (InterfaceReportingObject)reportingObjectInstance;

            Log.WriteLine(re.ToString(), LogLevel.DEBUG);
            ReportingObjects.Add(re as BaseReportingObject);
        }

        foreach (var player in _players)
        {
            Log.WriteLine("Adding player: " + player.PlayerDiscordId);
            var reportingObject = reportingObjects.FirstOrDefault(x => x.GetTypeOfTheReportingObject() == TypeOfTheReportingObject.PLAYERPLANE) as PLAYERPLANE;
            //var newReportObject = new ReportObject("Plane", EmojiName.REDSQUARE, true);
            reportingObject.TeamMemberIdsWithSelectedPlanesByTheTeam.TryAdd(player.PlayerDiscordId, UnitName.NOTSELECTED);
        }
    }

    public BaseReportingObject FindBaseReportingObjectOfType(TypeOfTheReportingObject _typeOfTheReportingObject)
    {
        Log.WriteLine("Finding: " + _typeOfTheReportingObject);
        var result = ReportingObjects.FirstOrDefault(x => x.GetTypeOfTheReportingObject() == _typeOfTheReportingObject);
        if (result == null)
        {
            Log.WriteLine(nameof(_typeOfTheReportingObject) + " was null!", LogLevel.ERROR);
            throw new InvalidOperationException(nameof(_typeOfTheReportingObject) + " was null!");
        }
        Log.WriteLine("Found: " + result.GetTypeOfTheReportingObject());
        return result;
    }
}