using System.Runtime.Serialization;
using System.Collections.Concurrent;
using Discord;

[DataContract]
public class REPORTINGMESSAGE : BaseMessage
{
    public REPORTINGMESSAGE()
    {
        messageName = MessageName.REPORTINGMESSAGE;

        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>(
            new ConcurrentBag<KeyValuePair<ButtonName, int>>()
            {
                new KeyValuePair<ButtonName, int>(ButtonName.REPORTSCOREBUTTON, 4),
            });

        messageEmbedTitle = "Report match score";
        messageDescription = "After the match has been completed, click on the buttons below to report your score.\n" +
            "Upload your Tacview by dragging it to the window and post it.\n" +
            "Optionally, you can use the /comment command to post a comment on the match.";
    }

    protected override void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        base.GenerateRegularButtons(_component, _leagueCategoryId);
    }

    public override string GenerateMessage()
    {
        if (messageDescription == null)
        {
            Log.WriteLine("messageDescription was null!", LogLevel.CRITICAL);
            return "messageDescription was null!";
        }

        return messageDescription;
    }
}