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

    string? InterfaceMessage.MessageEmbedTitle
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

    string? InterfaceMessage.MessageDescription
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
    [DataMember] protected string? messageEmbedTitle { get; set; }
    [DataMember] protected string? messageDescription { get; set; } // Not necessary for embed
    protected Discord.Color messageEmbedColor { get; set; } //= Discord.Color.DarkGrey;

    [DataMember] protected ulong messageId { get; set; }
    [DataMember] protected ulong messageChannelId { get; set; }
    [DataMember] protected ulong messageCategoryId { get; set; }
    [DataMember] protected ConcurrentBag<InterfaceButton> buttonsInTheMessage { get; set; }

    protected bool mentionMatchPlayers { get; set; }
    protected Discord.IUserMessage? cachedUserMessage { get; set; }
    

    public BaseMessage()
    {
        messageEmbedColor = Discord.Color.Default;
        messageButtonNamesWithAmount = new ConcurrentDictionary<ButtonName, int>();
        buttonsInTheMessage = new ConcurrentBag<InterfaceButton>();
    }

    // If the component is not null, this is a reply
    public async Task<InterfaceMessage?> CreateTheMessageAndItsButtonsOnTheBaseClass(
        DiscordSocketClient _client, InterfaceChannel _interfaceChannel, bool _embed, 
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

            // Send a regular messageDescription
            if (_component == null)
            {
                string finalMentionMessage = "";
                if (mentionMatchPlayers)
                {
                    MatchChannelComponents mmc = new MatchChannelComponents();
                    mmc.FindMatchAndItsLeagueAndInsertItToTheCache(this);
                    if (mmc.interfaceLeagueCached == null || mmc.leagueMatchCached == null)
                    {
                        Log.WriteLine(nameof(mmc) + " was null!", LogLevel.CRITICAL);
                        return null;
                    }

                    if (mmc.interfaceLeagueCached == null || mmc.leagueMatchCached == null)
                    {
                        string errorMsg = nameof(mmc) + " was null!";
                        Log.WriteLine(errorMsg, LogLevel.ERROR);
                        //return null;
                    }
                    else
                    {
                        ulong[] playerIdsInTheMatch =
                            mmc.leagueMatchCached.GetIdsOfThePlayersInTheMatchAsArray(mmc.interfaceLeagueCached);
                        foreach (ulong id in playerIdsInTheMatch)
                        {
                            finalMentionMessage += "<@" + id.ToString() + "> ";
                        }
                    }
                }

                if (_embed)
                {
                    var embed = new EmbedBuilder();

                    // set the title, description, and color of the embedded messageDescription
                    embed.WithTitle(messageEmbedTitle)
                         .WithDescription(messageForGenerating)
                         .WithColor(messageEmbedColor);

                    // add a field to the embedded messageDescription
                    //embed.AddField("Field Name", "Field Value");

                    // add a thumbnail image to the embedded messageDescription
                    //embed.WithThumbnailUrl("");

                    if (_files.Length == 0)
                    {
                        cachedUserMessage = await textChannel.SendMessageAsync(
                            finalMentionMessage, false, embed.Build(), components: componentsBuilt);

                        messageId = cachedUserMessage.Id;
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
                // NON EMBED MESSAGES ARE NOT ADDED TO THE InterfaceMessagesWithIds list!!!
                else
                {
                    cachedUserMessage = await textChannel.SendMessageAsync(
                        messageDescription, false, components: componentsBuilt);
                }
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
        DiscordSocketClient _client, InterfaceChannel _interfaceChannel, AttachmentData[] _attachmentDatas,
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
                    finalCustomId, b, messageCategoryId, _leagueCategoryId));

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
                buttonToGenerateKvp.Key, ++buttonId, messageCategoryId, _leagueCategoryId));

            buttonsInTheMessage.Add(interfaceButton);
        }

        Log.WriteLine("Done generating buttons", LogLevel.VERBOSE);
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