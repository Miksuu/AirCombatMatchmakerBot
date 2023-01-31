using Discord;
using System.Reflection;
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

    ulong InterfaceMessage.MessageChannelId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(messageChannelId)
                + ": " + messageChannelId, LogLevel.VERBOSE);
            return messageChannelId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(messageChannelId) +
                messageChannelId + " to: " + value, LogLevel.VERBOSE);
            messageChannelId = value;
        }
    }

    ulong InterfaceMessage.MessageCategoryId
    {
        get
        {
            Log.WriteLine("Getting " + nameof(messageCategoryId)
                + ": " + messageCategoryId, LogLevel.VERBOSE);
            return messageCategoryId;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(messageCategoryId) +
                messageCategoryId + " to: " + value, LogLevel.VERBOSE);
            messageCategoryId = value;
        }
    }

    List<InterfaceButton> InterfaceMessage.ButtonsInTheMessage
    {
        get
        {
            Log.WriteLine("Getting " + nameof(buttonsInTheMessage) + " with count of: " +
                buttonsInTheMessage.Count, LogLevel.VERBOSE);
            return buttonsInTheMessage;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(buttonsInTheMessage)
                + " to: " + value, LogLevel.VERBOSE);
            buttonsInTheMessage = value;
        }
    }

    [DataMember] protected MessageName messageName;
    [DataMember] protected Dictionary<ButtonName, int> messageButtonNamesWithAmount;
    [DataMember] protected string message = "";
    [DataMember] protected ulong messageId;
    [DataMember] protected ulong messageChannelId;
    [DataMember] protected ulong messageCategoryId;
    [DataMember] protected List<InterfaceButton> buttonsInTheMessage;

    public BaseMessage()
    {
        messageButtonNamesWithAmount = new Dictionary<ButtonName, int>();
        buttonsInTheMessage = new List<InterfaceButton>();
    }

    public async Task<ulong> CreateTheMessageAndItsButtonsOnTheBaseClass(
        Discord.WebSocket.SocketGuild _guild, ulong _channelId, ulong _channelCategoryId,
        string _messageKey)
    {
        messageChannelId = _channelId;
        messageCategoryId = _channelCategoryId;

        var component = new ComponentBuilder();

        Log.WriteLine("Creating the channel message with id: "
            + _channelId + " with categoryID: " + _channelCategoryId, LogLevel.VERBOSE);

        var textChannel = _guild.GetChannel(_channelId) as ITextChannel;

        if (textChannel == null)
        {
            Log.WriteLine(nameof(textChannel) + " was null!", LogLevel.CRITICAL);
            return 0;
        }

        Log.WriteLine("Found text channel: " + textChannel.Name, LogLevel.VERBOSE);

        Log.WriteLine("messageButtonNames.Count: " +
            messageButtonNamesWithAmount.Count, LogLevel.VERBOSE);

        foreach (var buttonNameWithAmount in messageButtonNamesWithAmount)
        {
            Log.WriteLine("Looping through button name: " + buttonNameWithAmount.Key + 
                " with amount: " + buttonNameWithAmount.Value, LogLevel.DEBUG);

            for (int b = 0; b < buttonNameWithAmount.Value; ++b)
            {
                string finalCustomId = "";

                InterfaceButton interfaceButton =
                     (InterfaceButton)EnumExtensions.GetInstance(
                         buttonNameWithAmount.Key.ToString());

                Log.WriteLine("button: " + interfaceButton.ButtonLabel + " name: " +
                    interfaceButton.ButtonName, LogLevel.DEBUG);

                finalCustomId = interfaceButton.ButtonName + "_" + b;

                Log.WriteLine(nameof(finalCustomId) + ": " + finalCustomId, LogLevel.DEBUG);

                component.WithButton(interfaceButton.CreateTheButton(
                    finalCustomId, b, _channelCategoryId, _messageKey));

                buttonsInTheMessage.Add(interfaceButton);
            }
        }

        dynamic newMessage = await textChannel.SendMessageAsync(
            "\n" + GenerateMessage(), components: component.Build());
        ulong newMessageId = newMessage.Id;

        Log.WriteLine("Created a new message with id: " + newMessageId,LogLevel.VERBOSE);

        return newMessageId;
    }

    public async Task ModifyMessage(string _newContent)
    {
        message = _newContent;

        Log.WriteLine("Modifying a message on channel id: " + messageChannelId +
            " that has msg id: " + messageId + " with content: " + message, LogLevel.DEBUG);

        var guild = BotReference.GetGuildRef();

        if (guild == null)
        {
            Exceptions.BotGuildRefNull();
            return;
        }

        var channel = guild.GetTextChannel(messageChannelId) as ITextChannel;

        await channel.ModifyMessageAsync(messageId, m => m.Content = message);

        Log.WriteLine("Modifying the message: " + messageId + " done.", LogLevel.VERBOSE);
    }

    public async Task GenerateAndModifyTheMessage()
    {
        await ModifyMessage(GenerateMessage());
    }

    public abstract string GenerateMessage();
}