using System.Runtime.Serialization;
using Discord;

[DataContract]
public class RAWMESSAGEINPUT : BaseMessage
{
    public RAWMESSAGEINPUT()
    {
        thisInterfaceMessage.MessageName = MessageName.RAWMESSAGEINPUT;

        /*
        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.REPORTSCOREBUTTON, 4),
            });*/

        thisInterfaceMessage.MessageDescription = "Empty message";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _channelCategoryId)
    {
        base.GenerateRegularButtons(_component, _channelCategoryId);
    }

    public override Task<MessageComponents> GenerateMessage(ulong _channelCategoryId = 0)
    {
        if (thisInterfaceMessage.MessageDescription == null)
        {
            Log.WriteLine("MessageDescription was null!", LogLevel.ERROR);
            return Task.FromResult(new MessageComponents("MessageDescription was null!"));
        }

        return Task.FromResult(new MessageComponents(thisInterfaceMessage.MessageDescription));
    }

    public string GenerateRawMessage(string _input, string _embedTitle = "")
    {
        Log.WriteLine("Generating a raw message with input: " + _input +
            " and title: " + _embedTitle);
        thisInterfaceMessage.MessageEmbedTitle = _embedTitle;
        thisInterfaceMessage.MessageDescription = _input;
        return thisInterfaceMessage.MessageDescription;
    }

    public override string GenerateMessageFooter()
    {
        return "";
    }
}