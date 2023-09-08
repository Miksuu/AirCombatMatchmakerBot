using Discord;
using Discord.WebSocket;

public static class MessageReceiver
{
    static List<string> allowedFileFormats = new List<string>
            {
                "acmi", // Tacview file format
                "mp4", "webm", "mov", // videos
                "png", "jpg", "jpeg", "gif", // images
            };

    public async static Task ReceiveMessage(SocketMessage _socketMessage)
    {
        Log.WriteLine("Received message: " + _socketMessage.Content + " in: " + _socketMessage.Channel.Id, LogLevel.DEBUG);

        // Disregards any MessageDescription that's not inside the bot's match channels
        if (!Database.GetInstance<ApplicationDatabase>().MatchChannelsIdWithCategoryId.ContainsKey(
            _socketMessage.Channel.Id))
        {
            var matchChannnelsIdWithCategoryId = Database.GetInstance<ApplicationDatabase>().MatchChannelsIdWithCategoryId;

            Log.WriteLine("Is not in " + nameof(matchChannnelsIdWithCategoryId) + ", returning");
            return;
        }

        string receivedUrl = ReceiveMessageWithAttachmentAndReturnItsURL(_socketMessage, "acmi").Result;

        if (receivedUrl == "")
        {
            Log.WriteLine("Received url was empty, returning");
            return;
        }

        TacviewManager.HandleAcmiUrlOnToTacview(receivedUrl, _socketMessage);

        Log.WriteLine("Done receiving message in: " + _socketMessage.Channel.Id);
    }

    private async static Task<string> ReceiveMessageWithAttachmentAndReturnItsURL(SocketMessage _socketMessage, string _specificFileType = "")
    {
        if (!_socketMessage.Attachments.Any())
        {
            Log.WriteLine("Attachments not found, returning");
            return string.Empty;
        }

        Log.WriteLine("Message: " + _socketMessage.Id + " + detected in: " + _socketMessage.Channel.Id + " by: " +
            _socketMessage.Author.Id + " with " + _socketMessage.Attachments.Count +
            " attachments. Looking for specific file type: " + _specificFileType, LogLevel.DEBUG);

        foreach (var attachment in _socketMessage.Attachments)
        {
            if (attachment == null)
            {
                Log.WriteLine(nameof(attachment) + " was null!", LogLevel.ERROR);
                continue;
            }

            Log.WriteLine("Found attachment: " + attachment.Filename);

            string fileType = FileManager.GetFileTypeOfAnFile(attachment.Filename);

            if (!allowedFileFormats.Contains(fileType) && (_specificFileType != "" && _specificFileType != fileType))
            {
                Log.WriteLine("User: " + _socketMessage.Author.Id + " posted a file that was not " + _specificFileType + " disregarding.");
                continue;
            }

            if (fileType == _specificFileType)
            {
                Log.WriteLine("returning: " + attachment.Url, LogLevel.DEBUG);
                return attachment.Url;
            }

            Log.WriteLine(_socketMessage.Author.Id + " tried to send a file that is not a .acmi file!" +
                " URL:" + attachment.Url, LogLevel.WARNING);

            await _socketMessage.Channel.SendMessageAsync("\n" +
              _socketMessage.Author.Mention +
              ", make sure the attachment you are sending is in a valid format!");

            await _socketMessage.DeleteAsync();
            break;
        }

        Log.WriteLine("Returning empty string", LogLevel.DEBUG);

        return string.Empty;
    }
}