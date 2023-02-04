using System.Runtime.Serialization;

public class CommandOption
{
    public string? OptionName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(optionName), LogLevel.VERBOSE);
            return optionName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(optionName)
                + " to: " + value, LogLevel.VERBOSE);
            optionName = value;
        }
    }

    public string? OptionDescription
    {
        get
        {
            Log.WriteLine("Getting " + nameof(optionDescription), LogLevel.VERBOSE);
            return optionDescription;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(optionDescription)
                + " to: " + value, LogLevel.VERBOSE);
            optionDescription = value;
        }
    }

    private string? optionName = "";
    private string? optionDescription = "";

    public CommandOption()
    {
    }

    public CommandOption(string _optionName, string _optionDescription)
    {
        optionName = _optionName.ToLower();
        optionDescription = _optionDescription;
    }
}