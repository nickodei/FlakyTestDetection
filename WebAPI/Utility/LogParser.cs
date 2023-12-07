namespace WebAPI.Utility;

public class LogParser
{
    public Stream GetLogStreamFromZip(FileStream stream, string path)
    {
        var archive = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read);
        var entry = archive.GetEntry(path);
        if (entry is null)
        {
            throw new Exception("File could not be found");
        }

        return entry.Open();
    }
}