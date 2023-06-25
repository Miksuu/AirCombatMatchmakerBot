using Discord.WebSocket;

public class TacviewManager
{
    public static Task SaveTacviewFromUserUpload(
    LeagueName _leagueName, int _matchId, SocketMessage _message)
    {
        Log.WriteLine("Saving tacview from user upload on league: " + _leagueName + ", on matchId:" +
            _matchId + ", from user: " + _message.Author.Id);
        string pathToCreate = @"C:\AirCombatMatchmakerBot\Data\Tacviews\" +
            _leagueName.ToString() + @"\" + _matchId.ToString();

        string fileName = "Match-" + _matchId + "_" + _message.Author.Id.ToString() + ".acmi";
        FileManager.SaveFileAttachment(_message, pathToCreate, fileName);

        Log.WriteLine("Done saving tacview from user upload on league: " + _leagueName + ", on matchId:" +
            _matchId + ", from user: " + _message.Author.Id);

        return Task.CompletedTask;
    }

    public static Task<AttachmentData[]> FindTacviewAttachmentsForACertainMatch(
        int _matchId, InterfaceLeague _interfaceLeague)
    {
        Log.WriteLine("Getting tacview from user upload on league: " +
            _interfaceLeague.LeagueCategoryName + ", on matchId:" + _matchId);

        string pathToLookFor = @"C:\AirCombatMatchmakerBot\Data\Tacviews\" +
            _interfaceLeague.LeagueCategoryName.ToString() + @"\" + _matchId.ToString() + @"\";

        if (!Directory.Exists(pathToLookFor))
        {
            Log.WriteLine("Path doesn't exists: " + pathToLookFor, LogLevel.CRITICAL);
            throw new InvalidOperationException("Path doesn't exists: " + pathToLookFor);
        }

        List<string> files = Directory.GetFiles(pathToLookFor).ToList();
        AttachmentData[] tacviewResults = new AttachmentData[files.Count];
        if (files.Count > 2 || files.Count == 0)
        {
            Log.WriteLine("Warning! files count was: " + files.Count, LogLevel.WARNING);
        }

        int index = 0;
        foreach (string fileIndex in files)
        {
            string[] finalFiles = new string[1];
            Discord.IUserMessage cachedUserMessage;

            try
            {
                InterfaceChannel interfaceChannel =
                Database.Instance.Categories.FindInterfaceChannelInsideACategoryWithNames(
                    CategoryType.BOTSTUFF, ChannelType.TACVIEWSTORAGE);

                cachedUserMessage =
                    interfaceChannel.CreateARawMessageForTheChannelFromMessageName(
                        "", "Match " + _matchId + "'s tacviews", true, null, false, finalFiles[0] = fileIndex).Result;
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message, LogLevel.ERROR);
                continue;
            }

            foreach (var file in cachedUserMessage.Attachments)
            {
                Log.WriteLine("Found file: " + file.Url + " on match: " + _matchId);

                string playerUlongString = file.Url.Split('/').Last().Split('_').Last().Split('.').First();
                ulong playerId = ulong.Parse(playerUlongString);

                var teams = _interfaceLeague.LeagueData.Teams;

                var foundTeam = teams.CheckIfPlayersTeamIsActiveByIdAndReturnThatTeam(playerId).TeamName;

                tacviewResults[index] = new AttachmentData(foundTeam + "'s Tacview", file.Url);
                index++;
            }

            Log.WriteLine("Done getting tacview from user upload on league: " + _interfaceLeague.LeagueCategoryName +
                ", on matchId:" + _matchId + ", with count: " + finalFiles.Length);
        }

        return Task.FromResult(tacviewResults);
    }
}