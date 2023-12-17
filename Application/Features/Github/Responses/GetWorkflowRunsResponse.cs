using Application.Features.Github.Models;

namespace Application.Features.Github.Responses;

public class GetWorkflowRunsResponse
{
    public int TotalCount { get; set; }
    public List<WorkflowRunVm> WorkflowRuns { get; set; }
}