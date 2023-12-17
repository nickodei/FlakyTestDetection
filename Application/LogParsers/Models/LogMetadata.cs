namespace Application.LogParsers.Models;

public class LogMetadata
{
    public int CountFlakyTests { get; set; }
    public int CountPassedTests { get; set; }
    public int CountFailedTests { get; set; }
    public int CountSkippedTests { get; set; }
}