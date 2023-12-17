using Application.Features.Github.Models;

namespace Application.Features.Github.Responses;

public class GetWorkflowsResponse
{
    public int TotalCount { get; set; }
    public IEnumerable<Workflow> Workflows { get; set; }
}