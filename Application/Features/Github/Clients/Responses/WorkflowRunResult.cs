using Application.Features.Github.Models;

namespace Application.Features.Github.Clients.Responses;

public class WorkflowRunResult
{
    public int TotalCount { get; set; }
    public List<WorkflowRunVm> WorkflowRuns { get; set; }
}