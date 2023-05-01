using Discord.WebSocket;

public class TacviewManager
{
    public static Task SaveTacviewFromUserUpload(
    CategoryType _leagueName, int _matchId, SocketMessage _message)
    {
        Log.WriteLine("Saving tacview from user upload on league: " + _leagueName + ", on matchId:" +
            _matchId + ", from user: " + _message.Author.Id, LogLevel.VERBOSE);
        string pathToCreate = @"C:\AirCombatMatchmakerBot\Data\Tacviews\" +
            _leagueName.ToString() + @"\" + _matchId.ToString();

        string fileName = "Match-" + _matchId + "_" + _message.Author.Id.ToString() + ".acmi";
        FileManager.SaveFileAttachment(_message, pathToCreate, fileName);

        Log.WriteLine("Done saving tacview from user upload on league: " + _leagueName + ", on matchId:" +
            _matchId + ", from user: " + _message.Author.Id, LogLevel.VERBOSE);

        return Task.CompletedTask;
    }

    public async static Task<AttachmentData[]> FindTacviewAttachmentsForACertainMatch(
        int _matchId, InterfaceLeague _interfaceLeague)
    {
        Log.WriteLine("Getting tacview from user upload on league: " +
            _interfaceLeague.LeagueCategoryName + ", on matchId:" + _matchId, LogLevel.VERBOSE);

        string pathToLookFor = @"C:\AirCombatMatchmakerBot\Data\Tacviews\" +
            _interfaceLeague.LeagueCategoryName.ToString() + @"\" + _matchId.ToString() + @"\";

        if (!Directory.Exists(pathToLookFor))
        {
            Log.WriteLine("Path doesn't exists: " + pathToLookFor, LogLevel.CRITICAL);
            return Array.Empty<AttachmentData>();
        }

        List<string> files = Directory.GetFiles(pathToLookFor).ToList();
        AttachmentData[] tacviewResults = new AttachmentData[files.Count];
        if (files.Count > 2 || files.Count == 0)
        {
            Log.WriteLine("Warning! files count was: " + files.Count, LogLevel.WARNING);
        }

        int index = 0;

        var cachedUserMessage =
            await Database.Instance.Categories.FindCreatedCategoryWithChannelKvpByCategoryName(
                CategoryType.BOTSTUFF).Value.FindInterfaceChannelWithNameInTheCategory(
                    ChannelType.TACVIEWSTORAGE).CreateARawMessageForTheChannelFromMessageName(
                        "", "Match " + _matchId + "'s tacviews", true, null, false, files.ToArray());
        if (cachedUserMessage == null)
        {
            Log.WriteLine(nameof(cachedUserMessage) + " was null!", LogLevel.CRITICAL);
            return Array.Empty<AttachmentData>();
        }

        foreach (var file in cachedUserMessage.Attachments)
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
}