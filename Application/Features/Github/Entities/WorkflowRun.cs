namespace Application.Features.Github.Entities;

public class WorkflowRun
{
    public int Id { get; set; }
    public long WorkflowRunId { get; set; }
    public required string WorkflowName { get; set; }
    
    public int RepositoryId { get; set; }
    public Repository Repository { get; set; } = null!;

    public string GetRepositoryName => $"{Repository?.Owner}/{Repository?.Name}";
}