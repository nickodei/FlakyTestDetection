namespace Application.Features.Tests.Entities;

public class Test
{
    public int Id { get; set; }
    public TestStatus Status { get; set; }
    public ICollection<TestAttempts> Attempts { get; set; } = [];
}