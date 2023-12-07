using FluentAssertions;
using WebAPI.Utility;

namespace UnitTests;

public class LogParsingTests
{
    private readonly string zipPath = "Playwright - 7/7_Run the tests.txt";
    private readonly string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\TestLogs\githubLogs.zip");
    
    [Fact]
    public void GivenAFilePath_OpeningTheZip_ReturnsNoError()
    {
        using var stream = new LogParser().GetLogStreamFromZip(new FileStream(logPath, FileMode.Open), zipPath);
        stream.Should().NotBeNull();
    }
}