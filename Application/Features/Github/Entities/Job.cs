namespace Application.Features.Github.Entities;

public class Job
{
    public long JobId { get; set; }
    public long WorkflowRunId { get; set; }
    public string Name { get; set; }
    
    public WorkflowRun? WorkflowRun { get; set; }
}