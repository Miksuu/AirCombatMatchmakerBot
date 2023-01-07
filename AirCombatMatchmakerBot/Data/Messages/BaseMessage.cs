using Discord;
using System.Runtime.Serialization;
using System.Threading.Channels;

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

    Dictionary<ButtonName, int> InterfaceMessage.MessageButtonNamesWithAmount
    {
        get
        {
            Log.WriteLine("Getting " + nameof(messageButtonNamesWithAmount) + " with count of: " +
                messageButtonNamesWithAmount.Count, LogLevel.VERBOSE);
            return messageButtonNamesWithAmount;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(messageButtonNamesWithAmount)
                + " to: " + value, LogLevel.VERBOSE);
            messageButtonNamesWithAmount = value;
        }
    }

    string InterfaceMessage.Message
    {
        get
        {
            Log.WriteLine("Getting " + nameof(message)
                + ": " + message, LogLevel.VERBOSE);
            return message;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(message) +
                message + " to: " + value, LogLevel.VERBOSE);
            message = value;
        }
    }

    ulong InterfaceMessage.MessageId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(messageId)
                + ": " + messageId, LogLevel.VERBOSE);
            return messageId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(messageId) +
                messageId + " to: " + value, LogLevel.VERBOSE);
            messageId = value;
        }
    }

    [DataMember] protected MessageName messageName;
    [DataMember] protected bool showOnChannelGeneration;
    [DataMember] protected Dictionary<ButtonName, int> messageButtonNamesWithAmount;
    [DataMember] protected string message = "";
    [DataMember] protected ulong messageId;

    public BaseMessage()
    {
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>();
    }

    public async Task<ulong> CreateTheMessageAndItsButtonsOnTheBaseClass(
        Discord.WebSocket.SocketGuild _guild, ulong _channelId,
        string _customIdForButton, ulong _channelCategoryId)
    {
        var component = new ComponentBuilder();

        Log.WriteLine("Creating the channel message with id: "
            + _channelId, LogLevel.VERBOSE);

        var textChannel = _guild.GetChannel(_channelId) as ITextChannel;

        if (textChannel == null)
        {
            Log.WriteLine(nameof(textChannel) + " was null!", LogLevel.CRITICAL);
            return 0;
        }

        Log.WriteLine("Found text channel: " + textChannel.Name, LogLevel.VERBOSE);

        Log.WriteLine("messageButtonNames.Count: " + messageButtonNamesWithAmount.Count, LogLevel.VERBOSE);

        foreach (var buttonNameWithAmount in messageButtonNamesWithAmount)
        {
            Log.WriteLine("Looping through button name: " + buttonNameWithAmount.Key + 
                " with amount: " + buttonNameWithAmount.Value, LogLevel.DEBUG);

            for (int b = 0; b < buttonNameWithAmount.Value; ++b)
            {
                string finalCustomId = "";

                Log.WriteLine("Button number: " + b, LogLevel.VERBOSE);
                InterfaceButton interfaceButton =
                     (InterfaceButton)EnumExtensions.GetInstance(buttonNameWithAmount.Key.ToString());
                Log.WriteLine("button: " + interfaceButton.ButtonLabel + " name: " +
                    interfaceButton.ButtonName, LogLevel.DEBUG);

                finalCustomId = _customIdForButton + "_" + b;

                Log.WriteLine(nameof(finalCustomId) + ": " + finalCustomId, LogLevel.DEBUG);

                component.WithButton(interfaceButton.CreateTheButton(finalCustomId, b));
            }
        }

        var newMessage = await textChannel.SendMessageAsync(
            GenerateMessage(_channelId, _channelCategoryId), components: component.Build());
        ulong newMessageId = newMessage.Id;

        Log.WriteLine("Created a new message with id: " + newMessageId,LogLevel.VERBOSE);

        return newMessageId;
    }

    public abstract string GenerateMessage(ulong _channelId, ulong _channelCategoryId);
}