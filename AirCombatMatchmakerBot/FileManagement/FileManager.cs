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

    public async static void SaveFileAttachment(SocketMessage _message, string _filePath, string _fileName)
    {
        foreach (var attachment in _message.Attachments)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(attachment.Url))
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        CheckIfFileAndPathExistsAndCreateItIfNecessary(_filePath, _fileName);
                    }
                }
            }
        }
    }

    public async static void CheckIfFileAndPathExistsAndCreateItIfNecessary(string _filePath, string _fileName)
    {
        string _filePathWithFileName = _filePath + @"\" + _fileName;

        Log.WriteLine("Starting to create: " + _filePathWithFileName, LogLevel.VERBOSE);
        if (!Directory.Exists(_filePath))
        {
            Directory.CreateDirectory(_filePath);
            Log.WriteLine("Done creating: " + _filePath, LogLevel.VERBOSE);
        }
        else
        {
            Log.WriteLine("Already exists, did not create : " + _filePath, LogLevel.VERBOSE);
        }

        Log.WriteLine("Starting to create: " + _filePathWithFileName, LogLevel.VERBOSE);
        if (!File.Exists(_filePathWithFileName))
        {
            using (var fileStream = new FileStream(_filePathWithFileName, FileMode.CreateNew))
            {
                await fileStream.CopyToAsync(fileStream);
                fileStream.Dispose();
                Log.WriteLine("Done creating: " + _filePathWithFileName, LogLevel.VERBOSE);
            }
        }
        else if (File.Exists(_filePathWithFileName) && File.ReadAllText(_filePathWithFileName) == "0")
        {
            using (var fileStream = new FileStream(_filePathWithFileName, FileMode.Truncate))
            {
                await fileStream.CopyToAsync(fileStream);
                fileStream.Dispose();
                Log.WriteLine("Already exists, truncated: " + _filePathWithFileName, LogLevel.VERBOSE);
            }
        }
    }
}