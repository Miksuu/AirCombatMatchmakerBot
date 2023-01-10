using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;
using System.Reflection.Emit;

[DataContract]
public abstract class BaseButton : InterfaceButton
{
    ButtonName InterfaceButton.ButtonName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(buttonName) + ": " +
                buttonName, LogLevel.VERBOSE);
            return buttonName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(buttonName) + buttonName
                + " to: " + value, LogLevel.VERBOSE);
            buttonName = value;
        }
    }

    string InterfaceButton.ButtonLabel
    {
        get
        {
            Log.WriteLine("Getting " + nameof(buttonLabel) + ": " +
                buttonLabel, LogLevel.VERBOSE);
            return buttonLabel;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(buttonLabel) + buttonLabel
                + " to: " + value, LogLevel.VERBOSE);
            buttonLabel = value;
        }
    }

    ButtonStyle InterfaceButton.ButtonStyle
    {
        get
        {
            Log.WriteLine("Getting " + nameof(buttonStyle) + ": " +
                buttonStyle, LogLevel.VERBOSE);
            return buttonStyle;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(buttonStyle) + buttonStyle
                + " to: " + value, LogLevel.VERBOSE);
            buttonStyle = value;
        }
    }

    ulong InterfaceButton.ButtonCategoryId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(buttonCategoryId)
                + ": " + buttonCategoryId, LogLevel.VERBOSE);
            return buttonCategoryId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(buttonCategoryId) +
                buttonCategoryId + " to: " + value, LogLevel.VERBOSE);
            buttonCategoryId = value;
        }
    }

    string InterfaceButton.ButtonCustomId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(buttonCustomId) + ": " +
                buttonCustomId, LogLevel.VERBOSE);
            return buttonCustomId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(buttonCustomId) + buttonCustomId
                + " to: " + value, LogLevel.VERBOSE);
            buttonCustomId = value;
        }
    }

    [DataMember] protected ButtonName buttonName;
    [DataMember] protected string buttonLabel = "";
    [DataMember] protected ButtonStyle buttonStyle;
    [DataMember] protected ulong buttonCategoryId;
    [DataMember] protected string buttonCustomId = "";

    public Discord.ButtonBuilder CreateTheButton(string _customId, int _buttonIndex, ulong _buttonCategoryId, string _messageKey)
    {
        Log.WriteLine("Creating a button: " + buttonName + " | label: " +
            buttonLabel + " | custom-id:" + _customId + " with style: " +
            buttonStyle + " | category-id: " + _buttonCategoryId, LogLevel.VERBOSE);

        // Report score specific stuff, add the index as label
        if (buttonName == ButtonName.REPORTSCOREBUTTON)
        {
            buttonLabel = _buttonIndex.ToString();
            Log.WriteLine("is: " + nameof(buttonName) +
                " set label to: " + buttonLabel, LogLevel.VERBOSE);
        }

        // Create the button to match the league category id, for easier later referencing
        if (buttonName == ButtonName.LEAGUEREGISTRATIONBUTTON)
        {
            _customId = _messageKey + "_" + _buttonIndex;
            Log.WriteLine("Setting league-registration button custom id to: " + 
                _customId, LogLevel.DEBUG);
        }

        // Insert the button category id for faster reference later
        buttonCategoryId = _buttonCategoryId;
        buttonCustomId = _customId;

        var button = new Discord.ButtonBuilder()
        {
            Label = buttonLabel,
            CustomId = _customId,
            Style = buttonStyle,
        };

        return button;
    }

    public abstract Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, ulong _channelId,
        ulong _messageId, string _message);
}