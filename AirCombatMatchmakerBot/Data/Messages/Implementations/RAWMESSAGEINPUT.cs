using System.Runtime.Serialization;
using System.Collections.Concurrent;

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

        messageDescription = "Empty message"; 
    }

    public override string GenerateMessage()
    {
        return messageDescription;
    }

    public string GenerateRawMessage(string _input, string _embedTitle = "")
    {
        Log.WriteLine("Generating a raw message with input: " + _input +
            " and title: " + _embedTitle, LogLevel.VERBOSE);
        messageEmbedTitle = _embedTitle;
        messageDescription = _input;
        return messageDescription;
    }
}