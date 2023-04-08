using Discord;
using Discord.WebSocket;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Channels;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;

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

    ConcurrentDictionary<ButtonName, int> InterfaceMessage.MessageButtonNamesWithAmount
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

    string InterfaceMessage.MessageEmbedTitle
    {
        get
        {
            Log.WriteLine("Getting " + nameof(messageEmbedTitle)
                + ": " + messageEmbedTitle, LogLevel.VERBOSE);
            return messageEmbedTitle;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(messageEmbedTitle) +
                messageEmbedTitle + " to: " + value, LogLevel.VERBOSE);
            messageEmbedTitle = value;
        }
    }

    string InterfaceMessage.MessageDescription
    {
        get
        {
            Log.WriteLine("Getting " + nameof(messageDescription)
                + ": " + messageDescription, LogLevel.VERBOSE);
            return messageDescription;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(messageDescription) +
                messageDescription + " to: " + value, LogLevel.VERBOSE);
            messageDescription = value;
        }
    }

    Discord.Color InterfaceMessage.MessageEmbedColor
    {
        get
        {
            Log.WriteLine("Getting " + nameof(messageEmbedColor)
                + ": " + messageEmbedColor, LogLevel.VERBOSE);
            return messageEmbedColor;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(messageEmbedColor) +
                messageEmbedColor + " to: " + value, LogLevel.VERBOSE);
            messageEmbedColor = value;
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

    ConcurrentBag<InterfaceButton> InterfaceMessage.ButtonsInTheMessage
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

    Discord.IUserMessage InterfaceMessage.CachedUserMessage
    {
        get
        {
            Log.WriteLine("Getting " + nameof(cachedUserMessage)
                + ": " + cachedUserMessage, LogLevel.VERBOSE);
            return cachedUserMessage;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(cachedUserMessage) +
                cachedUserMessage + " to: " + value, LogLevel.VERBOSE);
            cachedUserMessage = value;
        }
    }

    [DataMember] protected MessageName messageName;
    [DataMember] protected ConcurrentDictionary<ButtonName, int> messageButtonNamesWithAmount;

    // Embed properties
    [DataMember] protected string messageEmbedTitle = "";
    [DataMember] protected string messageDescription = ""; // Not necessary for embed
    protected Discord.Color messageEmbedColor = Discord.Color.DarkGrey;

    [DataMember] protected ulong messageId;
    [DataMember] protected ulong messageChannelId;
    [DataMember] protected ulong messageCategoryId;
    [DataMember] protected ConcurrentBag<InterfaceButton> buttonsInTheMessage;

    protected Discord.IUserMessage cachedUserMessage;
    

    public BaseMessage()
    {
        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>();
        buttonsInTheMessage = new ConcurrentBag<InterfaceButton>();
    }

    // If the component is not null, this is a reply
    public async Task<InterfaceMessage?> CreateTheMessageAndItsButtonsOnTheBaseClass(
        DiscordSocketClient _client, InterfaceChannel _interfaceChannel, 
        bool _displayMessage = true, ulong _leagueCategoryId = 0, 
        SocketMessageComponent? _component = null, bool _ephemeral = true,
        params string[] _files)
    {
        messageChannelId = _interfaceChannel.ChannelId;
        messageCategoryId = _interfaceChannel.ChannelsCategoryId;

        string messageForGenerating = string.Empty;
        var component = new ComponentBuilder();

        Log.WriteLine("Creating the channel message with id: "
            + messageChannelId + " with categoryID: " + messageCategoryId, LogLevel.VERBOSE);

        var textChannel = await _client.GetChannelAsync(messageChannelId) as ITextChannel;
        if (textChannel == null)
        {
            Log.WriteLine(nameof(textChannel) + " was null!", LogLevel.CRITICAL);
            InterfaceMessage? interfaceMessage = null;
            return interfaceMessage;
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
                    finalCustomId, b, messageCategoryId, _leagueCategoryId));

                buttonsInTheMessage.Add(interfaceButton);
            }
        }

        if (messageName == MessageName.LEAGUEREGISTRATIONMESSAGE)
        {
            LEAGUEREGISTRATIONMESSAGE? leagueRegistrationMessage = this as LEAGUEREGISTRATIONMESSAGE;
            if (leagueRegistrationMessage == null)
            {
                Log.WriteLine(nameof(leagueRegistrationMessage) + " was null!", LogLevel.CRITICAL);
                return leagueRegistrationMessage;
            }

            // Pass league id as parameter here
            leagueRegistrationMessage.belongsToLeagueCategoryId = _leagueCategoryId;
            
            messageForGenerating = leagueRegistrationMessage.GenerateMessageForSpecificCategoryLeague();
        }
        else
        {
            messageForGenerating = "\n" + GenerateMessage();
        }

        if (_displayMessage)
        {
            var componentsBuilt = component.Build();

            // Send a regular messageDescription
            if (_component == null)
            {
                var embed = new EmbedBuilder();

                // set the title, description, and color of the embedded messageDescription
                embed.WithTitle(messageEmbedTitle)
                     .WithDescription(messageForGenerating)
                     .WithColor(messageEmbedColor);

                // add a field to the embedded messageDescription
                //embed.AddField("Field Name", "Field Value");

                // add a thumbnail image to the embedded messageDescription
                //embed.WithThumbnailUrl("https://example.com/thumbnail.png");

                if (_files.Length == 0)
                {
                    var userMessage = await textChannel.SendMessageAsync(
                        "", false, embed.Build(), components: componentsBuilt);

                    messageId = userMessage.Id;
                    
                }
                else
                {
                    var iMessageChannel = await _interfaceChannel.GetMessageChannelById(_client);
                    if (iMessageChannel == null)
                    {
                        Log.WriteLine(nameof(iMessageChannel) + " was null!", LogLevel.CRITICAL);
                        return this;
                    }
                    List<FileAttachment> attachments = new List<FileAttachment>();
                    for (int i = 0; i < _files.Length; i++)
                    {

                        FileStream fileStream = new FileStream(_files[i], FileMode.Open, FileAccess.Read);

                        string newName = "Match-" + _files[i].Split('-').Last();
                        attachments.Add(new FileAttachment(fileStream, newName));
                    }
                    cachedUserMessage = await iMessageChannel.SendFilesAsync(attachments);
                }

                messageDescription = messageForGenerating;

                _interfaceChannel.InterfaceMessagesWithIds.TryAdd(messageId, this);
            }
            // Reply to a messageDescription
            else
            {
                await _component.RespondAsync(
                    messageForGenerating, ephemeral: _ephemeral, components: componentsBuilt);
            }
        }

        Log.WriteLine("Created a new message with id: " + messageId, LogLevel.VERBOSE);

        return this;
    }

    public async Task<InterfaceMessage?> CreateTheMessageAndItsButtonsOnTheBaseClassWithAttachmentData(
        DiscordSocketClient _client, InterfaceChannel _interfaceChannel, FileManager.AttachmentData[] _attachmentDatas,
        bool _displayMessage = true, ulong _leagueCategoryId = 0,
        SocketMessageComponent? _component = null, bool _ephemeral = true)
    {
        messageChannelId = _interfaceChannel.ChannelId;
        messageCategoryId = _interfaceChannel.ChannelsCategoryId;

        string messageForGenerating = string.Empty;
        var component = new ComponentBuilder();

        Log.WriteLine("Creating the channel message with id: "
            + messageChannelId + " with categoryID: " + messageCategoryId, LogLevel.VERBOSE);

        var textChannel = await _client.GetChannelAsync(messageChannelId) as ITextChannel;
        if (textChannel == null)
        {
            Log.WriteLine(nameof(textChannel) + " was null!", LogLevel.CRITICAL);
            InterfaceMessage? interfaceMessage = null;
            return interfaceMessage;
        }

        Log.WriteLine("Found text channel: " + textChannel.Name, LogLevel.VERBOSE);


        for (int a = 0; a < _attachmentDatas.Length; a++)
        {
            Log.WriteLine("Looping through link button for " + _attachmentDatas[a], LogLevel.VERBOSE);

            //string finalCustomId = "";

            InterfaceButton interfaceButton =
                 (InterfaceButton)EnumExtensions.GetInstance(
                     ButtonName.LINKBUTTON.ToString());

            Log.WriteLine("button: " + interfaceButton.ButtonLabel + " name: " +
                interfaceButton.ButtonName, LogLevel.DEBUG);

            //finalCustomId = interfaceButton.ButtonName + "_" + a;

            //Log.WriteLine(nameof(finalCustomId) + ": " + finalCustomId, LogLevel.DEBUG);

            LINKBUTTON linkButton = interfaceButton as LINKBUTTON;

            component.WithButton(linkButton.CreateALinkButton(_attachmentDatas[a]));

            buttonsInTheMessage.Add(interfaceButton);
        }

        messageForGenerating = "\n" + GenerateMessage();

        if (_displayMessage)
        {
            var componentsBuilt = component.Build();

            // Send a regular messageDescription
            if (_component == null)
            {
                var embed = new EmbedBuilder();

                // set the title, description, and color of the embedded messageDescription
                embed.WithTitle(messageEmbedTitle)
                     .WithDescription(messageForGenerating)
                     .WithColor(messageEmbedColor);

                // add a field to the embedded messageDescription
                //embed.AddField("Field Name", "Field Value");

                // add a thumbnail image to the embedded messageDescription
                //embed.WithThumbnailUrl("https://example.com/thumbnail.png");

                var userMessage = await textChannel.SendMessageAsync(
                    "", false, embed.Build(), components: componentsBuilt);

                messageId = userMessage.Id;

                messageDescription = messageForGenerating;

                _interfaceChannel.InterfaceMessagesWithIds.TryAdd(messageId, this);
            }
            // Reply to a messageDescription
            else
            {
                await _component.RespondAsync(
                    messageForGenerating, ephemeral: _ephemeral, components: componentsBuilt);
            }
        }

        Log.WriteLine("Created a new message with id: " + messageId, LogLevel.VERBOSE);

        return this;
    }

    public async Task ModifyMessage(string _newContent)
    {
        messageDescription = _newContent;

        Log.WriteLine("Modifying a message on channel id: " + messageChannelId +
            " that has msg id: " + messageId + " with content: " + messageDescription +
            " with new content:" + _newContent, LogLevel.DEBUG);

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return;
        }

        var channel = await client.GetChannelAsync(messageChannelId) as ITextChannel;
        if (channel == null)
        {
            Log.WriteLine(nameof(channel) + " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Found channel: " + channel.Id, LogLevel.VERBOSE);

        var embed = new EmbedBuilder();

        // set the title, description, and color of the embedded messageDescription
        embed.WithTitle(messageEmbedTitle)
             .WithDescription(messageDescription)
             .WithColor(messageEmbedColor);

        await channel.ModifyMessageAsync(messageId, m => m.Embed = embed.Build());

        Log.WriteLine("Modifying the message: " + messageId + " done.", LogLevel.VERBOSE);
    }

    public async Task GenerateAndModifyTheMessage()
    {
        await ModifyMessage(GenerateMessage());
    }

    public abstract string GenerateMessage();

    public async Task<Discord.IMessage?> GetMessageById(IMessageChannel _channel)
    {
        Log.WriteLine("Getting IMessageChannel with id: " + messageId, LogLevel.VERBOSE);

        var message = await _channel.GetMessageAsync(messageId);
        if (message == null)
        {
            Log.WriteLine(nameof(message) + " was null!", LogLevel.ERROR);
            return null;
        }

        Log.WriteLine("Found: " + message.Id, LogLevel.VERBOSE);
        return message;
    }
}