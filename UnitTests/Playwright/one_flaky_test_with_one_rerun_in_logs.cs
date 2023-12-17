using FluentAssertions;
using WebAPI.LogParsing.Models;
using WebAPI.LogParsing.Parsers;

namespace UnitTests.Playwright;

public class one_flaky_test_with_one_rerun_in_logs
{
    private readonly PlaywrightLogParser parser = new();
    private const string LogPath = @"..\..\..\Playwright\Logs\OneFlakyTestWithOneRerun.txt";

    [Fact]
    public void returns_one_flaky_tests_after_parsing()
    {
        using var stream = LogReader.ReadFile(LogPath);
        var result = parser.ParseLogFile(stream);

        result.TestResults.Should().HaveCount(1);
        
        var testResult = result.TestResults[0];
        testResult.TestStatus.Should().Be(TestStatus.Flaky);
        testResult.Identifier.Should().BeEquivalentTo(new TestIdentifier(
            "Font Library › When a theme with bundled fonts is active › should display the \"Manage Fonts\" icon",
            "site-editor/font-library.spec.js:45:3",
            "[chromium]"
        ));
        testResult.Attempts.Should().HaveCount(1);
        testResult.Attempts[0].Should().BeEquivalentTo(new TestRun()
        {
            identifier = new TestIdentifier(
            "Font Library › When a theme with bundled fonts is active › should display the \"Manage Fonts\" icon",
            "site-editor/font-library.spec.js:45:3",
            "[chromium]"
            ),
            ErrorMessage = 
                """
                    TimeoutError: locator.click: Timeout 10000ms exceeded.
                    =========================== logs ===========================
                    waiting for frameLocator('[name="editor-canvas"]').locator('body')
                    ============================================================
                
                      40 | 				postType: 'wp_template',
                      41 | 			} );
                    > 42 | 			await editor.canvas.locator( 'body' ).click();
                         | 			                                      ^
                      43 | 		} );
                      44 |
                      45 | 		test( 'should display the "Manage Fonts" icon', async ( { page } ) => {
                
                        at /home/runner/work/gutenberg/gutenberg/test/e2e/specs/site-editor/font-library.spec.js:42:42
                
                """
        });
    }
    
    [Fact]
    public void returns_count_of_passed_tests_after_parsing_correctly()
    {
        using var stream = LogReader.ReadFile(LogPath);
        var result = parser.ParseLogFile(stream);

        result.CountPassedTests.Should().Be(113);
    }
    
    [Fact]
    public void returns_count_of_skipped_tests_after_parsing_correctly()
    {
        using var stream = LogReader.ReadFile(LogPath);
        var result = parser.ParseLogFile(stream);

        result.CountSkippedTests.Should().Be(1);
    }
    
    [Fact]
    public void returns_zero_count_of_failed_tests_after_parsing_correctly()
    {
        using var stream = LogReader.ReadFile(LogPath);
        var result = parser.ParseLogFile(stream);

        result.CountFailedTests.Should().Be(0);
    }
    
    [Fact]
    public void returns_count_of_flaky_tests_after_parsing_correctly()
    {
        using var stream = LogReader.ReadFile(LogPath);
        var result = parser.ParseLogFile(stream);

        result.CountFlakyTests.Should().Be(1);
    }
}