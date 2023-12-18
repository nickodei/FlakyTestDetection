using Application.Features.Github.Models;

namespace Application.Features.Github.Clients.Responses;

public class GetWorkflowsResponse
{
    public int TotalCount { get; set; }
    public IEnumerable<WorkflowResult> Workflows { get; set; }
}