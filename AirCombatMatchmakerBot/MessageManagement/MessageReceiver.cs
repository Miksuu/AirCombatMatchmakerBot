using Discord;
using Discord.WebSocket;

public static class MessageReceiver
{
    public async static Task ReceiveMessage(SocketMessage _socketMessage)
    {
                /*
        Log.WriteLine("Looking for: " + _socketMessage.Channel.Id, LogLevel.DEBUG);
        foreach (var item in CategoryAndChannelManager.matchChannelsIdWithCategoryId)
        {
            Log.WriteLine(item.Key + " | " + item.Value, LogLevel.DEBUG);
        }*/

        // Disregards any MessageDescription that's not inside the bot's match channels
        if (!Database.Instance.Categories.MatchChannelsIdWithCategoryId.ContainsKey(
            _socketMessage.Channel.Id))
        {
            return;
        }

        //Log.WriteLine("Found: " + _socketMessage.Channel.Id, LogLevel.DEBUG);

        // Checks for any attachments
        if (!_socketMessage.Attachments.Any())
        {
            return;
        }

        Log.WriteLine("Message: " + _socketMessage.Id + " + detected in: " +
            _socketMessage.Channel.Id + " by: " + _socketMessage.Author.Id);

        //var socketMessageComponent = _socketMessage.ToComponent();

        // Check if the MessageDescription contains a file and only one file
        if (_socketMessage.Attachments.Count != 1)
        {
            Log.WriteLine("Message: " + _socketMessage.Id +
                " contained more than 1 attachment!");

            await _socketMessage.Channel.SendMessageAsync("\n" +
                _socketMessage.Author.Mention + ", make sure only include one attachment in the message," +
                " with the .acmi file of the match!");

            await _socketMessage.DeleteAsync();

            return;
        }

        //var attachment = _socketMessage.Attachments.FirstOrDefault();

        List<string> allowedFileFormats = new List<string>
            {
                "acmi", // Tacview file format
                "mp4", "webm", "mov", // videos
                "png", "jpg", "jpeg", "gif", // images
            };

        string acmiUrl = "";

        string cachedBadFileName = "";

        foreach (var attachment in _socketMessage.Attachments)
        {
            if (attachment == null)
            {
                Log.WriteLine(nameof(attachment) + " was null!", LogLevel.CRITICAL);
                continue;
            }

            Log.WriteLine("Found attachment: " + attachment.Filename);

            foreach (string fileFormat in allowedFileFormats)
            {
                if (!allowedFileFormats.Contains(fileFormat))
                {
                    cachedBadFileName = attachment.Filename;
                }

                if (fileFormat == "acmi" && acmiUrl == "")
                {
                    acmiUrl = attachment.Url;
                }
            }
        }

        if (cachedBadFileName != "")
        {
            Log.WriteLine(_socketMessage.Author.Id +
            " tried to send a file that is not a .acmi file!" +
            " URL:" + cachedBadFileName, LogLevel.WARNING);

            await _socketMessage.Channel.SendMessageAsync("\n" +
              _socketMessage.Author.Mention +
              ", make sure the attachment you are sending is in a valid format!");

            await _socketMessage.DeleteAsync();
            return;
        }
        else if (acmiUrl != "")
        {
            Log.WriteLine("acmiUrl detected: " + acmiUrl, LogLevel.DEBUG);

            MatchChannelComponents mcc = new MatchChannelComponents( _socketMessage.Channel.Id);
            if (mcc.interfaceLeagueCached == null || mcc.leagueMatchCached == null)
            {
                Log.WriteLine(nameof(mcc.interfaceLeagueCached) + " or " +
                    nameof(mcc.leagueMatchCached) + " was null!", LogLevel.CRITICAL);
                return;
            }

            // Maybe use MatchChannel component here?
            //var interfaceLeagueWithLeagueMatch =
            //    Database.Instance.Leagues.FindLeagueInterfaceAndLeagueMatchWithChannelId(_socketMessage.Channel.Id);

            //if (interfaceLeagueWithLeagueMatch.Item1 == null || interfaceLeagueWithLeagueMatch.Item2 == null)
            //{
            //    Log.WriteLine(nameof(interfaceLeagueWithLeagueMatch) + " was null!", LogLevel.CRITICAL);
            //    return;
            //}

            // Process the tacview file, and delete the original MessageDescription by the user
            var finalResponse = mcc.leagueMatchCached.MatchReporting.ProcessPlayersSentReportObject(
                _socketMessage.Author.Id, acmiUrl,
                    TypeOfTheReportingObject.TACVIEWLINK,
                    mcc.interfaceLeagueCached.LeagueCategoryId,
                    _socketMessage.Channel.Id).Result;

            if (!finalResponse.serialize)
            {
                return;
            }

            if (mcc.leagueMatchCached.MatchState == MatchState.REPORTINGPHASE)
            {
                finalResponse = await mcc.leagueMatchCached.MatchReporting.PrepareFinalMatchResult(
                    _socketMessage.Author.Id, _socketMessage.Channel.Id);

                if (!finalResponse.serialize)
                {
                    return;
                }
            }

            await TacviewManager.SaveTacviewFromUserUpload(
                mcc.interfaceLeagueCached.LeagueCategoryName,
                mcc.leagueMatchCached.MatchId, _socketMessage);

            ulong smId = _socketMessage.Author.Id;

            await _socketMessage.DeleteAsync();

            Log.WriteLine("Done deleting message from: " + smId);

            await SerializationManager.SerializeDB();
        }

        return;
    }
}