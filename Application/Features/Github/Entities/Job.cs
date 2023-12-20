namespace Application.Features.Github.Entities;

public class Job
{
    public long JobId { get; set; }
    public required long WorkflowRunId { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
    public required string Conclusion { get; set; }
    public required DateTime StartedAt { get; set; }
    
    public WorkflowRun? WorkflowRun { get; set; }
}