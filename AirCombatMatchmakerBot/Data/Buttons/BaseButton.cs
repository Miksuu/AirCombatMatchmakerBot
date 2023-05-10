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
                buttonName, LogLevel.GET_VERBOSE);
            return buttonName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(buttonName) + buttonName
                + " to: " + value, LogLevel.SET_VERBOSE);
            buttonName = value;
        }
    }

    string InterfaceButton.ButtonLabel
    {
        get => buttonLabel.GetValue();
        set => buttonLabel.SetValue(value);
    }

    ButtonStyle InterfaceButton.ButtonStyle
    {
        get
        {
            Log.WriteLine("Getting " + nameof(buttonStyle) + ": " +
                buttonStyle, LogLevel.GET_VERBOSE);
            return buttonStyle;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(buttonStyle) + buttonStyle
                + " to: " + value, LogLevel.SET_VERBOSE);
            buttonStyle = value;
        }
    }

    ulong InterfaceButton.ButtonCategoryId
    {
        get => buttonCategoryId.GetValue();
        set => buttonCategoryId.SetValue(value);
    }

    string InterfaceButton.ButtonCustomId
    {
        get => buttonCustomId.GetValue();
        set => buttonCustomId.SetValue(value);
    }


    bool InterfaceButton.EphemeralResponse
    {
        get
        {
            Log.WriteLine("Getting " + nameof(ephemeralResponse)
                + ": " + ephemeralResponse, LogLevel.GET_VERBOSE);
            return ephemeralResponse;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(ephemeralResponse) +
                ephemeralResponse + " to: " + value, LogLevel.SET_VERBOSE);
            ephemeralResponse = value;
        }
    }

    [DataMember] protected ButtonName buttonName;
    [DataMember] protected logString buttonLabel = new logString();
    [DataMember] protected ButtonStyle buttonStyle;
    [DataMember] protected logUlong buttonCategoryId = new logUlong();
    [DataMember] protected logString buttonCustomId = new logString();
    protected bool ephemeralResponse = false;
    protected InterfaceButton thisInterfaceButton;

    public BaseButton()
    {
        thisInterfaceButton = this;
    }

    public Discord.ButtonBuilder CreateTheButton(
        string _customId, int _buttonIndex, ulong _buttonCategoryId,
        ulong _leagueCategoryId = 0)
    {
        Log.WriteLine("Creating a button: " + buttonName + " | label: " +
            thisInterfaceButton.ButtonLabel + " | custom-id:" + _customId + " with style: " +
            buttonStyle + " | category-id: " + _buttonCategoryId + " with buttonIndex:" +
            _buttonIndex, LogLevel.VERBOSE);

        //buttonIndex = _buttonIndex;

        string tempCustomId = GenerateCustomButtonProperties(_buttonIndex, _leagueCategoryId);
        Log.WriteLine("tempCustomId: " + tempCustomId, LogLevel.VERBOSE);

        if (tempCustomId != "")
        {
            Log.WriteLine("Button had " + nameof(GenerateCustomButtonProperties) + " generated for it.", LogLevel.VERBOSE);
            _customId = tempCustomId;
        }

        Log.WriteLine("_customId: " + _customId, LogLevel.VERBOSE);

        // Insert the button category id for faster reference later
        thisInterfaceButton.ButtonCategoryId = _buttonCategoryId;
        thisInterfaceButton.ButtonCustomId = _customId;

        var button = new Discord.ButtonBuilder()
        {
            Label = thisInterfaceButton.ButtonLabel,
            CustomId = _customId,
            Style = buttonStyle,
        };

        return button;
    }

    protected abstract string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId);

    public abstract Task<Response> ActivateButtonFunction(
         SocketMessageComponent _component, InterfaceMessage _interfaceMessage);
}