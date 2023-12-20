namespace Application.Features.Tests.Entities;

public class TestAttempt
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public Test? Test { get; set; }
    public string? Message { get; set; }
}