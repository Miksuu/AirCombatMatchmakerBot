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

    [DataMember] private string? fieldNameDisplay { get; set; }
    [DataMember] private string? objectValue { get; set; }
    [DataMember] private bool fieldFilled { get; set; }

    public ReportObject()
    {
    }

    public ReportObject(string _fieldNameDisplay)
    {
        fieldNameDisplay = _fieldNameDisplay;
    }

    public void SetObjectValueAndFieldBool(string _value, bool _fieldBool)
    {
        Log.WriteLine("Setting " + nameof(ReportObject) + "'s value to: " + _value
            + " with bool: " + _fieldBool, LogLevel.VERBOSE);

        ObjectValue = _value;
        FieldFilled = _fieldBool;
    }
}