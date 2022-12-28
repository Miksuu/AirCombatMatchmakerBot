using Discord.WebSocket;
using Discord;
using System.Runtime.Serialization;

[DataContract]
public abstract class BaseMessage : InterfaceMessage
{
    MessageName InterfaceMessage.MessageName
    {
        get
        {
            Log.WriteLine("Getting " + nameof(messageName) + 
                ": " + messageName, LogLevel.VERBOSE);
            return messageName;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(messageName) + messageName
                + " to: " + value, LogLevel.VERBOSE);
            messageName = value;
        }
    }
    bool InterfaceMessage.ShowOnChannelGeneration
    {
        get
        {
            Log.WriteLine("Getting " + nameof(showOnChannelGeneration) 
                + ": " + showOnChannelGeneration, LogLevel.VERBOSE);
            return showOnChannelGeneration;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(showOnChannelGeneration) +
                showOnChannelGeneration + " to: " + value, LogLevel.VERBOSE);
            showOnChannelGeneration = value;
        }
    }

    List<ButtonName> MessageButtonNames
    {
        get
        {
            Log.WriteLine("Getting " + nameof(messageButtonNames) + " with count of: " +
                messageButtonNames.Count, LogLevel.VERBOSE);
            return messageButtonNames;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(messageButtonNames)
                + " to: " + value, LogLevel.VERBOSE);
            messageButtonNames = value;
        }
    }

    [DataMember] protected MessageName messageName;
    [DataMember] protected bool showOnChannelGeneration;
    [DataMember] protected List<ButtonName> messageButtonNames;

    public BaseMessage()
    {
        messageButtonNames = new List<ButtonName>();
    }

    public abstract void TempMethod();
}