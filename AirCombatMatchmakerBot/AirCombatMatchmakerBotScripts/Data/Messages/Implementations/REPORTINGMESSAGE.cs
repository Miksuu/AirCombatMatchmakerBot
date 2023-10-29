using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class REPORTINGMESSAGE : BaseMessage
{
    public REPORTINGMESSAGE()
    {
        thisInterfaceMessage.MessageName = MessageName.REPORTINGMESSAGE;

        thisInterfaceMessage.MessageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.REPORTSCOREBUTTON, 4),
            });

        thisInterfaceMessage.MessageEmbedTitle = "Report match score";
        thisInterfaceMessage.MessageDescription = "After the match has been completed, click on the buttons below to report your score.\n" +
            "Upload your Tacview by dragging it to the window and post it.\n" +
            "Optionally, you can use the /comment command to post a comment on the match, and write " + @"""" + "-" + @"""" + " to the command to " +
            "remove your comment";
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
            return Task.FromResult("MessageDescription was null!");
        }

        return Task.FromResult(thisInterfaceMessage.MessageDescription);
    }

    public override string GenerateMessageFooter()
    {
        return "";
    }
}