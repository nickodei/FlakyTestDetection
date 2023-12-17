using FluentAssertions;
using WebAPI.LogParsing.Parsers;

namespace UnitTests.Playwright;

public class no_test_block_in_log
{
    private readonly PlaywrightLogParser parser = new();
    private const string LogPath = @"..\..\..\Playwright\Logs\NoTestBlock.txt";

    [Fact]
    public void returns_zero_tests_after_parsing()
    {
        using var stream = LogReader.ReadFile(LogPath);
        var result = parser.ParseLogFile(stream);

        result.FailedTests.Should().HaveCount(0);
    }
    
    [Fact]
    public void returns_count_of_passed_tests_after_parsing_correctly()
    {
        using var stream = LogReader.ReadFile(LogPath);
        var result = parser.ParseLogFile(stream);

        result.CountPassedTests.Should().Be(151);
    }
    
    [Fact]
    public void returns_count_of_skipped_tests_after_parsing_correctly()
    {
        using var stream = LogReader.ReadFile(LogPath);
        var result = parser.ParseLogFile(stream);

        result.CountSkippedTests.Should().Be(11);
    }
    
    [Fact]
    public void returns_zero_count_of_failed_tests_after_parsing_correctly()
    {
        using var stream = LogReader.ReadFile(LogPath);
        var result = parser.ParseLogFile(stream);

        result.CountFailedTests.Should().Be(0);
    }
}