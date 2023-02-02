﻿using System.Runtime.Serialization;

[DataContract]
public class ReportObject
{
    public string? FieldNameDisplay
    {
        get
        {
            Log.WriteLine("Getting " + nameof(fieldNameDisplay), LogLevel.VERBOSE);
            return fieldNameDisplay;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(fieldNameDisplay)
                + " to: " + value, LogLevel.VERBOSE);
            fieldNameDisplay = value;
        }
    }

    public string? ObjectValue
    {
        get
        {
            Log.WriteLine("Getting " + nameof(objectValue), LogLevel.VERBOSE);
            return objectValue;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(objectValue)
                + " to: " + value, LogLevel.VERBOSE);
            objectValue = value;
        }
    }

    public bool FieldFilled
    {
        get
        {
            Log.WriteLine("Getting " + nameof(fieldFilled), LogLevel.VERBOSE);
            return fieldFilled;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(fieldFilled)
                + " to: " + value, LogLevel.VERBOSE);
            fieldFilled = value;
        }
    }

    public EmojiName DefaultStateEmoji
    {
        get
        {
            Log.WriteLine("Getting " + nameof(defaultStateEmoji), LogLevel.VERBOSE);
            return defaultStateEmoji;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(defaultStateEmoji)
                + " to: " + value, LogLevel.VERBOSE);
            defaultStateEmoji = value;
        }
    }

    [DataMember] private string? fieldNameDisplay { get; set; }
    [DataMember] private string? objectValue { get; set; }
    [DataMember] private bool fieldFilled { get; set; }
    [DataMember] private EmojiName defaultStateEmoji { get; set; }

    public ReportObject()
    {
    }

    public ReportObject(string _fieldNameDisplay, EmojiName _defaultStateEmoji)
    {
        fieldNameDisplay = _fieldNameDisplay;
        defaultStateEmoji = _defaultStateEmoji;
    }

    public void SetObjectValueAndFieldBool(string _value, bool _fieldBool)
    {
        Log.WriteLine("Setting " + nameof(ReportObject) + "'s value to: " + _value
            + " with bool: " + _fieldBool, LogLevel.VERBOSE);

        objectValue = _value;
        fieldFilled = _fieldBool;
    }
}