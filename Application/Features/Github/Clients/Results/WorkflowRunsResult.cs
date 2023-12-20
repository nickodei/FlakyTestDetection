using Application.Features.Github.Models;

namespace Application.Features.Github.Clients.Responses;

public class WorkflowRunsResult
{
    public int TotalCount { get; set; }
    public List<WorkflowRunResult> WorkflowRuns { get; set; }
}