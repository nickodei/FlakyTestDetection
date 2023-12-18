namespace Application.Features.Github.Models;

public class WorkflowRunVm
{
    public long Id { get; set; }
    public required string DisplayTitle { get; set; }
    public required string Status { get; set; }
    public required DateTime CreatedAt { get; set; }
}