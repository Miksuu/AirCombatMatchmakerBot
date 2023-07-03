using Discord;
using Discord.WebSocket;
using System.Runtime.Serialization;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

[DataContract]
public abstract class BaseMessage : InterfaceMessage
{
    MessageName InterfaceMessage.MessageName
    {
        get => messageName.GetValue();
        set => messageName.SetValue(value);
    }

    [IgnoreDataMember]
    ConcurrentDictionary<ButtonName, int> InterfaceMessage.MessageButtonNamesWithAmount
    {
        get => messageButtonNamesWithAmount.GetValue();
        set => messageButtonNamesWithAmount.SetValue(value);
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
                + ": " + messageEmbedColor);
            return messageEmbedColor;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(messageEmbedColor) +
                messageEmbedColor + " to: " + value);
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
        get => buttonsInTheMessage.GetValue();
        set => buttonsInTheMessage.SetValue(value);
    }

    Discord.IUserMessage InterfaceMessage.CachedUserMessage
    {
        get
        {
            Log.WriteLine("Getting " + nameof(cachedUserMessage)
                + ": " + cachedUserMessage);
            return cachedUserMessage;
        }
        set
        {
            Log.WriteLine("Setting " + nameof(cachedUserMessage) +
                cachedUserMessage + " to: " + value);
            cachedUserMessage = value;
        }
    }

    [DataMember] protected logEnum<MessageName> messageName = new logEnum<MessageName>();
    [DataMember]
    protected logConcurrentDictionary<ButtonName, int> messageButtonNamesWithAmount =
        new logConcurrentDictionary<ButtonName, int>();

    // Embed properties
    [DataMember] protected logString messageEmbedTitle = new logString();
    [DataMember] protected logString messageDescription = new logString(); // Not necessary for embed
    protected Discord.Color messageEmbedColor = Discord.Color.Default;

    [DataMember] protected logVar<ulong> messageId = new logVar<ulong>();
    [DataMember] protected logVar<ulong> messageChannelId = new logVar<ulong>();
    [DataMember] protected logVar<ulong> messageCategoryId = new logVar<ulong>();
    [DataMember] protected logConcurrentBag<InterfaceButton> buttonsInTheMessage = new logConcurrentBag<InterfaceButton>();

    protected bool mentionMatchPlayers { get; set; }
    protected bool mentionOtherTeamsPlayers { get; set; }
    protected Discord.IUserMessage cachedUserMessage { get; set; }

    protected InterfaceMessage thisInterfaceMessage;

    public BaseMessage()
    {
        thisInterfaceMessage = this;
    }

    // If the component is not null, this is a reply
    public async Task<InterfaceMessage> CreateTheMessageAndItsButtonsOnTheBaseClass(
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
            + thisInterfaceMessage.MessageChannelId + " with categoryID: " + thisInterfaceMessage.MessageCategoryId);

        var textChannel = await _client.GetChannelAsync(thisInterfaceMessage.MessageChannelId) as ITextChannel;
        if (textChannel == null)
        {
            Log.WriteLine(nameof(textChannel) + " was null!", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(textChannel) + " was null!");
        }

        Log.WriteLine("Found text channel: " + textChannel.Name);

        Log.WriteLine("messageButtonNames.Count: " +
            thisInterfaceMessage.MessageButtonNamesWithAmount.Count);

        // Generates either normal buttons, or custom amount of buttons with different properties
        GenerateButtons(component, _leagueCategoryId);


        messageForGenerating = "\n" + GenerateMessage(_leagueCategoryId).Result;

        if (_displayMessage)
        {
            var componentsBuilt = component.Build();

            // Send a regular MessageDescription
            if (_component == null)
            {
                string finalMentionMessage = "";
                if (mentionMatchPlayers || mentionOtherTeamsPlayers)
                {
                    MatchChannelComponents mcc = new MatchChannelComponents(this);
                    if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
                    {
                        Log.WriteLine(nameof(mcc) + " was null!", LogLevel.CRITICAL);
                        throw new InvalidOperationException(nameof(mcc) + " was null!");
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
                            mcc.leagueMatchCached.GetIdsOfThePlayersInTheMatchAsArray();
                        foreach (ulong playerId in playerIdsInTheMatch)
                        {
                            // Skip pinging the team that doesn't need to be pinged (such as when received Schedule request)
                            if (mentionOtherTeamsPlayers &&
                                mcc.interfaceLeagueCached.LeagueData.FindActiveTeamByPlayerIdInAPredefinedLeagueByPlayerId(playerId).TeamId ==
                                    mcc.leagueMatchCached.ScheduleObject.TeamIdThatRequestedScheduling)
                            {
                                continue;
                            }

                            finalMentionMessage += "<@" + playerId.ToString() + "> ";
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

        Log.WriteLine("Created a new message with id: " + thisInterfaceMessage.MessageId);

        return this;
    }

    public async Task<InterfaceMessage> CreateTheMessageAndItsButtonsOnTheBaseClassWithAttachmentData(
        DiscordSocketClient _client, InterfaceChannel _interfaceChannel, AttachmentData[] _attachmentDatas,
        bool _displayMessage = true, ulong _leagueCategoryId = 0,
        SocketMessageComponent? _component = null, bool _ephemeral = true)
    {
        thisInterfaceMessage.MessageChannelId = _interfaceChannel.ChannelId;
        thisInterfaceMessage.MessageCategoryId = _interfaceChannel.ChannelsCategoryId;

        string messageForGenerating = string.Empty;
        var component = new ComponentBuilder();

        Log.WriteLine("Creating the channel message with id: "
            + thisInterfaceMessage.MessageChannelId + " with categoryID: " + thisInterfaceMessage.MessageCategoryId);

        var textChannel = await _client.GetChannelAsync(thisInterfaceMessage.MessageChannelId) as ITextChannel;
        if (textChannel == null)
        {
            Log.WriteLine(nameof(textChannel) + " was null!", LogLevel.CRITICAL);
            return this;
        }

        Log.WriteLine("Found text channel: " + textChannel.Name);

        for (int a = 0; a < _attachmentDatas.Length; a++)
        {
            Log.WriteLine("Looping through link button for " + _attachmentDatas[a]);

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
                throw new InvalidOperationException(nameof(linkButton) + " was null!");
            }

            component.WithButton(linkButton.CreateALinkButton(_attachmentDatas[a]));

            thisInterfaceMessage.ButtonsInTheMessage.Add(interfaceButton);
        }

        messageForGenerating = "\n" + GenerateMessage().Result;

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

        Log.WriteLine("Created a new message with id: " + thisInterfaceMessage.MessageId);

        return this;
    }

    public async void ModifyMessage(string _newContent)
    {
        try
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

            Log.WriteLine("Found channel: " + channel.Id);

            var embed = new EmbedBuilder();

            Log.WriteLine(thisInterfaceMessage.MessageDescription);

            // set the title, description, and color of the embedded MessageDescription
            embed.WithTitle(thisInterfaceMessage.MessageEmbedTitle)
                 .WithDescription(thisInterfaceMessage.MessageDescription)
                 .WithColor(messageEmbedColor);

            await channel.ModifyMessageAsync(thisInterfaceMessage.MessageId, m => m.Embed = embed.Build());

            Log.WriteLine("Modifying the message: " + thisInterfaceMessage.MessageId + " done.");
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.CRITICAL);
            throw;
        }
    }

    public async Task AddContentToTheEndOfTheMessage(string _content)
    {
        Log.WriteLine("Adding content: " + _content + " to the end of the message: " +
            thisInterfaceMessage.MessageId);

        ModifyMessage(thisInterfaceMessage.MessageDescription + "\n\n" + _content);

        Log.WriteLine("Done adding content: " + _content + " to the end of the message: " +
            thisInterfaceMessage.MessageId);
    }

    public void GenerateAndModifyTheMessage(ulong _messageCategoryId = 0)
    {
        Log.WriteLine(_messageCategoryId.ToString());
        ModifyMessage(GenerateMessage(_messageCategoryId).Result);
    }

    protected abstract void GenerateButtons(ComponentBuilder _component, ulong _leagueCategoryId);
    protected void GenerateRegularButtons(ComponentBuilder _component, ulong _leagueCategoryId)
    {
        foreach (var buttonNameWithAmount in thisInterfaceMessage.MessageButtonNamesWithAmount)
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

                thisInterfaceMessage.ButtonsInTheMessage.Add(interfaceButton);
            }
        }
    }

    // CustomID, Label and the type to generate from the inherited class
    protected void GenerateButtonsWithCustomPropertiesAndIds(
        Dictionary<string, string> _buttonsToGenerate, ButtonName _buttonTypeToGenerate,
        ComponentBuilder _component, ulong _leagueCategoryId)
    {
        Log.WriteLine("buttons to generate count:" + _buttonsToGenerate);

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

            thisInterfaceMessage.ButtonsInTheMessage.Add(interfaceButton);
        }

        Log.WriteLine("Done generating buttons");
    }

    public abstract Task<string> GenerateMessage(ulong _messageCategoryId = 0);

    public async Task<Discord.IMessage> GetMessageById(IMessageChannel _channel)
    {
        Log.WriteLine("Getting IMessageChannel with id: " + thisInterfaceMessage.MessageId);

        var message = await _channel.GetMessageAsync(thisInterfaceMessage.MessageId);
        if (message == null)
        {
            Log.WriteLine(nameof(message) + " was null!", LogLevel.ERROR);
            throw new InvalidOperationException(nameof(message) + " was null!");
        }

        Log.WriteLine("Found: " + message.Id);
        return message;
    }

    public InterfaceButton FindButtonWithComponentDataCustomIdInTheMessage(string _componentDataCustomId)
    {
        InterfaceButton? foundButton = thisInterfaceMessage.ButtonsInTheMessage.FirstOrDefault(
            b => b.ButtonCustomId == _componentDataCustomId);
        if (foundButton == null)
        {
            Log.WriteLine(nameof(foundButton) + " was null", LogLevel.CRITICAL);
            throw new InvalidOperationException(nameof(foundButton) + " was null!");
        }

        Log.WriteLine("Found the specific button: " + foundButton.ButtonName +
            " with label: " + foundButton.ButtonLabel, LogLevel.DEBUG);

        return foundButton;
    }
}