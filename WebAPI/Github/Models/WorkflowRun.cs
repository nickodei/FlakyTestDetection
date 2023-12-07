namespace WebAPI.Github.Models;

public class WorkflowRun
{
    public long Id { get; set; }
    public string DisplayTitle { get; set; }
    public string Status { get; set; }
}