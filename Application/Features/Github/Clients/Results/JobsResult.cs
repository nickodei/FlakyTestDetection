namespace Application.Features.Github.Clients.Responses;

public class JobsResult
{
    public int TotalCount { get; set; }
    public ICollection<JobResult> Jobs { get; set; } = [];
}

public class JobResult
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
    public required string Conclusion { get; set; }
    public required DateTime StartedAt { get; set; }
}