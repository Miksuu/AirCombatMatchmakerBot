using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;

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

    /*
    ulong InterfaceButton.ChannelId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(channelId) + ": " +
                channelId, LogLevel.VERBOSE);
            return channelId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(channelId) + channelId
                + " to: " + value, LogLevel.VERBOSE);
            channelId = value;
        }
    }

    ulong InterfaceButton.MessageId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(messageId) + ": " +
                messageId, LogLevel.VERBOSE);
            return messageId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(messageId) + messageId
                + " to: " + value, LogLevel.VERBOSE);
            messageId = value;
        }
    }*/

    [DataMember] protected ButtonName buttonName;
    [DataMember] protected string buttonLabel = "";
    [DataMember] protected ButtonStyle buttonStyle;
    // Need to know these for message updating (from button function itself)
    //[DataMember] protected ulong channelId;
    //[DataMember] protected ulong messageId;

    public Discord.ButtonBuilder CreateTheButton(string _customId)
    {
        Log.WriteLine("Creating a button: " + buttonName + " | label: " +
            buttonLabel + " | custom-id:" + _customId + " with style: " + buttonStyle, LogLevel.VERBOSE);

        var button = new Discord.ButtonBuilder()
        {
            Label = buttonLabel,
            CustomId = buttonName.ToString() + "_" + _customId,
            Style = buttonStyle,
        };

        return button;
    }

    public abstract Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, string _splitString, ulong _channelId, ulong _messageId);
}