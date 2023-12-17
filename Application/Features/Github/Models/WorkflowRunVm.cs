namespace Application.Features.Github.Models;

public class WorkflowRunVm
{
    public long Id { get; set; }
    public string DisplayTitle { get; set; }
    public string Status { get; set; }
}