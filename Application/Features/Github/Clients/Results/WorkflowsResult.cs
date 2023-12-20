using System.Collections;

namespace Application.Features.Github.Clients.Responses;

public class WorkflowsResult
{
    public int TotalCount { get; set; }
    public ICollection<WorkflowResult> Workflows { get; set; } = [];
}