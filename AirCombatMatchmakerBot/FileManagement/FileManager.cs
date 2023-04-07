using Discord.WebSocket;
using System;
using System.Text;

static class FileManager
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

        string finalPath = pathToCreate + _message.Author.Id.ToString() + ".acmi";
        SaveFileAttachment(_message, finalPath);

        Log.WriteLine("Done saving tacview from user upload on league: " + _leagueName + ", on matchId:" +
            _matchId + ", from user: " + _message.Author.Id, LogLevel.VERBOSE);

        return Task.CompletedTask;
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
                        using (var fileStream = new FileStream(_finalPath, FileMode.Create))
                        {
                            await stream.CopyToAsync(fileStream);
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