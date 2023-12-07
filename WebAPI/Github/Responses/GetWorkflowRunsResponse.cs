using Newtonsoft.Json;
using Refit;
using WebAPI.Github.Models;

namespace WebAPI.Github.Responses;

public class GetWorkflowRunsResponse
{
    public int TotalCount { get; set; }
    public List<WorkflowRun> WorkflowRuns { get; set; }
}