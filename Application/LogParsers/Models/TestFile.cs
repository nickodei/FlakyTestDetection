namespace Application.LogParsers.Models;

public class TestFile
{
    public int CountFlakyTests { get; set; }
    public int CountPassedTests { get; set; }
    public int CountFailedTests { get; set; }
    public int CountSkippedTests { get; set; }
    public List<TestResult> TestResults { get; set; } = [];
    
    public List<TestRun> FailedTests { get; set; } = [];
}