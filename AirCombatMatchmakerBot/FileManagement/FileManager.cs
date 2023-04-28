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