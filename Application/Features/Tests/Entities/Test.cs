namespace Application.Features.Tests.Entities;

public class Test
{
    public int Id { get; set; }
    public int TestSuiteId { get; set; }
    public required string Name { get; set; }
    public TestStatus Status { get; set; }
    public TestSuite? TestSuite { get; set; }
    public ICollection<TestAttempt> Attempts { get; set; } = [];
}