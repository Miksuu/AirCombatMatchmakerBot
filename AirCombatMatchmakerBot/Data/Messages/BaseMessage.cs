using Discord;
using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Collections.Generic;

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
        get => messageEmbedTitle.GetValue();
        set => messageEmbedTitle.SetValue(value);
    }

    string InterfaceMessage.MessageDescription
    {
        get => messageDescription.GetValue();
        set => messageDescription.SetValue(value);
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
        get => messageId.GetValue();
        set => messageId.SetValue(value);
    }

    ulong InterfaceMessage.MessageChannelId
    {
        get => messageChannelId.GetValue();
        set => messageChannelId.SetValue(value);
    }

    ulong InterfaceMessage.MessageCategoryId
    {
        get => messageCategoryId.GetValue();
        set => messageCategoryId.SetValue(value);
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

    Discord.IUserMessage? InterfaceMessage.CachedUserMessage
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
    [DataMember] protected logString messageEmbedTitle = new logString();
    [DataMember] protected logString messageDescription = new logString(); // Not necessary for embed
    protected Discord.Color messageEmbedColor { get; set; } //= Discord.Color.DarkGrey;

    [DataMember] protected logUlong messageId = new logUlong();
    [DataMember] protected logUlong messageChannelId = new logUlong();
    [DataMember] protected logUlong messageCategoryId = new logUlong();
    [DataMember] protected ConcurrentBag<InterfaceButton> buttonsInTheMessage { get; set; }

    protected bool mentionMatchPlayers { get; set; }
    protected Discord.IUserMessage? cachedUserMessage { get; set; }

    protected InterfaceMessage thisInterfaceMessage;

    public BaseMessage()
    {
        messageEmbedColor = Discord.Color.Default;
        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>();
        buttonsInTheMessage = new ConcurrentBag<InterfaceButton>();
        thisInterfaceMessage = this;
    }

    // If the component is not null, this is a reply
    public async Task<InterfaceMessage?> CreateTheMessageAndItsButtonsOnTheBaseClass(
        DiscordSocketClient _client, InterfaceChannel _interfaceChannel, bool _embed, 
        bool _displayMessage = true, ulong _leagueCategoryId = 0, 
        SocketMessageComponent? _component = null, bool _ephemeral = true,
        params string[] _files)
    {
        thisInterfaceMessage.MessageChannelId = _interfaceChannel.ChannelId;
        thisInterfaceMessage.MessageCategoryId = _interfaceChannel.ChannelsCategoryId;

        string messageForGenerating = string.Empty;
        var component = new ComponentBuilder();

        Log.WriteLine("Creating the channel message with id: "
            + thisInterfaceMessage.MessageChannelId + " with categoryID: " + thisInterfaceMessage.MessageCategoryId, LogLevel.VERBOSE);

        var textChannel = await _client.GetChannelAsync(thisInterfaceMessage.MessageChannelId) as ITextChannel;
        if (textChannel == null)
        {
            Log.WriteLine(nameof(textChannel) + " was null!", LogLevel.CRITICAL);
            InterfaceMessage? interfaceMessage = null;
            return interfaceMessage;
        }

        Log.WriteLine("Found text channel: " + textChannel.Name, LogLevel.VERBOSE);

        Log.WriteLine("messageButtonNames.Count: " +
            messageButtonNamesWithAmount.Count, LogLevel.VERBOSE);

        // Generates either normal buttons, or custom amount of buttons with different properties
        GenerateButtons(component, _leagueCategoryId);

        // Add this as inherited button method
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

            // Send a regular MessageDescription
            if (_component == null)
            {
                string finalMentionMessage = "";
                if (mentionMatchPlayers)
                {
                    MatchChannelComponents mcc = new MatchChannelComponents();
                    mcc.FindMatchAndItsLeagueAndInsertItToTheCache(this);
                    if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
                    {
                        Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
                        return null;
                    }

                    if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
                    {
                        string errorMsg = nameof(mcc) + " was null!";
                        Log.WriteLine(errorMsg, LogLevel.ERROR);
                        //return null;
                    }
                    else
                    {
                        ulong[] playerIdsInTheMatch =
                            mcc.leagueMatchCached.GetIdsOfThePlayersInTheMatchAsArray(mcc.interfaceLeagueCached);
                        foreach (ulong id in playerIdsInTheMatch)
                        {
                            finalMentionMessage += "<@" + id.ToString() + "> ";
                        }
                    }
                }

                if (_embed)
                {
                    var embed = new EmbedBuilder();

                    // set the title, description, and color of the embedded MessageDescription
                    embed.WithTitle(thisInterfaceMessage.MessageEmbedTitle)
                         .WithDescription(messageForGenerating)
                         .WithColor(messageEmbedColor);

                    // add a field to the embedded MessageDescription
                    //embed.AddField("Field Name", "Field Value");

                    // add a thumbnail image to the embedded MessageDescription
                    //embed.WithThumbnailUrl("");

                    if (_files.Length == 0)
                    {
                        cachedUserMessage = await textChannel.SendMessageAsync(
                            finalMentionMessage, false, embed.Build(), components: componentsBuilt);

                        thisInterfaceMessage.MessageId = cachedUserMessage.Id;
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

                    thisInterfaceMessage.MessageDescription = messageForGenerating;

                    _interfaceChannel.InterfaceMessagesWithIds.TryAdd(thisInterfaceMessage.MessageId, this);
                }
                // NON EMBED MESSAGES ARE NOT ADDED TO THE InterfaceMessagesWithIds list!!!
                else
                {
                    cachedUserMessage = await textChannel.SendMessageAsync(
                        thisInterfaceMessage.MessageDescription, false, components: componentsBuilt);
                }
            }
            // Reply to a MessageDescription
            else
            {
                await _component.RespondAsync(
                    messageForGenerating, ephemeral: _ephemeral, components: componentsBuilt);
            }
        }

        Log.WriteLine("Created a new message with id: " + thisInterfaceMessage.MessageId, LogLevel.VERBOSE);

        return this;
    }

    public async Task<InterfaceMessage?> CreateTheMessageAndItsButtonsOnTheBaseClassWithAttachmentData(
        DiscordSocketClient _client, InterfaceChannel _interfaceChannel, AttachmentData[] _attachmentDatas,
        bool _displayMessage = true, ulong _leagueCategoryId = 0,
        SocketMessageComponent? _component = null, bool _ephemeral = true)
    {
        thisInterfaceMessage.MessageChannelId = _interfaceChannel.ChannelId;
        thisInterfaceMessage.MessageCategoryId = _interfaceChannel.ChannelsCategoryId;

        string messageForGenerating = string.Empty;
        var component = new ComponentBuilder();

        Log.WriteLine("Creating the channel message with id: "
            + thisInterfaceMessage.MessageChannelId + " with categoryID: " + thisInterfaceMessage.MessageCategoryId, LogLevel.VERBOSE);

        var textChannel = await _client.GetChannelAsync(thisInterfaceMessage.MessageChannelId) as ITextChannel;
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

            LINKBUTTON? linkButton = interfaceButton as LINKBUTTON;
            if (linkButton == null)
            {
                Log.WriteLine(nameof(linkButton) + " was null!", LogLevel.CRITICAL);
                return null;
            }

            component.WithButton(linkButton.CreateALinkButton(_attachmentDatas[a]));

            buttonsInTheMessage.Add(interfaceButton);
        }

        messageForGenerating = "\n" + GenerateMessage();

        if (_displayMessage)
        {
            var componentsBuilt = component.Build();

            // Send a regular MessageDescription
            if (_component == null)
            {
                var embed = new EmbedBuilder();

                // set the title, description, and color of the embedded MessageDescription
                embed.WithTitle(thisInterfaceMessage.MessageEmbedTitle)
                     .WithDescription(messageForGenerating)
                     .WithColor(messageEmbedColor);

                // add a field to the embedded MessageDescription
                //embed.AddField("Field Name", "Field Value");

                // add a thumbnail image to the embedded MessageDescription
                //embed.WithThumbnailUrl("https://example.com/thumbnail.png");

                var userMessage = await textChannel.SendMessageAsync(
                    "", false, embed.Build(), components: componentsBuilt);

                thisInterfaceMessage.MessageId = userMessage.Id;

                thisInterfaceMessage.MessageDescription = messageForGenerating;

                _interfaceChannel.InterfaceMessagesWithIds.TryAdd(thisInterfaceMessage.MessageId, this);
            }
            // Reply to a MessageDescription
            else
            {
                await _component.RespondAsync(
                    messageForGenerating, ephemeral: _ephemeral, components: componentsBuilt);
            }
        }

        Log.WriteLine("Created a new message with id: " + thisInterfaceMessage.MessageId, LogLevel.VERBOSE);

        return this;
    }

    public async Task ModifyMessage(string _newContent)
    {
        thisInterfaceMessage.MessageDescription = _newContent;

        Log.WriteLine("Modifying a message on channel id: " + thisInterfaceMessage.MessageChannelId +
            " that has msg id: " + thisInterfaceMessage.MessageId + " with content: " + thisInterfaceMessage.MessageDescription +
            " with new content:" + _newContent, LogLevel.DEBUG);

        var client = BotReference.GetClientRef();
        if (client == null)
        {
            Exceptions.BotClientRefNull();
            return;
        }

        var channel = await client.GetChannelAsync(thisInterfaceMessage.MessageChannelId) as ITextChannel;
        if (channel == null)
        {
            Log.WriteLine(nameof(channel) + " was null!", LogLevel.CRITICAL);
            return;
        }

        Log.WriteLine("Found channel: " + channel.Id, LogLevel.VERBOSE);

        var embed = new EmbedBuilder();

        // set the title, description, and color of the embedded MessageDescription
        embed.WithTitle(thisInterfaceMessage.MessageEmbedTitle)
             .WithDescription(thisInterfaceMessage.MessageDescription)
             .WithColor(messageEmbedColor);

        await channel.ModifyMessageAsync(thisInterfaceMessage.MessageId, m => m.Embed = embed.Build());

        Log.WriteLine("Modifying the message: " + thisInterfaceMessage.MessageId + " done.", LogLevel.VERBOSE);
    }

    public async Task GenerateAndModifyTheMessage()
    {
        await ModifyMessage(GenerateMessage());
    }

    protected abstract void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId);
    protected void GenerateRegularButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
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

                _component.WithButton(interfaceButton.CreateTheButton(
                    finalCustomId, b, thisInterfaceMessage.MessageCategoryId, _leagueCategoryId));

                buttonsInTheMessage.Add(interfaceButton);
            }
        }
    }

    // CustomID, Label and the type to generate from the inherited class
    protected void GenerateButtonsWithCustomPropertiesAndIds(
        Dictionary<string, string> _buttonsToGenerate, ButtonName _buttonTypeToGenerate,
        ComponentBuilder _component, ulong _leagueCategoryId)
    {
        Log.WriteLine("buttons to generate count:" + _buttonsToGenerate, LogLevel.VERBOSE);

        int buttonId = 0;
        foreach (var buttonToGenerateKvp in _buttonsToGenerate)
        {
            InterfaceButton interfaceButton =
                 (InterfaceButton)EnumExtensions.GetInstance(
                     _buttonTypeToGenerate.ToString());

            interfaceButton.ButtonLabel = buttonToGenerateKvp.Value;

            Log.WriteLine("button: " + interfaceButton.ButtonLabel + " name: " +
                interfaceButton.ButtonName + " with customId: " + "customId: " +
                buttonToGenerateKvp.Key, LogLevel.DEBUG);

            _component.WithButton(interfaceButton.CreateTheButton(
                buttonToGenerateKvp.Key, ++buttonId, thisInterfaceMessage.MessageCategoryId, _leagueCategoryId));

            buttonsInTheMessage.Add(interfaceButton);
        }

        Log.WriteLine("Done generating buttons", LogLevel.VERBOSE);
    }

    public abstract string GenerateMessage();

    public async Task<Discord.IMessage?> GetMessageById(IMessageChannel _channel)
    {
        Log.WriteLine("Getting IMessageChannel with id: " + thisInterfaceMessage.MessageId, LogLevel.VERBOSE);

        var message = await _channel.GetMessageAsync(thisInterfaceMessage.MessageId);
        if (message == null)
        {
            Log.WriteLine(nameof(message) + " was null!", LogLevel.ERROR);
            return null;
        }

        Log.WriteLine("Found: " + message.Id, LogLevel.VERBOSE);
        return message;
    }
}