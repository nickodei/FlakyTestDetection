namespace UnitTests;

public static class LogReader
{
    public static StreamReader ReadFile(string path)
    {
        return new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));
    }
}