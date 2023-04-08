using Discord.WebSocket;

public static class FileManager
{
    public static Task AppendText(string _pathToFileAndFileExtension, string _content)
    {
        using (Stream stream = File.Open(_pathToFileAndFileExtension,
            FileMode.Append, FileAccess.Write, FileShare.Write))
        {
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.WriteLine(_content);
                sw.Close();
            }
            stream.Close();
        }
        return Task.CompletedTask;
    }

    public static Task SaveTacviewFromUserUpload(
        CategoryType _leagueName, int _matchId, SocketMessage _message)
    {
        Log.WriteLine("Saving tacview from user upload on league: " + _leagueName + ", on matchId:" +
            _matchId + ", from user: " + _message.Author.Id, LogLevel.VERBOSE);
        string pathToCreate = @"C:\AirCombatMatchmakerBot\Data\Tacviews\" +
            _leagueName.ToString() + @"\" + _matchId.ToString() + @"\";

        CheckIfPathExistsAndCreateItIfNecessary(pathToCreate);

        string finalPath = pathToCreate + "Match-" + _matchId + "_" + _message.Author.Id.ToString() + ".acmi";
        SaveFileAttachment(_message, finalPath);

        Log.WriteLine("Done saving tacview from user upload on league: " + _leagueName + ", on matchId:" +
            _matchId + ", from user: " + _message.Author.Id, LogLevel.VERBOSE);

        return Task.CompletedTask;
    }

    public async static Task<AttachmentData[]> FindTacviewAttachmentsForACertainMatch(
        int _matchId, InterfaceLeague _interfaceLeague)
    {
        Log.WriteLine("Getting tacview from user upload on league: " + _interfaceLeague.LeagueCategoryName + ", on matchId:" +
            _matchId, LogLevel.VERBOSE);
        string pathToLookFor = @"C:\AirCombatMatchmakerBot\Data\Tacviews\" +
            _interfaceLeague.LeagueCategoryName.ToString() + @"\" + _matchId.ToString() + @"\";

        if (!Directory.Exists(pathToLookFor))
        {
            Log.WriteLine("Path doesn't exists: " + pathToLookFor, LogLevel.CRITICAL);
            return new AttachmentData[0];
        }

        List<string> files = Directory.GetFiles(pathToLookFor).ToList();
        AttachmentData[] tacviewResults = new AttachmentData[files.Count];
        if (files.Count > 2 || files.Count == 0)
        {
            Log.WriteLine("Warning! files count was: " + files.Count, LogLevel.WARNING);
        }

        int index = 0;

        InterfaceMessage? interfaceMessageWithAttachments = 
            await Database.Instance.Categories.FindCreatedCategoryWithChannelKvpByCategoryName(
                CategoryType.BOTSTUFF).Value.FindInterfaceChannelWithNameInTheCategory(
                    ChannelType.TACVIEWSTORAGE).CreateARawMessageForTheChannelFromMessageName(
                        "", "Match " + _matchId + "'s tacviews", true, null, false, files.ToArray());
        if (interfaceMessageWithAttachments == null)
        {
            Log.WriteLine(nameof(interfaceMessageWithAttachments) + " was null!", LogLevel.CRITICAL);
            return new AttachmentData[] { };
        }

        foreach (var file in interfaceMessageWithAttachments.CachedUserMessage.Attachments)
        {
            Log.WriteLine("Found file: " + file.Url + " on match: " + _matchId, LogLevel.VERBOSE);

            string playerUlongString = file.Url.Split('/').Last().Split('_').Last().Split('.').First();
            ulong playerId = ulong.Parse(playerUlongString);

            var teams = _interfaceLeague.LeagueData.Teams;

            var foundTeam = teams.CheckIfPlayersTeamIsActiveByIdAndReturnThatTeam(
                _interfaceLeague.LeaguePlayerCountPerTeam, playerId).TeamName;

            tacviewResults[index] = new AttachmentData(foundTeam + "'s Tacview", file.Url);
            index++;
        }

        Log.WriteLine("Done getting tacview from user upload on league: " + _interfaceLeague.LeagueCategoryName + 
            ", on matchId:" + _matchId + ", with count: " + files.Count, LogLevel.VERBOSE);

        return tacviewResults;
    }

    public class AttachmentData
    {
        public string attachmentName = "";
        public string attachmentLink = "";
        public AttachmentData() { }
        public AttachmentData(string _name, string _link) 
        {
            attachmentName = _name;
            attachmentLink = _link;
        }
    }

    public async static void SaveFileAttachment(SocketMessage _message, string _finalPath)
    {
        foreach (var attachment in _message.Attachments)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(attachment.Url))
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        if (!File.Exists(_finalPath))
                        {
                            using (var fileStream = new FileStream(_finalPath, FileMode.CreateNew))
                            {
                                await stream.CopyToAsync(fileStream);
                                fileStream.Dispose();
                            }
                        } 
                        else
                        {
                            using (var fileStream = new FileStream(_finalPath, FileMode.Truncate))
                            {
                                await stream.CopyToAsync(fileStream);
                                fileStream.Dispose();
                            }
                        }
                    }
                }
            }
        }
    }

    public static void CheckIfPathExistsAndCreateItIfNecessary(string _path)
    {
        Log.WriteLine("Starting to create: " + _path, LogLevel.VERBOSE);
        if (!Directory.Exists(_path))
        {
            Directory.CreateDirectory(_path);
            Log.WriteLine("Done creating: " + _path, LogLevel.VERBOSE);
        }
        else
        {
            Log.WriteLine("Already exists, did not create : " + _path, LogLevel.VERBOSE);
        }
    }
}