﻿using System.Runtime.Serialization;

[DataContract]
public class ReportData
{
    public string TeamName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(teamName), LogLevel.VERBOSE);
            return teamName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(teamName)
                + " to: " + value, LogLevel.VERBOSE);
            teamName = value;
        }
    }

    public ReportObject ReportedScore
    {
        get
        {
            Log.WriteLine("Getting " + nameof(reportedScore), LogLevel.VERBOSE);
            return reportedScore;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(reportedScore)
                + " to: " + value, LogLevel.VERBOSE);
            reportedScore = value;
        }
    }

    public ReportObject TacviewLink
    {
        get
        {
            Log.WriteLine("Getting " + nameof(tacviewLink), LogLevel.VERBOSE);
            return tacviewLink;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(tacviewLink)
                + " to: " + value, LogLevel.VERBOSE);
            tacviewLink = value;
        }
    }

    public ReportObject CommentByTheUser
    {
        get
        {
            Log.WriteLine("Getting " + nameof(commentByTheUser), LogLevel.VERBOSE);
            return commentByTheUser;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(commentByTheUser)
                + " to: " + value, LogLevel.VERBOSE);
            commentByTheUser = value;
        }
    }

    /*
    public float CachedSkillRating
    {
        get
        {
            Log.WriteLine("Getting " + nameof(cachedSkillRating) + ": " + cachedSkillRating, LogLevel.VERBOSE);
            return cachedSkillRating;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(cachedSkillRating)
                + " to: " + value, LogLevel.VERBOSE);
            cachedSkillRating = value;
        }
    }*/

    public float FinalEloDelta
    {
        get
        {
            Log.WriteLine("Getting " + nameof(finalEloDelta) + ": " + finalEloDelta, LogLevel.VERBOSE);
            return finalEloDelta;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(finalEloDelta)
                + " to: " + value, LogLevel.VERBOSE);
            finalEloDelta = value;
        }
    }

    public bool ConfirmedMatch
    {
        get
        {
            Log.WriteLine("Getting " + nameof(confirmedMatch) + ": " + confirmedMatch, LogLevel.VERBOSE);
            return confirmedMatch;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(confirmedMatch)
                + " to: " + value, LogLevel.VERBOSE);
            confirmedMatch = value;
        }
    }

    [DataMember] private string teamName { get; set; }
    [DataMember] private ReportObject reportedScore { get; set; }
    [DataMember] private ReportObject tacviewLink { get; set; }
    [DataMember] private ReportObject commentByTheUser { get; set; }
    //[DataMember] private float cachedSkillRating { get; set; }
    [DataMember] private float finalEloDelta { get; set; }
    [DataMember] private bool confirmedMatch { get; set; }

    public ReportData(string _reportingTeamName)
    {
        teamName = _reportingTeamName;      
        reportedScore = new ReportObject("Reported score", EmojiName.REDSQUARE);
        tacviewLink = new ReportObject("Tacview link", EmojiName.REDSQUARE);
        commentByTheUser = new ReportObject("Comment", EmojiName.YELLOWSQUARE);
    }
}