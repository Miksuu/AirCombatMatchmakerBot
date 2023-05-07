using System.Runtime.Serialization;
using Discord;

[DataContract]
public class RAWMESSAGEINPUT : BaseMessage
{
    public RAWMESSAGEINPUT()
    {
        messageName = MessageName.RAWMESSAGEINPUT;

        /*
        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.REPORTSCOREBUTTON, 4),
            });*/

        thisInterfaceMessage.MessageDescription = "Empty message"; 
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override string GenerateMessage()
    {
        if (thisInterfaceMessage.MessageDescription == null)
        {
            Log.WriteLine("MessageDescription was null!", LogLevel.CRITICAL);
            return "MessageDescription was null!";
        }

        return thisInterfaceMessage.MessageDescription;
    }

    public string GenerateRawMessage(string _input, string _embedTitle = "")
    {
        Log.WriteLine("Generating a raw message with input: " + _input +
            " and title: " + _embedTitle, LogLevel.VERBOSE);
        thisInterfaceMessage.MessageEmbedTitle = _embedTitle;
        thisInterfaceMessage.MessageDescription = _input;
        return thisInterfaceMessage.MessageDescription;
    }
}