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


    [DataMember] protected ButtonName buttonName;

    public BaseButton()
    {
    }

    public abstract void TempMethod();
}