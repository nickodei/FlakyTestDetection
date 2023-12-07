using Refit;
using WebAPI.Github.Responses;

namespace WebAPI.Github;

public interface IGithubApiService
{
    [Get("/repos/{owner}/{repo}/actions/workflows")]
    Task<GetWorkflowsResponse> GetWorkflows(string owner, string repo);

    [Get("/repos/{owner}/{repo}/actions/workflows/{workflowId}/runs")]
    Task<GetWorkflowRunsResponse> GetWorkflowRuns(string owner, string repo, long workflowId);
    
    [Get("/repos/{owner}/{repo}/actions/runs/{workflowRunId}/logs")]
    Task<HttpResponseMessage> GetWorkflowRunLogs(string owner, string repo, long workflowRunId);
}