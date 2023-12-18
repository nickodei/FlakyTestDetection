using System.Collections;

namespace Application.Features.Github.Clients.Responses;

public class WorkflowListResult
{
    public int TotalCount { get; set; }
    public ICollection<WorkflowResult> Workflows { get; set; } = [];
}