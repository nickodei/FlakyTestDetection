using System.Text;
using System.Text.RegularExpressions;
using Application.LogParsers.Models;

namespace Application.LogParsers.Parsers;

public enum ParseState
{
    Default,
    Summary,
    Error
}

public partial class PlaywrightLogParser : ILogFileParser
{
    private readonly Regex errorIdentifierRegex = ErrorIdentifierParsing();
    private readonly Regex passedTestsRegex = ContainsCountOfPassedTests();
    private readonly Regex skippedTestsRegex = ContainsCountOfSkippedTests();
    private readonly Regex flakyTestsRegex = ContainsCountOfFlakyTests();
    private readonly Regex failedTestsRegex = ContainsCountOfFailedTests();
    private readonly Regex testSummaryIdentifierParsing = TestSummaryIdentifierParsing();
    
    private readonly Regex reachedError = ReachedError();
    private readonly Regex reachedSummary = ReachedSummary();

    public TestFile ParseLogFile(StreamReader stream)
    {
        TestFile result = new();
        var state = ParseState.Default;
        
        //  Parse the log files
        var testDictionary = new Dictionary<TestIdentifier, List<TestRun>?>();
        for (var currentLine = stream.ReadLine(); currentLine is not null;)
        {
            if (reachedError.IsMatch(currentLine))
            {
                currentLine = ParseError(testDictionary, stream, currentLine);
                continue;
            }
            
            if (state == ParseState.Summary || reachedSummary.IsMatch(currentLine))
            {
                state = ParseState.Summary;
                
                currentLine = ParseSummary(result, stream, currentLine);
                continue;
            }

            currentLine = stream.ReadLine();
        }
        
        // Prepare the test-results
        foreach (var item in result.TestResults)
        {
            if (testDictionary.TryGetValue(item.Identifier, out var testRuns))
            {
                item.Attempts.AddRange(testRuns ?? []);
            }
        }
        
        return result;
    }

    private string? ParseError(Dictionary<TestIdentifier, List<TestRun>?> testDictionary, StreamReader stream, string? currentLine)
    {
        if (currentLine!.Contains("Process completed with exit code"))
        {
            return stream.ReadLine();
        }
        
        if (!errorIdentifierRegex.IsMatch(currentLine!))
        {
            throw new Exception("Couldn't parse identifier of test case");
        }
        
        var identifier = GetTestIdentifierFromError(errorIdentifierRegex, currentLine);
        var failedTest = new TestRun()
        {
            identifier = identifier
        };

        // Read Error Message
        var errorMessage = new StringBuilder();
        for (currentLine = stream.ReadLine(); currentLine is not null && !errorIdentifierRegex.IsMatch(currentLine) &&  !reachedSummary.IsMatch(currentLine); currentLine = stream.ReadLine())
        {
            errorMessage.AppendLine(currentLine);
        }
        
        failedTest.ErrorMessage = errorMessage.ToString();
        if (testDictionary.TryGetValue(identifier, out var tests))
        {
            tests ??= [];
            tests.Add(failedTest);
        }
        else
        {
            testDictionary.Add(identifier, [failedTest]);
        }
        
        return currentLine;
    }

    private string? ParseSummary(TestFile result, StreamReader stream, string? currentLine)
    {
        if (passedTestsRegex.IsMatch(currentLine!))
        {
            result.CountPassedTests = ParseIntFromRegexGroup(passedTestsRegex, 1, currentLine!);
        }
        else if (skippedTestsRegex.IsMatch(currentLine!))
        {
            result.CountSkippedTests = ParseIntFromRegexGroup(skippedTestsRegex, 1, currentLine!);
        }
        else if (flakyTestsRegex.IsMatch(currentLine!))
        {
            result.CountFlakyTests = ParseIntFromRegexGroup(flakyTestsRegex, 1, currentLine!);
            for (currentLine = stream.ReadLine(); currentLine is not null && testSummaryIdentifierParsing.IsMatch(currentLine); currentLine = stream.ReadLine())
            {
                var identifier = GetTestIdentifierFromSummary(testSummaryIdentifierParsing, currentLine);
                result.TestResults.Add(TestResult.CreateFlayTest(identifier));
            }
            
            return currentLine;
        }
        else if (failedTestsRegex.IsMatch(currentLine!))
        {
            result.CountFailedTests = ParseIntFromRegexGroup(failedTestsRegex, 1, currentLine!);
            for (currentLine = stream.ReadLine(); currentLine is not null && testSummaryIdentifierParsing.IsMatch(currentLine); currentLine = stream.ReadLine())
            {
                var identifier = GetTestIdentifierFromSummary(testSummaryIdentifierParsing, currentLine);
                result.TestResults.Add(TestResult.CreateFailedTest(identifier));
            }
            
            return currentLine;
        }

        return stream.ReadLine();
    }

    private static TestIdentifier GetTestIdentifierFromError(Regex regex, string currentLine)
    {
        if (!regex.IsMatch(currentLine))
        {
            throw new Exception("Couldn't parse TestIdentifier from line");
        }
        
        var match = regex.Match(currentLine);

        var name = match.Groups[7].Value.Trim();
        var path = match.Groups[5].Value.Trim();
        var platform = match.Groups[3].Value.Trim();
        
        return new TestIdentifier(name, path, platform);
    }
    
    private static TestIdentifier GetTestIdentifierFromSummary(Regex regex, string currentLine)
    {
        if (!regex.IsMatch(currentLine))
        {
            throw new Exception("Couldn't parse TestIdentifier from Summary");
        }
        
        var match = regex.Match(currentLine);

        var name = match.Groups[6].Value.Trim();
        var path = match.Groups[4].Value.Trim();
        var platform = match.Groups[2].Value.Trim();
        
        return new TestIdentifier(name, path, platform);
    }

    private static int ParseIntFromRegexGroup(Regex regex, int groupIndex, string currentLine)
    {
        var match = regex.Match(currentLine);
        if (!int.TryParse(match.Groups[groupIndex].Value, out var count))
        {
            //TODO: Error?
        }

        return count;
    }
    
    [GeneratedRegex(@"(##\[error\])(.+?)(\[.*\])( › )(.+?)( › )(.*)")]
    private static partial Regex ErrorIdentifierParsing();
    
    [GeneratedRegex(@"(.+?)(\[.*\])( › )(.+?)( › )(.*)")]
    private static partial Regex TestSummaryIdentifierParsing();
    
    [GeneratedRegex(@"(.* )(at )(\/.*)+")]
    private static partial Regex StackTraceParsing();
    
    [GeneratedRegex(@"([0-9]+) passed")]
    private static partial Regex ContainsCountOfPassedTests();
    
    [GeneratedRegex(@"([0-9]+) skipped")]
    private static partial Regex ContainsCountOfSkippedTests();
    
    [GeneratedRegex(@"([0-9]+) flaky")]
    private static partial Regex ContainsCountOfFlakyTests();
    
    [GeneratedRegex(@"([0-9]+) failed")]
    private static partial Regex ContainsCountOfFailedTests();
    
    [GeneratedRegex(@"(##\[error\])(.*)")]
    private static partial Regex ReachedError();
    
    [GeneratedRegex(@"##\[notice\]")]
    private static partial Regex ReachedSummary();
}