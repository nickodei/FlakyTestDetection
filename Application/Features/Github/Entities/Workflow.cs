namespace Application.Features.Github.Entities;

public class Workflow
{
    public long WorkflowId { get; set; }
    public required long RepositoryId { get; set; }
    public required string Name { get; set; }
    public required string State { get; set; }
    public required DateTime CreatedAt { get; set; }
    public Repository? Repository { get; set; }
    public ICollection<WorkflowRun> WorkflowRuns { get; set; } = [];
}