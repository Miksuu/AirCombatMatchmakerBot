using System.Runtime.Serialization;

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

    public bool FieldIsOptional
    {
        get
        {
            Log.WriteLine("Getting " + nameof(fieldIsOptional), LogLevel.VERBOSE);
            return fieldIsOptional;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(fieldIsOptional)
                + " to: " + value, LogLevel.VERBOSE);
            fieldIsOptional = value;
        }
    }

    [DataMember] private string? fieldNameDisplay { get; set; }
    [DataMember] private string? objectValue { get; set; }
    [DataMember] private bool fieldFilled { get; set; }
    [DataMember] private bool fieldIsOptional { get; set; }

    public ReportObject()
    {
    }

    public ReportObject(string _fieldNameDisplay, bool _fieldIsOptional)
    {
        fieldNameDisplay = _fieldNameDisplay;
        fieldIsOptional = _fieldIsOptional;
    }

    public void SetObjectValueAndFieldBool(string _value, bool _fieldBool)
    {
        Log.WriteLine("Setting " + nameof(ReportObject) + "'s value to: " + _value
            + " with bool: " + _fieldBool, LogLevel.VERBOSE);

        ObjectValue = _value;
        FieldFilled = _fieldBool;
    }
}