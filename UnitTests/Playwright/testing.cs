using Application.LogParsers.Parsers;
using FluentAssertions;

namespace UnitTests.Playwright;

public class testing
{
    private readonly PlaywrightLogParser parser = new();
    private const string LogPath = @"..\..\..\Playwright\Logs\testing.txt";

    [Fact]
    public void returns_zero_tests_after_parsing()
    {
        using var stream = LogReader.ReadFile(LogPath);
        var result = parser.ParseLogFile(stream);
    }
}