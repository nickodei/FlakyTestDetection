namespace Application.Features.Github.Clients.Responses;

public class JobsResult
{
    public int TotalCount { get; set; }
    public ICollection<JobResult> Jobs { get; set; } = [];
}

public class JobResult
{
    public long Id { get; set; }
    public string Name { get; set; }
}