using System.Text;
using System.Text.RegularExpressions;
using Application.Features.Tests.Entities;
using Application.LogParsers.Models;

namespace Application.LogParsers.Parsers;

public partial class PlaywrightLogParser : ILogFileParser
{
    private readonly Regex reachedTestError = ReachedTestError();
    private readonly Regex reachedTestRetry = ReachedTestRetry();
    private readonly Regex reachedSkippedTestCount = ReachedSkippedTestCount();
    private readonly Regex reachedFailedTestCount = ReachedFailedTestCount();
    private readonly Regex reachedFlakyTestCount = ReachedFlakyTestCount();
    private readonly Regex reachedPassedTestCount = ReachedPassedTestCount();
    private readonly Regex reachedDuplicatedErrorMessages = ReachedDuplicatedErrorMessages();
    private readonly Regex reachedShortTestError = ReachedShortTestError();
    
    private enum SummaryState
    {
        None,
        Flaky,
        Failed
    }
    
    public TestFile ParseLogFile(StreamReader stream)
    {
        TestFile result = new();
        var summaryState = SummaryState.None;
        TestIdentifier? currentTestCase = null;

        StringBuilder errorMessage = new();
        var testDictionary = new Dictionary<TestIdentifier, List<TestRun>>();
        for (var currentLine = stream.ReadLine(); currentLine is not null; currentLine = stream.ReadLine())
        {
            // We would else just parse everything twice
            if (reachedDuplicatedErrorMessages.IsMatch(currentLine))
            {
                break;
            }
            
            if (reachedTestError.IsMatch(currentLine))
            {
                if (currentLine.Contains("Process completed with exit code"))
                {
                    continue;
                }

                if (currentTestCase is not null && errorMessage.Length != 0)
                {
                    AddAttemptToTestCase(currentTestCase, errorMessage, testDictionary);
                }

                currentTestCase = GetTestIdentifierFromError(reachedTestError, currentLine);
                switch (summaryState)
                {
                    case SummaryState.Flaky:
                        result.TestResults.Add(new TestResult()
                        {
                            Identifier = currentTestCase,
                            TestStatus = TestStatus.Flaky,
                            Attempts = testDictionary.GetValueOrDefault(currentTestCase) ?? []
                        });
                        currentTestCase = null;
                        continue;
                    case SummaryState.Failed:
                        result.TestResults.Add(new TestResult()
                        {
                            Identifier = currentTestCase,
                            TestStatus = TestStatus.Failed,
                            Attempts = testDictionary.GetValueOrDefault(currentTestCase) ?? []
                        });
                        currentTestCase = null;
                        continue;
                }
                
                testDictionary.Add(currentTestCase, []);
                continue;
            }

            if (reachedTestRetry.IsMatch(currentLine) && currentTestCase is not null)
            {
                AddAttemptToTestCase(currentTestCase, errorMessage, testDictionary);
                continue;
            }

            if (reachedSkippedTestCount.IsMatch(currentLine))
            {
                if (currentTestCase is not null)
                {
                    AddAttemptToTestCase(currentTestCase, errorMessage, testDictionary);
                    currentTestCase = null;
                }
                
                result.CountSkippedTests = ParseIntFromRegexGroup(reachedSkippedTestCount, 1, currentLine);
                continue;
            }
            
            if (reachedFlakyTestCount.IsMatch(currentLine))
            {
                if (currentTestCase is not null)
                {
                    AddAttemptToTestCase(currentTestCase, errorMessage, testDictionary);
                    currentTestCase = null;
                }
                
                summaryState = SummaryState.Flaky;
                result.CountFlakyTests = ParseIntFromRegexGroup(reachedFlakyTestCount, 1, currentLine);
                continue;
            }
            
            if (reachedFailedTestCount.IsMatch(currentLine))
            {
                if (currentTestCase is not null)
                {
                    AddAttemptToTestCase(currentTestCase, errorMessage, testDictionary);
                    currentTestCase = null;
                }

                summaryState = SummaryState.Failed;
                result.CountFailedTests = ParseIntFromRegexGroup(reachedFailedTestCount, 1, currentLine);
                continue;
            }
            
            if (reachedPassedTestCount.IsMatch(currentLine))
            {
                if (currentTestCase is not null)
                {
                    AddAttemptToTestCase(currentTestCase, errorMessage, testDictionary);
                    currentTestCase = null;
                }
                
                result.CountPassedTests = ParseIntFromRegexGroup(reachedPassedTestCount, 1, currentLine);
                break;
            }
            
            if (currentTestCase is not null)
            {
                errorMessage.AppendLine(currentLine[(currentLine.IndexOf('Z') + 1)..]);
            }
        }

        return result;
    }

    private void AddAttemptToTestCase(TestIdentifier identifier, StringBuilder errorMessage, Dictionary<TestIdentifier, List<TestRun>> dictionary)
    {
        if (!dictionary.TryGetValue(identifier, out var testAttempts)) return;
        
        testAttempts.Add(new TestRun()
        {
            identifier = identifier,
            ErrorMessage = errorMessage.ToString()
        });
        
        errorMessage.Clear();
    }
    
    private TestIdentifier GetTestIdentifierFromError(Regex regex, string currentLine)
    {
        if (!regex.IsMatch(currentLine))
        {
            throw new Exception("Couldn't parse TestIdentifier from line");
        }
        
        var match = reachedShortTestError.IsMatch(currentLine) ? reachedShortTestError.Match(currentLine) : regex.Match(currentLine);

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
    
    [GeneratedRegex(@"(.+?)(\[.*\])( › )(.+?)( › )(.*)")]
    private static partial Regex ReachedTestError();

    [GeneratedRegex(@"(.+?)(\[.*\])( › )(.+?)( › )(.+?)(.─)")]
    private static partial Regex ReachedShortTestError();
    
    [GeneratedRegex(@"Retry #([0-9]+) ([─]+)")]
    private static partial Regex ReachedTestRetry();
    
    [GeneratedRegex(@"([0-9]+) passed")]
    private static partial Regex ReachedPassedTestCount();
    
    [GeneratedRegex(@"([0-9]+) skipped")]
    private static partial Regex ReachedSkippedTestCount();
    
    [GeneratedRegex(@"([0-9]+) flaky")]
    private static partial Regex ReachedFlakyTestCount();
    
    [GeneratedRegex(@"([0-9]+) failed")]
    private static partial Regex ReachedFailedTestCount();
    
    [GeneratedRegex(@"(##\[error\])(.+?)(\[.*\])( › )(.+?)( › )(.*)")]
    private static partial Regex ReachedDuplicatedErrorMessages();
}