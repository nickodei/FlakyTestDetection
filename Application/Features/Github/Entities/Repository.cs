namespace Application.Features.Github.Entities;

public class Repository
{
    public int Id { get; set; }
    public required string Owner { get; set; }
    public required string Name { get; set; }
    public ICollection<WorkflowRun> WorkflowRuns { get; } = new List<WorkflowRun>();
}