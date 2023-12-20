namespace Application.Features.Github.Entities;

public class WorkflowRun
{
    public long WorkflowRunId { get; set; }
    public required long WorkflowId { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
    public required string Event { get; set; }
    public required DateTime CreatedAt { get; set; }
    
    public Workflow? Workflow { get; set; }
    public ICollection<Job> Jobs { get; set; } = [];
}