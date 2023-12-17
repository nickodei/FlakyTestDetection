namespace Application.LogParsers.Models;

public enum TestStatus
{
    Failed,
    Flaky
}

public class TestResult
{
    public required TestIdentifier Identifier { get; set; }
    public required TestStatus TestStatus { get; set; }
    public List<TestRun> Attempts { get; set; } = [];

    public static TestResult CreateFlayTest(TestIdentifier identifier)
    {
        return new TestResult()
        {
            Identifier = identifier,
            TestStatus = TestStatus.Flaky
        };
    }
    
    public static TestResult CreateFailedTest(TestIdentifier identifier)
    {
        return new TestResult()
        {
            Identifier = identifier,
            TestStatus = TestStatus.Failed
        };
    }
}